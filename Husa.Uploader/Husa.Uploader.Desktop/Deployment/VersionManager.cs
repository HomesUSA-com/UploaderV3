namespace Husa.Uploader.Desktop.Deployment
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class VersionManager
    {
        private static string appBuildDate;

        public static bool IsDevelopment { get; set; } = false;

        public static string ApplicationBuildVersion { get; set; }

        public static string ApplicationBuildDate
        {
            get
            {
                if (appBuildDate != null)
                {
                    return appBuildDate;
                }

                var timestamp = RetrieveLinkerTimestamp();
                appBuildDate = $"Version 3: {(timestamp.HasValue ? timestamp.Value.ToString("yyyy.MM.dd-HH.mm") : "Unkown")}";

                return appBuildDate;
            }
        }

        public static void ConfigureVersionInfo(bool isDevelopment)
        {
            if (!isDevelopment)
            {
                return;
            }

            IsDevelopment = true;
            ApplicationBuildVersion = "Development";
        }

        public static bool InstallUpdateSyncWithInfo()
        {
            if (IsDevelopment || !ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationBuildVersion = "Development";
                return false;
            }

            ApplicationBuildVersion = ApplicationDeployment.CurrentVersion.ToString();
            var updateCheckInfo = ApplicationDeployment.CheckForUpdate();
            return updateCheckInfo != null && updateCheckInfo.UpdateAvailable;
        }

        private static DateTime? RetrieveLinkerTimestamp()
        {
            var assembly = Assembly.GetCallingAssembly();
            var fileInfo = new FileInfo(assembly.Location);
            return fileInfo.Exists ? fileInfo.CreationTime.ToLocalTime() : null;
        }
    }
}
