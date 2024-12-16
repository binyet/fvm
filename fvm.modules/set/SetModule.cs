using fvm.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fvm.modules.set
{
    public class SetModule : IModuleBase
    {
        public SetModule(ModuleTypeEnum type) : base(type)
        {
        }

        public override bool CheckAction(string[] args)
        {
            return args.Length == 4;
        }

        public override async Task DoAction(string[] args)
        {
            var modulename = args[2];
            var configPath = args[3];
            if (this.CheckConfig(modulename, out ModuleInfo config))
            {
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }
                config.ModulePath = configPath;
                this.SaveConfig(modulename, config);
                this.ConsoleSuccess($"{modulename} save config completed.");
            }
        }
    }
}
