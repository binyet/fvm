using fvm.modules.help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fvm.modules
{
    public static class ModuleFactory
    {
        public static IModuleBase GetModule(ModuleTypeEnum type)
        {
            var impls = typeof(IModuleBase).Assembly.GetTypes().Where(p => !p.IsInterface && !p.IsAbstract && p.IsPublic);

            Type? implType = impls.FirstOrDefault(p => p.IsSubclassOf(typeof(IModuleBase)) && p.Name.ToLower() == type.ToString().ToLower() + "module");
            if(implType == null)
            {
                return new HelpModule(ModuleTypeEnum.help);
            }
            var m = Activator.CreateInstance(implType,type) as IModuleBase;
            if(m == null)
            {
                return new HelpModule(ModuleTypeEnum.help);
            }

            return m;
        }
    }
}
