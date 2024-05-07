namespace Husa.Uploader.Core.Services
{
    using Husa.Migration.Api.Client;
    using Husa.Migration.Api.Contracts.Response;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using UserRequest = Husa.Migration.Api.Contracts.Request.UserRequest;

    public class AuthenticationService : IAuthenticationService
    {
        public static readonly Guid SuperUserId = new Guid("8e445865-a24d-4543-a6c6-9443d048cdb9");

        private readonly ILogger<AuthenticationService> logger;
        private readonly ApplicationOptions appOptions;
        private readonly IMigrationClient migrationClient;

        public AuthenticationService(
            IMigrationClient migrationClient,
            IOptions<ApplicationOptions> appOptions,
            ILogger<AuthenticationService> logger)
        {
            this.migrationClient = migrationClient ?? throw new ArgumentNullException(nameof(migrationClient));
            this.appOptions = appOptions?.Value ?? throw new ArgumentNullException(nameof(appOptions));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserAuthResponse> LoginAsync(string username, string password)
        {
            if (this.appOptions.FeatureFlags.SkipAuthentication)
            {
                this.logger.LogInformation("Skipping authentication of user {Username}", username);
                return DefaultUser();
            }

            try
            {
                var userRequest = new UserRequest(username, password);
                return await this.migrationClient.Connections.LoginAsync(userRequest);
            }
            catch (HttpRequestException httpRequestException)
            {
                this.logger.LogError(httpRequestException, "Authentication failed for user {Username} with response status {Status}", username, httpRequestException.StatusCode);
                return null;
            }
        }

        private static UserAuthResponse DefaultUser() => new()
        {
            UserID = 1,
            UserGUID = SuperUserId.ToString(),
            Username = "superadmin@homesusa.com",
            FirstName = "Super",
            LastName = "Admin",
            FullName = "Super Admin",
        };
    }
}
