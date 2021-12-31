namespace Husa.Uploader.Support
{
    internal class UploaderConfiguration
    {
        public static UploaderConfiguration GetConfiguration()
        {
            return new UploaderConfiguration
            {
                AuthenticateServerUrl = "http://prod-husa-authentication.eastus.azurecontainer.io/api/v1/Authentication/login",
                SingalRRefreshIntervalSeconds = 2,
                SignalRURLServer = "https://husauploadersignalrwebapp.azurewebsites.net/",
                DataRefreshIntervalInSeconds = 45,
                DatabaseConnectionString = "Server=40.121.13.197;Database=MLS;User ID=databasedev;Password=f2w}b/4)#xQssK<g;Trusted_Connection=False;Connection Timeout=15;",
#if !DEBUG
                ElasticSearchServerUrl = "http://logs.homesusa.com:9200/",
                    //DatabaseConnectionString = "Server=40.121.13.197;Database=MLS;User ID=uploaderapp;Password=UPL01User23#.;Trusted_Connection=False;"
                    //DatabaseConnectionString = "Server=40.121.13.197;Database=MLS;User ID=databasedev;Password=f2w}b/4)#xQssK<g;Trusted_Connection=False;Connection Timeout=5;"
#else
                ElasticSearchServerUrl = "http://localhost:9200/",
                //DatabaseConnectionString = "Server=40.121.13.197;Database=MLS;User ID=uploaderapp;Password=UPL01User23#.;Trusted_Connection=False;"
                //DatabaseConnectionString = "Server=40.121.13.197;Database=MLS;User ID=databasedev;Password=f2w}b/4)#xQssK<g;Trusted_Connection=False;Connection Timeout=5;"
#endif
            };
        }

        public string DatabaseConnectionString { get; set; }
        public string ElasticSearchServerUrl { get; set; }
        public int DataRefreshIntervalInSeconds { get; set; }
        public int SingalRRefreshIntervalSeconds { get; set; }
        public string SignalRURLServer { get; set; }
        public string AuthenticateServerUrl { get; set; }
    }
}