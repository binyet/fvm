using System.Security.Principal;

namespace fvm.common
{
    public static class SystemHelper
    {
        /// <summary>
        /// 判断是否是管理员权限启动
        /// </summary>
        /// <returns></returns>
        public static bool CheckIsAdminRunning()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 保存环境变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SaveEnvironmentVariable(string name, string value)
        {
            var target = CheckIsAdminRunning() ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;
            Environment.SetEnvironmentVariable(name, value, target);
        }

        /// <summary>
        /// 获取环境变量
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEnvironmentVariable(string name)
        {
            var target = CheckIsAdminRunning() ? EnvironmentVariableTarget.Machine : EnvironmentVariableTarget.User;
            return Environment.GetEnvironmentVariable(name, target);
        }
    }
}
