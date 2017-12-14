namespace Infinni.Deployer.Helpers
{
    /// <summary>
    /// Manager for applications that hosts as services/daemons.
    /// </summary>
    public interface ISystemServiceManager
    {
        /// <summary>
        /// Create service/daemon for application.
        /// </summary>
        /// <param name="packageId">App package id.</param>
        /// <param name="version">App package version.</param>
        /// <param name="executablePath">Path to application executable file.</param>
        void Create(AppInfo appInfo, string executablePath);

        /// <summary>
        /// Delete service/daemon for application.
        /// </summary>
        /// <param name="packageId">App package id.</param>
        /// <param name="version">App package version.</param>
        void Delete(AppInfo appInfo);

        /// <summary>
        /// Start service/daemon for application.
        /// </summary>
        /// <param name="packageId">App package id.</param>
        /// <param name="version">App package version.</param>
        void Start(AppInfo appInfo);

        /// <summary>
        /// Stop service/daemon for application.
        /// </summary>
        /// <param name="packageId">App package id.</param>
        /// <param name="version">App package version.</param>
        void Stop(AppInfo appInfo);
    }
}