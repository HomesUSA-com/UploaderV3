namespace Husa.Uploader.Core.Services.Common
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Husa.CompanyServicesManager.Api.Contracts.Response;

    public static class LoginCommon
    {
        public static async Task<Dictionary<LoginCredentials, string>> GetMarketCredentials(
            CompanyDetail company,
            Task<ReverseProspectInfoResponse> reverseProspectTask)
        {
            var credentialsDict = new Dictionary<LoginCredentials, string>
            {
                { LoginCredentials.Username, company.BrokerInfo.SiteUsername },
                { LoginCredentials.Password, company.BrokerInfo.SitePassword },
            };

            if (string.IsNullOrEmpty(credentialsDict[LoginCredentials.Username]) || string.IsNullOrEmpty(credentialsDict[LoginCredentials.Password]))
            {
                var marketCredentials = await reverseProspectTask;
                credentialsDict[LoginCredentials.Username] = marketCredentials.UserName;
                credentialsDict[LoginCredentials.Password] = marketCredentials.Password;
            }

            return credentialsDict;
        }
    }
}
