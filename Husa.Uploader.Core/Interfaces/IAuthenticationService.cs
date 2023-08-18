namespace Husa.Uploader.Core.Interfaces
{
    using Husa.Migration.Api.Contracts.Response;

    public interface IAuthenticationService
    {
        Task<UserAuthResponse> LoginAsync(string username, string password);
    }
}
