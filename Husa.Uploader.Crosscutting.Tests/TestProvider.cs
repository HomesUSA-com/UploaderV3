namespace Husa.Uploader.Crosscutting.Tests
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Options;

    public static class TestProvider
    {
        public static ApplicationOptions GetApplicationOptions() => new()
        {
            SignalRURLServer = "https://localhost/signalr",
            DataRefreshIntervalInSeconds = 45,
            SignalRRefreshIntervalSeconds = 2,
            ApplicationId = "some-id",
            ListDateSold = 4,
            PublishingPath = "https://localhost/publish-path",
            FeatureFlags = new()
            {
                EnableDetailedLogs = true,
                EnableSignalR = true,
                IsVersionCheckEnabled = true,
                UseDeveloperMode = true,
            },
            MarketInfo = new()
            {
                Ctx = new()
                {
                    IsEnabled = true,
                    MarketCode = MarketCode.CTX,
                    LoginUrl = "https://localhost/ctx-login",
                    LogoutUrl = "https://localhost/ctx-logout",
                },
                Sabor = new()
                {
                    IsEnabled = true,
                    MarketCode = MarketCode.SanAntonio,
                    LoginUrl = "https://localhost/sabor-login",
                    LogoutUrl = "https://localhost/sabor-logout",
                },
                Abor = new()
                {
                    IsEnabled = true,
                    MarketCode = MarketCode.Austin,
                    LoginUrl = "https://localhost/abor-login",
                    LogoutUrl = "https://localhost/abor-logout",
                },
                Har = new()
                {
                    IsEnabled = true,
                    MarketCode = MarketCode.Houston,
                    LoginUrl = "https://localhost/har-login",
                    LogoutUrl = "https://localhost/har-logout",
                },
                Dfw = new()
                {
                    IsEnabled = true,
                    MarketCode = MarketCode.DFW,
                    LoginUrl = "https://localhost/dfw-login",
                    LogoutUrl = "https://localhost/dfw-logout",
                    AgentId = "Ben Caballero (0096651)",
                    SupervisorId = "Ben Caballero (0096651)",
                },
                Amarillo = new()
                {
                    IsEnabled = true,
                    MarketCode = MarketCode.Amarillo,
                    LoginUrl = "https://localhost/amarillo-login",
                    LogoutUrl = "https://localhost/amarillo-logout",
                    AgentId = "Ben Caballero",
                    SupervisorId = "Ben Caballero",
                },
            },
            Services = new()
            {
                Media = "https://localhost/signalr",
                QuicklisterCtx = "https://localhost/quicklister-ctx",
                QuicklisterSabor = "https://localhost/quicklister-sabor",
                QuicklisterAbor = "https://localhost/quicklister-abor",
                QuicklisterHar = "https://localhost/quicklister-har",
                QuicklisterDfw = "https://localhost/quicklister-dfw",
                QuicklisterAmarillo = "https://localhost/quicklister-amarillo",
                MigrationService = "https://localhost/migration",
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
