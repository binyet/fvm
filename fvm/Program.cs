using fvm.modules;
namespace fvm
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var type = ModuleTypeEnum.help;
            if (args.Length != 0) 
            {
                try
                {
                    type = (ModuleTypeEnum)System.Enum.Parse(typeof(ModuleTypeEnum), args[0]);
                }
                catch 
                {

                }
            }
            await ModuleFactory.GetModule(type).Do(args);
        }
    }
}
