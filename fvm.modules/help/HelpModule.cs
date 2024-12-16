using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace fvm.modules.help
{
    public class HelpModule : IModuleBase
    {
        public HelpModule(ModuleTypeEnum type) : base(type)
        {
        }

        public override bool CheckAction(string[] args)
        {
            return true;
        }

        public override async Task DoAction(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Running Version "+Assembly.GetExecutingAssembly().GetName().Version?.ToString());
            Console.WriteLine();
            Console.WriteLine("Useage:");
            Console.WriteLine();

            Console.WriteLine("fvm [help]\t\t\t\t:Show fvm help info");
            Console.WriteLine("fvm init <modulename>\t\t:init module, generate default config file, the first step");
            Console.WriteLine("fvm get config [modulename]\t\t:get module config for modulename, or all ");
            Console.WriteLine("fvm set config <modulename> <path>\t:set module versions path, and 'current' shortcut will create");
            Console.WriteLine("fvm list [modulename]\t\t\t:show list modulename or all modules useage version");
            Console.WriteLine("fvm use <modulename> <version>\t\t:set modulename use this version");
            Console.WriteLine("fvm remove <modulename> <version>\t:remove modulename this version");
            Console.WriteLine("fvm install <modulename> <version> [url]\t:install module version by install.json or url");
            Console.WriteLine();
            Console.WriteLine();
        }

    }
}
