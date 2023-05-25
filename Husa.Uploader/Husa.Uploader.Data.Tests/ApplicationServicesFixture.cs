namespace Husa.Uploader.Data.Tests
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Options;
    using Microsoft.Extensions.Options;
    using Moq;

    public class ApplicationServicesFixture
    {
        public ApplicationServicesFixture()
        {
            this.ApplicationOptions = new Mock<IOptions<ApplicationOptions>>();

            this.ApplicationOptions
                .SetupGet(o => o.Value)
                .Returns(GetApplicationOptions());
        }

        public Mock<IOptions<ApplicationOptions>> ApplicationOptions { get; set; }

        private static ApplicationOptions GetApplicationOptions() => new()
        {
            AuthenticateServerUrl = "https://localhost/auth",
            ElasticSearchServerUrl = "https://localhost/elastic",
            SignalRURLServer = "https://localhost/signalr",
            DataRefreshIntervalInSeconds = 45,
            SignalRRefreshIntervalSeconds = 2,
            ApplicationId = "some-id",
            ListDateSold = 4,
            MarketInfo = new()
            {
                Ctx = new()
                {
                    IsEnabled = false,
                    MarketCode = MarketCode.CTX,
                    LoginUrl = "https://localhost/sabor-login",
                    LogoutUrl = "https://localhost/sabor-logout",
                },
                Sabor = new()
                {
                    IsEnabled = true,
                    MarketCode = MarketCode.SanAntonio,
                    LoginUrl = "https://localhost/ctx-login",
                    LogoutUrl = "https://localhost/ctx-logout",
                },
            },
            Services = new()
            {
                Media = "https://localhost/signalr",
                QuicklisterCtx = "https://localhost/quicklister-ctx",
                QuicklisterSabor = "https://localhost/quicklister-sabor",
            },
            Uploader = new()
            {
                ChromeOptions = new()
                {
                    Arguments = Array.Empty<string>(),
                    UserProfilePreferences = new List<UserProfilePreference>(),
                },
            },
        };
    }
}
