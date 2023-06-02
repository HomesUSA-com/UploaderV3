namespace Husa.Uploader.Desktop.Deployment
{
    using System;

    public class ApplicationDeployment
    {
        private static ApplicationDeployment currentDeployment = null;
        private static bool currentDeploymentInitialized = false;

        private static bool isNetworkDeployed = false;
        private static bool isNetworkDeployedInitialized = false;

        private ApplicationDeployment()
        {
            // As an alternative solution, we could initialize all properties here
        }

        public static bool IsNetworkDeployed
        {
            get
            {
                if (!isNetworkDeployedInitialized)
                {
                    _ = bool.TryParse(Environment.GetEnvironmentVariable("ClickOnce_IsNetworkDeployed"), out isNetworkDeployed);
                    isNetworkDeployedInitialized = true;
                }

                return isNetworkDeployed;
            }
        }

        public static ApplicationDeployment CurrentDeployment
        {
            get
            {
                if (!currentDeploymentInitialized)
                {
                    currentDeployment = IsNetworkDeployed ? new ApplicationDeployment() : null;
                    currentDeploymentInitialized = true;
                }

                return currentDeployment;
            }
        }

        public static Uri ActivationUri
        {
            get
            {
                _ = Uri.TryCreate(Environment.GetEnvironmentVariable("ClickOnce_ActivationUri"), UriKind.Absolute, out Uri activationUri);
                return activationUri;
            }
        }

        public static Version CurrentVersion
        {
            get
            {
                _ = Version.TryParse(Environment.GetEnvironmentVariable("ClickOnce_CurrentVersion"), out Version version);
                return version;
            }
        }

        public static string DataDirectory
        {
            get { return Environment.GetEnvironmentVariable("ClickOnce_DataDirectory"); }
        }

        public static bool IsFirstRun
        {
            get
            {
                _ = bool.TryParse(Environment.GetEnvironmentVariable("ClickOnce_IsFirstRun"), out bool isFirstRun);
                return isFirstRun;
            }
        }

        public static DateTime TimeOfLastUpdateCheck
        {
            get
            {
                _ = DateTime.TryParse(Environment.GetEnvironmentVariable("ClickOnce_TimeOfLastUpdateCheck"), out DateTime timeOfLastUpdateCheck);
                return timeOfLastUpdateCheck;
            }
        }

        public static string UpdatedApplicationFullName
        {
            get
            {
                return Environment.GetEnvironmentVariable("ClickOnce_UpdatedApplicationFullName");
            }
        }

        public static Version UpdatedVersion
        {
            get
            {
                _ = Version.TryParse(Environment.GetEnvironmentVariable("ClickOnce_UpdatedVersion"), out Version updatedVersion);
                return updatedVersion;
            }
        }

        public static Uri UpdateLocation
        {
            get
            {
                _ = Uri.TryCreate(Environment.GetEnvironmentVariable("ClickOnce_UpdateLocation"), UriKind.Absolute, out Uri updateLocation);
                return updateLocation;
            }
        }

        public static Version LauncherVersion
        {
            get
            {
                _ = Version.TryParse(Environment.GetEnvironmentVariable("ClickOnce_LauncherVersion"), out Version val);
                return val;
            }
        }

        public UpdateCheckInfo CheckForDetailedUpdate(bool persistUpdateCheckResult)
        {
            throw new NotImplementedException();
        }

        internal void Update()
        {
            throw new NotImplementedException();
        }
    }
}
