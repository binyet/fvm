using fvm.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace fvm.modules.init
{
    /// <summary>
    /// init 命令，添加默认的配置文件
    /// </summary>
    public class InitModule : IModuleBase
    {
        public InitModule(ModuleTypeEnum type) : base(type)
        {
        }

        public override bool CheckAction(string[] args)
        {
            return args.Length == 2;
        }

        protected void InitModuleConfig(string modulename)
        {
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            var moduleConfigFilePath = Path.Combine(configPath, "fvm-" + modulename + ".json");
            if (!File.Exists(moduleConfigFilePath))
            {
                File.Create(moduleConfigFilePath).Close();
            }
            var content = File.ReadAllText(moduleConfigFilePath);
            ModuleInfo config = new ModuleInfo();
            if (string.IsNullOrWhiteSpace(content))
            {
                File.WriteAllText(moduleConfigFilePath, JsonSerializer.Serialize(config));
            }
        }

        public override async Task DoAction(string[] args)
        {
            var modulename = args[1];
            this.CheckConfig(modulename, out ModuleInfo config, true);
            if (config != null)
            {
                this.ConsoleError($"{modulename} is already inited, please use set config ...");
                return;
            }

            var modulePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modules", modulename);
            if (!Directory.Exists(modulePath))
            {
                Directory.CreateDirectory(modulePath);
            }
            config = new ModuleInfo
            {
                CurrVersion = "",
                ModulePath = modulePath
            };
            this.SaveConfig(modulename, config);
            this.ConsoleSuccess($"{modulename} init completed.");
        }
    }
}
