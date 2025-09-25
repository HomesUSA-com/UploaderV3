namespace Husa.Uploader.Crosscutting.Options
{
    public class FeatureFlags
    {
        public bool IsVersionCheckEnabled { get; set; }

        public bool EnableDetailedLogs { get; set; }

        public bool UseDeveloperMode { get; set; }

        public bool EnableSignalR { get; set; }

        public bool SkipAuthentication { get; set; }

        public bool EnableBusHandlers { get; set; }
    }
}
