namespace Infinni.Deployer.Helpers
{
    public static class AppsHelper
    {
        public static string GetAppDirectoryName(string packageId, string version)
        {
            return $"{packageId}.{version}";
        }
    }
}