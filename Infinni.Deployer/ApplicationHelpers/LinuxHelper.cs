using System.Diagnostics;

namespace Infinni.Deployer.ApplicationHelpers
{
    public static class LinuxHelper
    {
        public static void SendSigIntToApplication(Process process)
        {
            var taskkill = Process.Start("kill", $"{process.Id}");
            taskkill.WaitForExit();
        }
    }
}