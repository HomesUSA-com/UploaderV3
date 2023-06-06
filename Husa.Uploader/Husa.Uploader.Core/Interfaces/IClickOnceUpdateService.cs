namespace Husa.Uploader.Core.Interfaces
{
    public interface IClickOnceUpdateService
    {
        /// <summary>
        /// Get the current installed version.
        /// </summary>
        /// <returns><see cref="T:System.Version" />.</returns>
        Task<Version> CurrentVersionAsync();

        /// <summary>
        /// Get the remote server version.
        /// </summary>
        /// <returns><see cref="T:System.Version" />.</returns>
        Task<Version> ServerVersionAsync();

        /// <summary>
        /// Manually check if there is a newer version.
        /// </summary>
        /// <returns><see langword="true" /> if there is a newer version available.
        /// </returns>
        Task<bool> UpdateAvailableAsync();
    }
}
