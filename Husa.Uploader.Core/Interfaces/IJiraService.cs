namespace Husa.Uploader.Desktop.Factories
{
    using Husa.Uploader.Core.Models;

    public interface IJiraService
    {
        void InitializeBasicAuth(string baseUrl, string email, string apiToken);
        Task<string> CreateBugAsync(string summary, string description, string projectKey, List<UploaderError> uploaderErrors);
    }
}
