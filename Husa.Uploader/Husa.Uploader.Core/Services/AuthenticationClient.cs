using Husa.Uploader.Core.Interfaces;
using Husa.Uploader.Core.Models;
using Husa.Uploader.Crosscutting.Options;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace Husa.Uploader.Core.Services
{
    public class AuthenticationClient : IAuthenticationClient
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<ApplicationOptions> options;
        private readonly string baseUri;

        public AuthenticationClient(HttpClient httpClient, IOptions<ApplicationOptions> options)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.baseUri = "api/v1/Authentication/login";
        }

        public async Task<UserResponse> LoginAsync(UserRequest userRequest)
        {
            var response = await this.httpClient.PostAsync(this.baseUri, userRequest.AsContent());
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }
    }
}
