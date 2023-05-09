using Husa.Uploader.Core.Models;

namespace Husa.Uploader.Core.Interfaces
{
    public interface IAuthenticationClient
    {
        Task<UserResponse> LoginAsync(UserRequest userRequest);
    }
}