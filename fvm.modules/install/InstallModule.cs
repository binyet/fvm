using fvm.entity;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fvm.modules.install
{
    public class InstallModule : IModuleBase
    {
        public InstallModule(ModuleTypeEnum type) : base(type)
        {
        }

        public override bool CheckAction(string[] args)
        {
            return args.Length <= 4;
        }

        public override async Task DoAction(string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    this.ShowAllCanDownloadVersions();
                    break;
                case 2:
                    this.ShowCanDownloadModuleByModuleName(args[1]);
                    break;
                case 3:
                    await this.InstallModuleVersion(args[1], args[2]);
                    break;
                case 4:
                    await this.InstallModuleVersion(args[1], args[2], args[3]);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 下载模块的某个版本，若url为空则从install.json中找下载地址
        /// </summary>
        /// <param name="modulename"></param>
        /// <param name="version"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task InstallModuleVersion(string modulename, string version, string url = "")
        {
            if(CheckConfig(modulename, out ModuleInfo config))
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    var versions = GetAllCanDownloadModule();
                    if (versions.ContainsKey(modulename))
                    {
                        var versionInfo = versions[modulename].Where(p => p.ContainsKey(version)).FirstOrDefault();
                        if (versionInfo != null)
                        {
                            url = versionInfo[version];
                        }
                    }
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        this.ConsoleError($"module {modulename} about version {version} is not support,you can change install.json file to use");
                        return;
                    }
                }

                string downloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "downloads");
                if (!Directory.Exists(downloadPath))
                {
                    Directory.CreateDirectory(downloadPath);
                }
                var zipFile = $"{modulename}-{version}.zip";
                var fileName = Path.Combine(downloadPath, zipFile);
                await DownloadFileWithProgressAsync(url, fileName);

                var extraPath = Path.Combine(downloadPath, version);

                ZipFile.ExtractToDirectory(fileName, extraPath);

                var subDirs = Directory.GetDirectories(extraPath);

                // 拷贝文件夹到配置路径
                var srcPath = extraPath;
                var dstPath = Path.Combine(config.ModulePath, version);

                if(subDirs.Length == 1 && Directory.Exists(subDirs[0]))
                {
                    // 如果解压后只有一个文件夹  则这个文件夹才是真正的软件包
                    srcPath = subDirs[0];
                    Directory.Move(srcPath, Path.Combine(extraPath, version));
                    srcPath = Path.Combine(extraPath, version);
                }
                CopyDirectory(srcPath, dstPath);
                Directory.Delete(downloadPath, true);
                this.ConsoleSuccess($"module {modulename} version {version} download completed, use list check.");
            }
        }

        /// <summary>
        /// 查询install.json配置文件中可以下载的模块
        /// </summary>
        /// <param name="modulename"></param>
        private void ShowCanDownloadModuleByModuleName(string modulename)
        {

            var versions = GetAllCanDownloadModule();
            if (versions.ContainsKey(modulename))
            {
                ShowCanDownloadModule(modulename, versions[modulename]);
            }
            else
            {
                this.ConsoleError($"{modulename} is not support download, you can change install.json file to use");
            }
        }

        /// <summary>
        /// 获取install.json中维护的数据
        /// </summary>
        private void ShowAllCanDownloadVersions()
        {
            var versions = GetAllCanDownloadModule();
            foreach (var version in versions)
            {
                ShowCanDownloadModule(version.Key, version.Value);
            }
        }

        protected void ShowCanDownloadModule(string modulename, List<Dictionary<string, string>> versions)
        {
            this.ConsoleSuccess(modulename + ":");
            foreach (var version in versions)
            {
                foreach (var item in version)
                {
                    this.ConsoleSuccess($"{item.Key}\t\t:{item.Value}");
                }
            }
        }


        private Dictionary<string, List<Dictionary<string, string>>> GetAllCanDownloadModule()
        {
            try
            {
                var content = File.ReadAllText("install.json");
                var versions = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(content);
                return versions;
            }
            catch (Exception exc)
            {
                return null;
            }
        }


        private async Task DownloadFileWithProgressAsync(string url, string localFilePath)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var totalReadBytes = 0L;

                    if (File.Exists(localFilePath))
                    {
                        File.Delete(localFilePath);
                    }
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var readBytes = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (readBytes == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, readBytes);
                                totalReadBytes += readBytes;
                                ReportProgress(totalReadBytes, totalBytes);
                            }
                        }
                        while (isMoreToRead);
                    }
                }
            }
        }


        private void ReportProgress(long totalReadBytes, long totalBytes)
        {
            if (totalBytes > 0)
            {
                var percentage = (int)((totalReadBytes * 100) / totalBytes);
                Console.Write($"\r下载进度: {percentage}% [{new string('#', percentage)}{new string('-', 100 - percentage)}]");
            }
            else
            {
                Console.Write($"\r已下载: {totalReadBytes} bytes");
            }
        }
    }
}
