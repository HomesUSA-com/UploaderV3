namespace Husa.Uploader.Core.Services
{
    using System.Net;
    using System.Net.Http.Json;
    using Husa.Extensions.Api.Client;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class AuthenticationClient : HusaStandardClient, IAuthenticationClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<AuthenticationClient> logger;
        private readonly ApplicationOptions appOptions;
        private readonly string baseUri;

        public AuthenticationClient(
            HttpClient httpClient,
            IOptions<ApplicationOptions> appOptions,
            IOptions<JsonOptions> jsonOptions,
            ILogger<AuthenticationClient> logger)
            : base(httpClient, jsonOptions)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.appOptions = appOptions?.Value ?? throw new ArgumentNullException(nameof(appOptions));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.baseUri = "api/v1/Authentication/login";
        }

        public async Task<UserResponse> LoginAsync(UserRequest userRequest)
        {
            if (this.appOptions.FeatureFlags.SkipAuthentication)
            {
                this.logger.LogInformation("Skipping authentication of user {@userRequest}", userRequest);
                return UserResponse.DefaultUser();
            }

            var response = await this.httpClient.PostAsync(this.baseUri, userRequest.AsContent());
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                this.logger.LogInformation("Authentication failed for user {@userRequest} with response status {status} with the following reason {reason}", userRequest, response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }
    }
}
