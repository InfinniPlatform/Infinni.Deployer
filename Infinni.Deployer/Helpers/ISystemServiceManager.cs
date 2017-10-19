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
        void Create(string packageId, string version, string executablePath);

        /// <summary>
        /// Delete service/daemon for application.
        /// </summary>
        /// <param name="packageId">App package id.</param>
        /// <param name="version">App package version.</param>
        void Delete(string packageId, string version);

        /// <summary>
        /// Start service/daemon for application.
        /// </summary>
        /// <param name="packageId">App package id.</param>
        /// <param name="version">App package version.</param>
        void Start(string packageId, string version);

        /// <summary>
        /// Stop service/daemon for application.
        /// </summary>
        /// <param name="packageId">App package id.</param>
        /// <param name="version">App package version.</param>
        void Stop(string packageId, string version);
    }
}