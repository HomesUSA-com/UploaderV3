namespace Husa.Uploader.Core.Services
{
    using System.Net;
    using System.Net.Http.Json;
    using Husa.Extensions.Api.Client;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    public class AuthenticationClient : HusaStandardClient, IAuthenticationClient
    {
        private readonly HttpClient httpClient;
        private readonly string baseUri;

        public AuthenticationClient(HttpClient httpClient, IOptions<JsonOptions> jsonOptions)
            : base(httpClient, jsonOptions)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
