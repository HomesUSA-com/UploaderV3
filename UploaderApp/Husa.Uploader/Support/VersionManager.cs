using System;
using System.Deployment.Application;
using System.IO;
using System.Reflection;
using Serilog;

namespace Husa.Cargador.Support
{
    internal static class VersionManager
    {
        private static string _appBuildDate;

        public static string ApplicationBuildVersion { get; set; }

        public static string ApplicationBuildDate
        {
            get
            {
                if (_appBuildDate != null)
                    return _appBuildDate;
                var te = RetrieveLinkerTimestamp();
                _appBuildDate = "Version: " + (te.HasValue ? te.Value.ToString("yyyy.MM.dd-HH.mm") : "Unkown");

                return _appBuildDate;
            }
        }

        private static DateTime? RetrieveLinkerTimestamp()
        {
            string filePath = Assembly.GetCallingAssembly().Location;
            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            Stream s = null;

            try
            {
                s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = BitConverter.ToInt32(b, cPeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(b, i + cLinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.ToLocalTime();
            return dt;
        }

        public static bool InstallUpdateSyncWithInfo()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationBuildVersion = "Development";
                return false;
            }
            
            var activeDeployment = ApplicationDeployment.CurrentDeployment;

            ApplicationBuildVersion = activeDeployment.CurrentVersion.ToString();

            UpdateCheckInfo info;
            try
            {
                info = activeDeployment.CheckForDetailedUpdate();
            }
            catch (DeploymentDownloadException ex)
            {
                Log.Error(ex, "The new version of the application cannot be downloaded at this time. Please check your network connection, or try again later");
                return false;
            }
            catch (InvalidDeploymentException ex)
            {
                Log.Error(ex, "Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again.");
                return false;
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "This application cannot be updated. It is likely not a ClickOnce application.");
                return false;
            }

            if (!info.UpdateAvailable)
                return false;

            try
            {
                activeDeployment.Update();
                return true;
            }
            catch (DeploymentDownloadException ex)
            {
                Log.Error(ex, "Cannot install the latest version of the application. Please check your network connection, or try again later.");
                return false;
            }
        }
    }
}