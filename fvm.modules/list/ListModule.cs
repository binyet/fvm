using fvm.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fvm.modules.list
{
    public class ListModule : IModuleBase
    {
        public ListModule(ModuleTypeEnum type) : base(type)
        {
        }

        public override bool CheckAction(string[] args)
        {
            return args.Length == 2;
        }

        public override async Task DoAction(string[] args)
        {
            var modulename = args[1];
            if (this.CheckConfig(modulename, out ModuleInfo config))
            {
                if (!Directory.Exists(config.ModulePath))
                {
                    this.ConsoleError($"{modulename} config path: {config.ModulePath} is not exist, use set config {modulename} first");
                    return;
                }

                this.GetModuleVersions(config).ForEach(version =>
                {
                    var msg = "\t" + version;
                    if (config.CurrVersion == version)
                    {
                        msg = "*" + msg;
                    }
                    this.ConsoleSuccess(msg);
                });
            }
        }
    }
}
