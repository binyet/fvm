using fvm.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fvm.modules.get
{
    public class GetModule : IModuleBase
    {
        public GetModule(ModuleTypeEnum type) : base(type)
        {
        }

        public override bool CheckAction(string[] args)
        {
            return args.Length == 2 || args.Length == 3;
        }

        public override async Task DoAction(string[] args)
        {
            var configs = new Dictionary<string, ModuleInfo>();

            if (args.Length == 3)
            {
                var modulename = args[2];

                if (this.CheckConfig(modulename, out ModuleInfo config))
                {
                    configs.Add(modulename, config);
                }
                else
                {
                    return;
                }
            }
            else
            {
                configs = this.GetAllModuleConfigs();
            }
            foreach (var config in configs)
            {

                this.ConsoleSuccess(config.Key);
                this.ConsoleSuccess("\tcurrent version: \t" + config.Value.CurrVersion);
                this.ConsoleSuccess("\tmodulePath:\t" + config.Value.ModulePath);
            }
        }

    }
}
