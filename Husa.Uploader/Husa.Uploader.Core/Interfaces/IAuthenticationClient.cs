namespace Husa.Uploader.Core.Interfaces
{
    using Husa.Uploader.Core.Models;

    public interface IAuthenticationClient
    {
        Task<UserResponse> LoginAsync(UserRequest userRequest);
    }
}
