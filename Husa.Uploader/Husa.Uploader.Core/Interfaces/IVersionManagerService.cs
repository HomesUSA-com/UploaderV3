namespace Husa.Uploader.Core.Interfaces
{
    public interface IVersionManagerService
    {
        /// <summary>
        /// Manually check if there is a newer version.
        /// </summary>
        /// <returns><see langword="true" /> if there is a newer version available.
        /// </returns>
        Task<bool> CheckForUpdateAsync();
    }
}
