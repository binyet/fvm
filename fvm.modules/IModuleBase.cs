using fvm.entity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace fvm.modules
{
    /// <summary>
    /// 模块基类
    /// </summary>
    public abstract class IModuleBase
    {
        protected string configPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config");

        private ModuleTypeEnum type;
        protected IModuleBase(ModuleTypeEnum type)
        {
            this.type = type;
        }

        public abstract Task DoAction(string[] args);
        public abstract bool CheckAction(string[] args);
        public async Task Do(string[] args)
        {
            if (CheckAction(args))
            {
                await DoAction(args);
            }
            else
            {
                ConsoleError("incorrect cmd arg, use 'fvm help' search");
            }
        }

        /// <summary>
        /// 获取所有配置信息
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, ModuleInfo> GetAllModuleConfigs()
        {
            var rst = new Dictionary<string, ModuleInfo>();
            foreach (var filePath in Directory.GetFiles(configPath, "fvm-*.json"))
            {
                var moduleName = Path.GetFileNameWithoutExtension(filePath).Replace("fvm-", "");
                if (!rst.ContainsKey(moduleName))
                {
                    rst.Add(moduleName, GetModuleFileConfig(moduleName));
                }
            };
            return rst;
        }

        /// <summary>
        /// 根据模块名称获取模块配置信息
        /// </summary>
        /// <param name="modulename"></param>
        /// <returns></returns>
        private ModuleInfo GetModuleFileConfig(string modulename)
        {
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            var moduleConfigFilePath = Path.Combine(configPath, "fvm-" + modulename + ".json");
            if (!File.Exists(moduleConfigFilePath))
            {
                return null;
            }
            var content = File.ReadAllText(moduleConfigFilePath);
            ModuleInfo config = new ModuleInfo();
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            else
            {
                config = JsonSerializer.Deserialize<ModuleInfo>(content);
            }

            return config;
        }

        /// <summary>
        /// 检查模块配置是否存在，若存在 out 配置信息
        /// </summary>
        /// <param name="modulename"></param>
        /// <param name="config"></param>
        /// <param name="isinit"></param>
        /// <returns></returns>
        protected bool CheckConfig(string modulename, out ModuleInfo config, bool isinit  = false)
        {
            config = GetModuleFileConfig(modulename);
            if (config == null && !isinit)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleError($"none module {modulename} config, run fvm init {modulename} first.");
                Console.ResetColor();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 打印错误信息
        /// </summary>
        /// <param name="message"></param>
        protected void ConsoleError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// 打印提醒信息
        /// </summary>
        /// <param name="message"></param>
        protected void ConsoleWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// 打印成功信息
        /// </summary>
        /// <param name="message"></param>
        protected void ConsoleSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// 保存模块配置
        /// </summary>
        /// <param name="modulename"></param>
        /// <param name="config"></param>
        protected void SaveConfig(string modulename, ModuleInfo config)
        {
            var moduleConfigFilePath = Path.Combine(configPath, "fvm-" + modulename + ".json");
            File.WriteAllText(moduleConfigFilePath, JsonSerializer.Serialize(config));
        }

        /// <summary>
        /// 获取模块可用版本信息
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        protected List<string> GetModuleVersions(ModuleInfo config)
        {
            if (!Directory.Exists(config.ModulePath))
            {
                this.ConsoleError($"config path: {config.ModulePath} is not exist, use set config  first");
                return new List<string>();
            }
            return Directory.GetDirectories(config.ModulePath).Where(p=>!p.EndsWith("current")).Select(p => Path.GetFileName(p)).ToList();
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destDir"></param>
        protected void CopyDirectory(string sourceDir, string destDir)
        {
            try
            {

                // 检查目标目录是否存在，如果不存在则创建它
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                // 获取源目录中的所有文件和子目录
                foreach (string file in Directory.GetFiles(sourceDir))
                {
                    string destFile = Path.Combine(destDir, Path.GetFileName(file));
                    File.Copy(file, destFile, true); // 覆盖已存在的文件
                }

                foreach (string subDir in Directory.GetDirectories(sourceDir))
                {
                    string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                    CopyDirectory(subDir, destSubDir); // 递归调用以复制子目录
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(11);
            }
        }
    }
}
