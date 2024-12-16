using fvm.common;
using fvm.entity;

namespace fvm.modules.use
{
    public class UseModule : IModuleBase
    {
        public UseModule(ModuleTypeEnum type) : base(type)
        {
        }

        public override bool CheckAction(string[] args)
        {
            return args.Length == 3;
        }

        public override async Task DoAction(string[] args)
        {
            var modulename = args[1];
            var version = args[2];
            if (this.CheckConfig(modulename, out ModuleInfo config))
            {
                var listVersions = this.GetModuleVersions(config);
                if (listVersions.Any(p => p == version))
                {
                    //var current = Directory.GetFiles(config.ModulePath, "current.lnk").FirstOrDefault();
                    //if (current != null)
                    //{
                    //    File.Delete(current);
                    //}

                    //current = Path.Combine(config.ModulePath, "current.lnk");
                    //this.CreateShortCutWindows(Path.Combine(config.ModulePath, version), current);

                    var currentDir = Path.Combine(config.ModulePath, "current");
                    var versiondir = Path.Combine(config.ModulePath, version);
                    if (Directory.Exists(currentDir))
                    {
                        Directory.Delete(currentDir, true);
                    }

                    this.CopyDirectory(versiondir, currentDir);


                    config.CurrVersion = version;
                    this.SaveConfig(modulename, config);
                    // 设置环境变量
                    var envPath = currentDir;
                    if (Directory.GetDirectories(versiondir, "bin", SearchOption.TopDirectoryOnly).Length != 0)
                    {
                        // 如果目录下包含bin目录，则将bin目录设置到环境变量中
                        envPath = Path.Combine(envPath, "bin");
                    }
                    SystemHelper.SaveEnvironmentVariable($"FVM_{modulename.ToUpper()}_HOME", envPath);


                    var paths = SystemHelper.GetEnvironmentVariable("PATH");
                    if(!paths.Split(";").Any(p=>p == envPath || p.IndexOf($"FVM_{modulename.ToUpper()}_HOME") != -1))
                    {
                        paths += $";%FVM_{modulename.ToUpper()}_HOME%";
                        SystemHelper.SaveEnvironmentVariable("PATH", paths);
                    }


                    this.ConsoleSuccess($"{modulename} set successfully! current version is {version}.");
                    if (!SystemHelper.CheckIsAdminRunning())
                    {
                        this.ConsoleWarning("current cmd is not Administrator runing, only change User environment, using Administrator to run cmd can modify machine environment variables");
                    }
                }
                else
                {
                    this.ConsoleError($"none version is {version}");
                }
            }
        }

        private void CreateShortCutWindows(string dstPath, string shortCutPath)
        {
            try
            {
                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                var shortcut = shell.CreateShortcut(shortCutPath);
                shortcut.TargetPath = dstPath;
                shortcut.Save();
            }
            catch (Exception exc)
            {
                this.ConsoleError(exc.ToString());
            }
        }


    }
}
