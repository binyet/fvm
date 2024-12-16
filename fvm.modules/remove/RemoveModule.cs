using fvm.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fvm.modules.remove
{
    public class RemoveModule : IModuleBase
    {
        public RemoveModule(ModuleTypeEnum type) : base(type)
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
            if (CheckConfig(modulename, out ModuleInfo config))
            {
                var versionPath = Directory.GetDirectories(config.ModulePath, version, SearchOption.TopDirectoryOnly)?.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(versionPath))
                {
                    this.ConsoleError($"{modulename} of {version} is not exist!");
                    return;
                }
                this.ConsoleWarning($"please confirm remove module {modulename} of version {version}, y:confirm,n:exit");
                var confirm = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(confirm))
                {
                    confirm = Console.ReadLine();
                }
                if(confirm.ToLower() == "y")
                {
                    Directory.Delete(versionPath, true);

                    this.ConsoleSuccess($"module {modulename} version {version} is removed");
                }
                
            }
        }
    }
}
