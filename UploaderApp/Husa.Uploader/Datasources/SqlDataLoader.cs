using Dapper;
using Husa.Uploader.EventLog;
using Husa.Uploader.Support;
using Husa.Uploader.ViewModels;
using Husa.Uploader.ViewModels.Enum;
using Husa.Core.UploaderBase;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Husa.Uploader.Datasources
{
    public class SqlDataLoader
    {
        private readonly string _connectionString;
        private Container saleContainer;
        private static readonly Regex IntCleaner = new Regex("[^1234567890]", RegexOptions.Compiled);
        public static DatabaseState dbConnectionState = DatabaseState.Online;
        private readonly ClassTransform classTransform = new ClassTransform();

        public SqlDataLoader(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<ResidentialListingRequest>> GetListingData()
        {
            var listingRequest = await GetDataInternalListingsCosmoDB();
            var internalRLRCosmo = classTransform.CosmoObjectToResidentialListingRequest(listingRequest);
            ////var internalListings = await GetDataInternalListings(" ([rl].[SysStatusID] = 1 AND [rl].[SysState] <> 'D' AND [m].[IsValidMarket] = 1 AND ([rl].[TransactionType] <> 'FL' OR [rl].[TransactionType] IS NULL) ) ");
            return internalRLRCosmo.Distinct();
        }

        public Task<IEnumerable<ResidentialListingRequest>> GetLeasingData()
        {
            return GetDataInternalLease(" ([rl].[SysStatusID] = 1 AND [rl].[SysState] <> 'D' AND [m].[IsValidMarket] = 1 ) ");
        }

        public Task<IEnumerable<ResidentialListingRequest>> GetLotsData()
        {
            return GetDataInternalLots("([l].[SysStatusID] = 1 AND [l].[SysState] <> 'D' AND [m].[IsValidMarket] = 1)");
        }

        public Task<IEnumerable<ResidentialListingRequest>> GetListing(string residentialListingRequestId)
        {
            if (String.IsNullOrEmpty(residentialListingRequestId))
                return GetDataInternalListings(" ([rl].[SysStatusID] = 1 AND [rl].[SysState] <> 'D' AND [m].[IsValidMarket] = 1 AND ([rl].[TransactionType] <> 'FL' OR [rl].[TransactionType] IS NULL) )");
            else
            {
                var cleanResidentialListingRequestId = IntCleaner.Replace(residentialListingRequestId.Trim(), "");
#if !DEBUG
                return GetDataInternalListings(string.Format(" (([rl].[ResidentialListingRequestId] = {0} OR [rl].[StreetNum] = {0}) AND [rl].[SysStatusID] = 1 AND [rl].[SysState] <> 'D' AND [m].[IsValidMarket] = 1  AND ([rl].[TransactionType] <> 'FL' OR [rl].[TransactionType] IS NULL) ) ", cleanResidentialListingRequestId));
#else
                return GetDataInternalListings(string.Format(" ([rl].[ResidentialListingRequestId] = {0}) ", cleanResidentialListingRequestId));
#endif
            }
        }

        public Task<IEnumerable<ResidentialListingRequest>> GetLeasing(string residentialLeaseRequestID)
        {
            if (String.IsNullOrEmpty(residentialLeaseRequestID))
                return GetDataInternalLease("([rl].[SysStatusID] = 1 AND [rl].[SysState] <> 'D' AND [m].[IsValidMarket] = 1 )");
            else
            {
                var cleanResidentialLeaseRequestID = IntCleaner.Replace(residentialLeaseRequestID.Trim(), "");
#if !DEBUG
                return GetDataInternalLease(string.Format("(([rl].[ResidentialLeaseRequestID] = {0} OR [rl].[StreetNum] = {0}) AND [rl].[SysStatusID] = 1 AND [rl].[SysState] <> 'D' AND [m].[IsValidMarket] = 1 )", cleanResidentialLeaseRequestID));
#else
                return GetDataInternalLease(string.Format("([rl].[ResidentialLeaseRequestID] = {0})", cleanResidentialLeaseRequestID));
#endif
            }
        }

        public Task<IEnumerable<ResidentialListingRequest>> GetLot(string internalLotRequestID)
        {
            if (String.IsNullOrEmpty(internalLotRequestID))
                return GetDataInternalLots(" ([l].[SysStatusID] = 1 AND [l].[SysState] <> 'D' AND [m].[IsValidMarket] = 1 AND [l].[InternalLotRequestID] > 0) ");
            else
            {
                var cleanInternalLotRequestID = IntCleaner.Replace(internalLotRequestID.Trim(), "");
#if !DEBUG
                return GetDataInternalListings(string.Format("(([l].[InternalLotRequestID] = {0} OR [l].[StreetNum] = {0}) AND [l].[SysStatusID] = 1 AND [l].[SysState] <> 'D' AND [m].[IsValidMarket] = 1)", cleanInternalLotRequestID));
#else
                return GetDataInternalLots(string.Format(" ([l].[InternalLotRequestID] = {0}) ", cleanInternalLotRequestID));
#endif
            }
        }

        private async Task<IEnumerable<ListingRequestSale>> GetDataInternalListingsCosmoDB()
        {
            try
            {
                Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient("https://dfwcosmosdb.documents.azure.com:443/", "aDBWzrq8xMCFRYVmPKxHRBVumR5ESgcijQcejMSTF9ZjHwCjfrp4zNy2DGpPxBQ56oj3ELVQ93dbQFNfDJGYqw==");
                this.saleContainer = client.GetContainer("saborDev", "saleRequestQA");
                Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync("saborDev");
                await database.Database.CreateContainerIfNotExistsAsync("saleRequestQA", "/ListingSaleId");
                CancellationToken token;

                using (
                    var query =
                    this.saleContainer.GetItemLinqQueryable<ListingRequestSale>(false)
                    .Where(x => x.RequestState == ListingRequestState.Pending && !x.IsDeleted)
                    .ToFeedIterator()
                    )
                {
                    if (query.HasMoreResults)
                    {
                        return (await query.ReadNextAsync(token)).ToList();
                    }
                }
            } 
            catch(Exception ex)
            {
                string andy = "ërror";
            }

            return null;
        }

        private async Task<IEnumerable<ResidentialListingRequest>> GetDataInternalListings(string whereClause)
        {
            IEnumerable<ResidentialListingRequest> data;
            IEnumerable<ResidentialListingRequest> dataCTX;

            if (dbConnectionState == DatabaseState.Failed)
                throw new Exception();

            try
            {
                var sql = new SqlConnection(_connectionString);
                sql.Open();
                String query = string.Format(@"
                    SELECT
                         [m].[Name] AS MarketName,
                         [b].[SiteUsername] AS MarketUsername,
                         [b].[SitePassword] AS MarketPassword,
                         [b].[LicenseNum] AS BrokerLicenseNum,
						 [a].[FullName] as [BrokerName],
                         [cp].[RealtorContactEmail] as CommunityProfileRealtorContactEmail,
                         [cp].[CommunityName] AS CommunityName, 
                         [cp].[SalesOfficeStreetName] AS CommunityProfileSalesOfficeStreetName,
                         [cp].[SalesOfficeStreetNum] AS CommunityProfileSalesOfficeStreetNum,
                         [cp].[SalesOfficeCity] AS CommunityProfileSalesOfficeCity,
                         [cp].[SalesOfficeZip] AS CommunityProfileSalesOfficeZip ,
                         [cp].[Phone] AS CommunityProfilePhone,
                         [comp].[Name] AS CompanyName, 
                         [comp].[IncludeRemarks] AS IncludeRemarks, 
                         [comp].[AgentListApptPhone] AS AgentListApptPhoneFromCompany, 
                         [comp].[RemarksFormat] AS RemarksFormatFromCompany, 
                         [cp].[AgentListApptPhone] AS AgentListApptPhoneFromCommunityProfile,   
                         [comp].[AlternatePhone] AS AlternatePhoneFromCompany, 
                         [cp].[OtherPhone] AS OtherPhoneFromCommunityProfile,
                         [comp].[ContactEmail] AS ContactEmailFromCompany,
                         [comp].[AutopopulateExpirationDate] as AutopopulateExpirationDate,
                         [cp].[RealtorContactEmail] AS RealtorContactEmailFromCommunityProfile,
                         [pp].[PlanName] AS PlanProfileName,    
                         [ag].[UID] AS SellingAgentUID,
                         [ag].[LicenseNum] AS SellingAgentLicenseNum,
                         [ag].[LastName] AS SellingAgentLastName,
                         [ag].[FirstName] AS SellingAgentFristName,
                         [ag].[UIDOFFICE] AS SellingAgentUIDOFFICE,
                         [rlra].[liststatus] as [OldListStatus],
						 [o].[UID] as [BrokerOffice],
                         [rl].*,
                         (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                            JOIN [METABASE].[dbo].[Market] mPD ON [rl].[MarketID] = [mPD].[Code]
                            JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'ListStatus'                                
                            JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                          WHERE  [lmd].[Code] = [rl].[ListStatus] AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                         ) as [ListStatusName],
                         (SELECT COUNT(ss.[ServiceSubscriptionID]) FROM [SERVICES].[dbo].[ServiceSubscription] ss WHERE ss.[ServiceID] = 5 AND ss.[SysState] != 'D' AND ss.[CompanyID] = comp.[CompanyID]) AS ServiceSubscription,
                         (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                            JOIN [METABASE].[dbo].[Market] mPD ON [rl].[MarketID] = [mPD].[Code]
                            JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'PlannedDevelopment'                                
                            JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                          WHERE  [lmd].[Code] = [rl].[PlannedDevelopment]  AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                         ) as [MasterPlannedCommunityName],
                         (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                            JOIN [METABASE].[dbo].[Market] mPD ON [rl].[MarketID] = [mPD].[Code]
                            JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'SchoolDistrict'                                
                            JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                          WHERE  [lmd].[Code] = [rl].[SchoolDistrict]  AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                         ) as [SchoolDistrictLongName],
                         (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                            JOIN [METABASE].[dbo].[Market] mPD ON [rl].[MarketID] = [mPD].[Code]
                            JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'GolfCourseName'                                
                            JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                          WHERE  [lmd].[Code] = [rl].[GolfCourseName]  AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                         ) as [GolfCourseFullName],
					CASE WHEN rl.[AgentID_SELL2] is not null THEN (select UID FROM [MLS].[dbo].[Agent] WHERE [rl].[AgentID_SELL2] = [MLS].[dbo].[Agent].[AgentID]) ELSE NULL END  AS SellingAgent2UID,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeSun ELSE cp.OHStartTimeSun END AS OHStartTimeSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeMon ELSE cp.OHStartTimeMon END AS OHStartTimeMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeTue ELSE cp.OHStartTimeTue END AS OHStartTimeTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeWed ELSE cp.OHStartTimeWed END AS OHStartTimeWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeThu ELSE cp.OHStartTimeThu END AS OHStartTimeThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeFri ELSE cp.OHStartTimeFri END AS OHStartTimeFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeSat ELSE cp.OHStartTimeSat END AS OHStartTimeSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeSun ELSE cp.OHEndTimeSun END AS OHEndTimeSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeMon ELSE cp.OHEndTimeMon END AS OHEndTimeMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeTue ELSE cp.OHEndTimeTue END AS OHEndTimeTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeWed ELSE cp.OHEndTimeWed END AS OHEndTimeWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeThu ELSE cp.OHEndTimeThu END AS OHEndTimeThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeFri ELSE cp.OHEndTimeFri END AS OHEndTimeFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeSat ELSE cp.OHEndTimeSat END AS OHEndTimeSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeSun ELSE cp.OHTypeSun END AS OHTypeSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeMon ELSE cp.OHTypeMon END AS OHTypeMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeTue ELSE cp.OHTypeTue END AS OHTypeTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeWed ELSE cp.OHTypeWed END AS OHTypeWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeThu ELSE cp.OHTypeThu END AS OHTypeThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeFri ELSE cp.OHTypeFri END AS OHTypeFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeSat ELSE cp.OHTypeSat END AS OHTypeSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsSun ELSE cp.OHRefreshmentsSun END AS OHRefreshmentsSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsMon ELSE cp.OHRefreshmentsMon END AS OHRefreshmentsMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsTue ELSE cp.OHRefreshmentsTue END AS OHRefreshmentsTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsWed ELSE cp.OHRefreshmentsWed END AS OHRefreshmentsWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsThu ELSE cp.OHRefreshmentsThu END AS OHRefreshmentsThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsFri ELSE cp.OHRefreshmentsFri END AS OHRefreshmentsFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshepa chamo.. buenos dimentsSat ELSE cp.OHRefreshmentsSat END AS OHRefreshmentsSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHLunchSun ELSE cp.OHLunchSun END AS OHLunchSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHLunchMon ELSE cp.OHLunchMon END AS OHLunchMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHLunchTue ELSE cp.OHLunchTue END AS OHLunchTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHLunchWed ELSE cp.OHLunchWed END AS OHLunchWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHLunchThu ELSE cp.OHLunchThu END AS OHLunchThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHLunchFri ELSE cp.OHLunchFri END AS OHLunchFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHLunchSat ELSE cp.OHLunchSat END AS OHLunchSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsSun ELSE cp.OHCommentsSun END AS OHCommentsSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsMon ELSE cp.OHCommentsMon END AS OHCommentsMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsTue ELSE cp.OHCommentsTue END AS OHCommentsTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsWed ELSE cp.OHCommentsWed END AS OHCommentsWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsThu ELSE cp.OHCommentsThu END AS OHCommentsThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsFri ELSE cp.OHCommentsFri END AS OHCommentsFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsSat ELSE cp.OHCommentsSat END AS OHCommentsSatOH,
                    (SELECT COUNT(ss.[ServiceSubscriptionID]) FROM [SERVICES].[dbo].[ServiceSubscription] ss WHERE ss.[ServiceID] = 5 AND ss.[SysState] != 'D' AND ss.[CompanyID] = comp.[CompanyID]) AS ServiceSubscription
                  FROM [MLS].[dbo].[ResidentialListingRequest] AS rl 
                  LEFT OUTER JOIN [METABASE].[dbo].[Market] AS m ON [rl].[MarketID] = [m].[Code]
                  LEFT OUTER JOIN [MLS].[dbo].[CommunityProfile] AS cp ON [rl].[CommunityProfileID] = [cp].[CommunityProfileID]
                  LEFT OUTER JOIN [MLS].[dbo].[PlanProfile] AS pp ON [rl].[PlanProfileID] = [pp].[PlanProfileID]
                  LEFT OUTER JOIN [USERMAN].[dbo].[Company] AS comp ON [rl].[SysOwnedBy] = [comp].[CompanyID]
                  LEFT OUTER JOIN [MLS].[dbo].[Agent] AS ag ON [rl].[AgentID_SELL] = [ag].[AgentID]
				  LEFT OUTER JOIN [MLS].[dbo].[ResidentialListingRequest] as [rlra] on ( select top 1 ResidentialListingRequestid from mls.dbo.ResidentialListingRequest where sysstatusid =3 and mlsnum=rl.mlsnum order by ResidentialListingRequestid desc)=[rlra].ResidentialListingRequestid
                  LEFT OUTER JOIN [MLS].[dbo].[Broker] AS b on comp.brokerid = b.brokerid
                  LEFT OUTER JOIN [MLS].[dbo].[agent] AS a on b.agentid = a.agentid    
                  LEFT OUTER JOIN [MLS].[dbo].[office] AS o on b.officeid = o.officeid       
                 
                  WHERE ([rl].[ResidentialListingRequestID] IS NOT NULL) AND {0}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                  ORDER BY [rl].[ResidentialListingRequestID] ASC", whereClause);

                data = await sql.QueryAsync<ResidentialListingRequest>(query);

                if (data != null && data.Count() > 0)
                {
                    dataCTX = await sql.QueryAsync<ResidentialListingRequest>(
                    string.Format(@"
                        SELECT
                                [m].[Name] + ' CTX' AS MarketName,
                                [b].[SiteUsername] AS MarketUsername,
                                [b].[SitePassword] AS MarketPassword,
                                [b].[LicenseNum] AS BrokerLicenseNum,
			                    [a].[FullName] as [BrokerName],
                                [cp].[RealtorContactEmail] as CommunityProfileRealtorContactEmail,
                                [cp].[CommunityName] AS CommunityName, 
                                [cp].[SalesOfficeStreetName] AS CommunityProfileSalesOfficeStreetName,
                                [cp].[SalesOfficeStreetNum] AS CommunityProfileSalesOfficeStreetNum,
                                [cp].[SalesOfficeCity] AS CommunityProfileSalesOfficeCity,
                                [cp].[SalesOfficeZip] AS CommunityProfileSalesOfficeZip ,
                                [cp].[Phone] AS CommunityProfilePhone,
                                [comp].[Name] AS CompanyName, 
                                [comp].[IncludeRemarks] AS IncludeRemarks, 
                                [comp].[AgentListApptPhone] AS AgentListApptPhoneFromCompany, 
                                [comp].[RemarksFormat] AS RemarksFormatFromCompany, 
                                [cp].[AgentListApptPhone] AS AgentListApptPhoneFromCommunityProfile,   
                                [comp].[AlternatePhone] AS AlternatePhoneFromCompany, 
                                [cp].[OtherPhone] AS OtherPhoneFromCommunityProfile,
                                [comp].[ContactEmail] AS ContactEmailFromCompany,
                                [comp].[AutopopulateExpirationDate] as AutopopulateExpirationDate,
                                [comp].[CTXUser] AS CTXUser,
                                [comp].[CTXPass] AS CTXPass,
                                [cp].[RealtorContactEmail] AS RealtorContactEmailFromCommunityProfile,
                                [pp].[PlanName] AS PlanProfileName,    
                                [ag].[UID] AS SellingAgentUID,
                                [ag].[LicenseNum] AS SellingAgentLicenseNum,
                                [ag].[LastName] AS SellingAgentLastName,
                                [ag].[FirstName] AS SellingAgentFristName,
                                [ag].[UIDOFFICE] AS SellingAgentUIDOFFICE,
                                [rlra].[liststatus] as [OldListStatus],
			                    [o].[UID] as [BrokerOffice],
                               [rl].*,
                               (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                                  JOIN [METABASE].[dbo].[Market] mPD ON [rl].[MarketID] = [mPD].[Code]
                                  JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'ListStatus'                                
                                  JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                                WHERE  [lmd].[Code] = [rl].[ListStatus] AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                               ) as [ListStatusName],
                               (SELECT COUNT(ss.[ServiceSubscriptionID]) FROM [SERVICES].[dbo].[ServiceSubscription] ss WHERE ss.[ServiceID] = 5 AND ss.[SysState] != 'D' AND ss.[CompanyID] = comp.[CompanyID]) AS ServiceSubscription
                        FROM [MLS].[dbo].[ResidentialListingRequest] AS rl 
                        LEFT OUTER JOIN [METABASE].[dbo].[Market] AS m ON [rl].[MarketID] = [m].[Code]
                        LEFT OUTER JOIN [MLS].[dbo].[CommunityProfile] AS cp ON [rl].[CommunityProfileID] = [cp].[CommunityProfileID]
                        LEFT OUTER JOIN [MLS].[dbo].[PlanProfile] AS pp ON [rl].[PlanProfileID] = [pp].[PlanProfileID]
                        LEFT OUTER JOIN [USERMAN].[dbo].[Company] AS comp ON [rl].[SysOwnedBy] = [comp].[CompanyID]
                        LEFT OUTER JOIN [MLS].[dbo].[Agent] AS ag ON [rl].[AgentID_SELL] = [ag].[AgentID]
	                    LEFT OUTER JOIN [MLS].[dbo].[ResidentialListingRequest] as [rlra] on ( select top 1 ResidentialListingRequestid from mls.dbo.ResidentialListingRequest where isCTX = '1' AND sysstatusid =3 and mlsnum=rl.mlsnum order by ResidentialListingRequestid desc)=[rlra].ResidentialListingRequestid 
                        LEFT OUTER JOIN [MLS].[dbo].[Broker] AS b on comp.brokerid = b.brokerid
                        LEFT OUTER JOIN [MLS].[dbo].[agent] AS a on b.agentid = a.agentid    
                        LEFT OUTER JOIN [MLS].[dbo].[office] AS o on b.officeid = o.officeid                   
                        WHERE [rl].[isCTX] = '1' AND {0}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                        ORDER BY [rl].[ResidentialListingRequestID] ASC", whereClause));

                    sql.Close();

                    if (dataCTX != null && dataCTX.Count() > 0)
                    {
                        foreach (var record in dataCTX)
                        {
                            record.MLSNum = record.CTXMLSNum;
                        }

                        return data.Concat(dataCTX).OrderBy(x => x.ResidentialListingRequestID).Distinct();
                    }
                }
                sql.Close();
            }
            catch (Exception ex)
            {
                dbConnectionState = DatabaseState.Failed;
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error while getting ResidentialListingRequest data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Error(ex, "Failed to load ResidentialListingRequests with {ConnectionString}", _connectionString);
                throw;
            }

            return data;
        }

        private async Task<IEnumerable<ResidentialListingRequest>> GetDataInternalLease(string whereClause)
        {
            IEnumerable<ResidentialListingRequest> data;

            if (dbConnectionState == DatabaseState.Failed)
                throw new Exception();

            try
            {
                var sql = new SqlConnection(_connectionString);

                sql.Open();
                data = await sql.QueryAsync<ResidentialListingRequest>(

                string.Format(@"
                    SELECT
                         [m].[Name] AS MarketName,
                         [b].[SiteUsername] AS MarketUsername,
                         [b].[SitePassword] AS MarketPassword,
                         [b].[LicenseNum] AS BrokerLicenseNum,
						 [a].[FullName] as [BrokerName],
                         [cp].[RealtorContactEmail] as CommunityProfileRealtorContactEmail,
                         [cp].[CommunityName] AS CommunityName, 
                         [cp].[SalesOfficeStreetName] AS CommunityProfileSalesOfficeStreetName,
                         [cp].[SalesOfficeStreetNum] AS CommunityProfileSalesOfficeStreetNum,
                         [cp].[SalesOfficeCity] AS CommunityProfileSalesOfficeCity,
                         [cp].[SalesOfficeZip] AS CommunityProfileSalesOfficeZip ,
                         [cp].[Phone] AS CommunityProfilePhone,
                         [comp].[Name] AS CompanyName, 
                         [comp].[IncludeRemarks] AS IncludeRemarks, 
                         [comp].[AgentListApptPhone] AS AgentListApptPhoneFromCompany, 
                         [comp].[RemarksFormat] AS RemarksFormatFromCompany, 
                         [cp].[AgentListApptPhone] AS AgentListApptPhoneFromCommunityProfile,   
                         [comp].[AlternatePhone] AS AlternatePhoneFromCompany, 
                         [cp].[OtherPhone] AS OtherPhoneFromCommunityProfile,
                         [comp].[ContactEmail] AS ContactEmailFromCompany,
                         [comp].[AutopopulateExpirationDate] as AutopopulateExpirationDate,
                         [cp].[RealtorContactEmail] AS RealtorContactEmailFromCommunityProfile,
                         [pp].[PlanName] AS PlanProfileName,    
                         [ag].[UID] AS SellingAgentUID,
                         [ag].[LicenseNum] AS SellingAgentLicenseNum,
                         [ag].[LastName] AS SellingAgentLastName,
                         [ag].[FirstName] AS SellingAgentFristName,
                         [ag].[UIDOFFICE] AS SellingAgentUIDOFFICE,
                         [rlra].[liststatus] as [OldListStatus],
						 [o].[UID] as [BrokerOffice],
                         [rl].*,
                         (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                            JOIN [METABASE].[dbo].[Market] mPD ON [rl].[MarketID] = [mPD].[Code]
                            JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'ListStatus'                                
                            JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                          WHERE  [lmd].[Code] = [rl].[ListStatus] AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                         ) as [ListStatusName],
                         (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                            JOIN [METABASE].[dbo].[Market] mPD ON [rl].[MarketID] = [mPD].[Code]
                            JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'GolfCourseName'                                
                            JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                          WHERE  [lmd].[Code] = [rl].[GolfCourseName]  AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                         ) as [GolfCourseFullName],
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeSun ELSE cp.OHStartTimeSun END AS OHStartTimeSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeMon ELSE cp.OHStartTimeMon END AS OHStartTimeMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeTue ELSE cp.OHStartTimeTue END AS OHStartTimeTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeWed ELSE cp.OHStartTimeWed END AS OHStartTimeWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeThu ELSE cp.OHStartTimeThu END AS OHStartTimeThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeFri ELSE cp.OHStartTimeFri END AS OHStartTimeFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHStartTimeSat ELSE cp.OHStartTimeSat END AS OHStartTimeSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeSun ELSE cp.OHEndTimeSun END AS OHEndTimeSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeMon ELSE cp.OHEndTimeMon END AS OHEndTimeMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeTue ELSE cp.OHEndTimeTue END AS OHEndTimeTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeWed ELSE cp.OHEndTimeWed END AS OHEndTimeWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeThu ELSE cp.OHEndTimeThu END AS OHEndTimeThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeFri ELSE cp.OHEndTimeFri END AS OHEndTimeFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHEndTimeSat ELSE cp.OHEndTimeSat END AS OHEndTimeSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeSun ELSE cp.OHTypeSun END AS OHTypeSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeMon ELSE cp.OHTypeMon END AS OHTypeMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeTue ELSE cp.OHTypeTue END AS OHTypeTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeWed ELSE cp.OHTypeWed END AS OHTypeWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeThu ELSE cp.OHTypeThu END AS OHTypeThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeFri ELSE cp.OHTypeFri END AS OHTypeFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHTypeSat ELSE cp.OHTypeSat END AS OHTypeSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsSun ELSE cp.OHRefreshmentsSun END AS OHRefreshmentsSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsMon ELSE cp.OHRefreshmentsMon END AS OHRefreshmentsMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsTue ELSE cp.OHRefreshmentsTue END AS OHRefreshmentsTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsWed ELSE cp.OHRefreshmentsWed END AS OHRefreshmentsWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsThu ELSE cp.OHRefreshmentsThu END AS OHRefreshmentsThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsFri ELSE cp.OHRefreshmentsFri END AS OHRefreshmentsFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHRefreshmentsSat ELSE cp.OHRefreshmentsSat END AS OHRefreshmentsSatOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsSun ELSE cp.OHCommentsSun END AS OHCommentsSunOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsMon ELSE cp.OHCommentsMon END AS OHCommentsMonOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsTue ELSE cp.OHCommentsTue END AS OHCommentsTueOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsWed ELSE cp.OHCommentsWed END AS OHCommentsWedOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsThu ELSE cp.OHCommentsThu END AS OHCommentsThuOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsFri ELSE cp.OHCommentsFri END AS OHCommentsFriOH,
                    CASE WHEN rl.[individualOH]='Y' THEN  rl.OHCommentsSat ELSE cp.OHCommentsSat END AS OHCommentsSatOH,
                    (SELECT COUNT(ss.[ServiceSubscriptionID]) FROM [SERVICES].[dbo].[ServiceSubscription] ss WHERE ss.[ServiceID] = 5 AND ss.[SysState] != 'D' AND ss.[CompanyID] = comp.[CompanyID]) AS ServiceSubscription
                  FROM [LEASE].[dbo].[ResidentialLeaseRequest] AS rl 
                  LEFT OUTER JOIN [METABASE].[dbo].[Market] AS m ON [rl].[MarketID] = [m].[Code]
                  LEFT OUTER JOIN [MLS].[dbo].[CommunityProfile] AS cp ON [rl].[CommunityProfileID] = [cp].[CommunityProfileID]
                  LEFT OUTER JOIN [MLS].[dbo].[PlanProfile] AS pp ON [rl].[PlanProfileID] = [pp].[PlanProfileID]
                  LEFT OUTER JOIN [USERMAN].[dbo].[Company] AS comp ON [rl].[SysOwnedBy] = [comp].[CompanyID]
                  LEFT OUTER JOIN [MLS].[dbo].[Agent] AS ag ON [rl].[AgentID_SELL] = [ag].[AgentID]
				  LEFT OUTER JOIN [LEASE].[dbo].[ResidentialLeaseRequest] as [rlra] on ( select top 1 ResidentialLeaseRequestID from LEASE.dbo.ResidentialLeaseRequest where sysstatusid =3 and mlsnum=rl.mlsnum order by ResidentialLeaseRequestID desc)=[rlra].ResidentialLeaseRequestID
                  LEFT OUTER JOIN [MLS].[dbo].[Broker] AS b on comp.brokerid = b.brokerid
                  LEFT OUTER JOIN [MLS].[dbo].[agent] AS a on b.agentid = a.agentid    
                  LEFT OUTER JOIN [MLS].[dbo].[office] AS o on b.officeid = o.officeid       
                 
                  WHERE ([rl].[ResidentialLeaseRequestID] IS NOT NULL) AND {0}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                  ORDER BY [rl].[ResidentialLeaseRequestID] ASC", whereClause));

                sql.Close();
            }
            catch (Exception ex)
            {
                dbConnectionState = DatabaseState.Failed;
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error while getting ResidentialLeaseRequest data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Error(ex, "Failed to load ResidentialLeaseRequest with {ConnectionString}", _connectionString);
                throw;
            }

            return data;
        }

        private async Task<IEnumerable<ResidentialListingRequest>> GetDataInternalLots(string whereClause)
        {
            IEnumerable<ResidentialListingRequest> data;
            try
            {
                var sql = new SqlConnection(_connectionString);

                sql.Open();
                data = await sql.QueryAsync<ResidentialListingRequest>(

                string.Format(@"
                    SELECT
                         [m].[Name] AS MarketName,
                         [b].[SiteUsername] AS MarketUsername,
                         [b].[SitePassword] AS MarketPassword,
                         [b].[LicenseNum] AS BrokerLicenseNum,
						 [a].[FullName] as [BrokerName],
                         [cp].[RealtorContactEmail] as CommunityProfileRealtorContactEmail,
                         [cp].[CommunityName] AS CommunityName, 
                         [cp].[SalesOfficeStreetName] AS CommunityProfileSalesOfficeStreetName,
                         [cp].[SalesOfficeStreetNum] AS CommunityProfileSalesOfficeStreetNum,
                         [cp].[SalesOfficeCity] AS CommunityProfileSalesOfficeCity,
                         [cp].[SalesOfficeZip] AS CommunityProfileSalesOfficeZip ,
                         [cp].[Phone] AS CommunityProfilePhone,
                         [comp].[Name] AS CompanyName, 
                         [comp].[IncludeRemarks] AS IncludeRemarks, 
                         [comp].[AgentListApptPhone] AS AgentListApptPhoneFromCompany, 
                         [comp].[RemarksFormat] AS RemarksFormatFromCompany, 
                         [cp].[AgentListApptPhone] AS AgentListApptPhoneFromCommunityProfile,   
                         [comp].[AlternatePhone] AS AlternatePhoneFromCompany, 
                         [cp].[OtherPhone] AS OtherPhoneFromCommunityProfile,
                         [comp].[ContactEmail] AS ContactEmailFromCompany,
                         [comp].[AutopopulateExpirationDate] as AutopopulateExpirationDate,
                         [cp].[RealtorContactEmail] AS RealtorContactEmailFromCommunityProfile,
                         [ag].[UID] AS SellingAgentUID,
                         [ag].[LicenseNum] AS SellingAgentLicenseNum,
                         [ag].[LastName] AS SellingAgentLastName,
                         [ag].[FirstName] AS SellingAgentFristName,
                         [ag].[UIDOFFICE] AS SellingAgentUIDOFFICE,
                         [lra].[liststatus] as [OldListStatus],
						 [o].[UID] as [BrokerOffice],
                         [l].MLSNum as [MLSNumLot],
                         [l].*,
                         (SELECT TOP 1 lmd.LongName FROM [METABASE].[dbo].[LookupMarketDefinition] AS lmd 
                            JOIN [METABASE].[dbo].[Market] mPD ON [l].[MarketID] = [mPD].[Code]
                            JOIN [METABASE].[dbo].[Property] pPD ON [pPD].[SchemaName] = 'ListStatus'                                
                            JOIN [METABASE].[dbo].[PropertyMarketDefinition] pmdPD ON [pPD].[PropertyID] = [pmdPD].[PropertyID] AND [mPD].[MarketID] = [pmdPD].[MarketID]
                          WHERE  [lmd].[Code] = [l].[ListStatus] AND [lmd].[PropertyMarketDefinitionID] = pmdPD.[PropertyMarketDefinitionID]
                         ) as [ListStatusName]
                  FROM [Lots].[dbo].[InternalLotRequest] AS l 
                  LEFT OUTER JOIN [METABASE].[dbo].[Market] AS m ON [l].[MarketID] = [m].[Code]
                  LEFT OUTER JOIN [MLS].[dbo].[CommunityProfile] AS cp ON [l].[CommunityProfileID] = [cp].[CommunityProfileID]
                  LEFT OUTER JOIN [USERMAN].[dbo].[Company] AS comp ON [l].[SysOwnedBy] = [comp].[CompanyID]
                  LEFT OUTER JOIN [MLS].[dbo].[Agent] AS ag ON [l].[AgentID_SELL] = [ag].[AgentID]
				  LEFT OUTER JOIN [Lots].[dbo].[InternalLotRequest] as [lra] on ( SELECT TOP 1 ilr.InternalLotRequestID FROM [Lots].[dbo].[InternalLotRequest] AS ilr WHERE ilr.SysStatusID = 3 AND ilr.MlsNum = l.MlsNum ORDER BY ilr.InternalLotRequestID DESC) = [lra].InternalLotRequestID
                  LEFT OUTER JOIN [MLS].[dbo].[Broker] AS b on comp.brokerid = b.brokerid
                  LEFT OUTER JOIN [MLS].[dbo].[agent] AS a on b.agentid = a.agentid    
                  LEFT OUTER JOIN [MLS].[dbo].[office] AS o on b.officeid = o.officeid       
                 
                  WHERE ([l].InternalLotRequestID IS NOT NULL) AND {0}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
                  ORDER BY [l].[SysCreatedOn] ASC", whereClause));

                sql.Close();
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error while getting ResidentialListingRequest data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Error(ex, "Failed to load ResidentialListingRequests with {ConnectionString}", _connectionString);
                throw;
            }

            return data;
        }

        public IEnumerable<IListingMedia> GetListingMedia(Guid residentialListingRequestId, string marketName)
        {
            IEnumerable<MlsResource> data;
            try
            {
                var sql = new SqlConnection(_connectionString);

                sql.Open();
                //(r.[ResourceID] IS NOT NULL OR r.[VirtualTourAddress] IS NOT NULL) AND 
                data = sql.Query<MlsResource>(string.Format(
                    @"
                    SELECT r.*  
                    FROM [MLS].[dbo].[MlsResource] as r
                      LEFT JOIN  [Mediaman].[dbo].[Resource] as mr on r.[ResourceID] = mr.[ResourceID] 
                    WHERE 
                      r.[SysStatusID] <> 2 AND r.[SysState] <> 'D' AND r.[ResidentialListingRequestID] = {0}
                    ORDER BY r.[Order] ASC, r.[IsPrimaryPic] DESC, r.[MlsResourceID] ASC ", residentialListingRequestId.ToString(CultureInfo.InvariantCulture.ToString())));

                sql.Close();
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load Media for listing " + residentialListingRequestId + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Error(ex, "Failed to load Media for listing {ResidentialListingRequestId} using {ConnectionString}", residentialListingRequestId, _connectionString);
                throw;
            }

            var result = new List<IListingMedia>();

            int count = 0;

            data = ConsolidateImages(data.ToList());
            foreach (var res in data)
            {
                if (res.ResourceID.HasValue)
                {
                    count++;
                    try
                    {
                        var resource = GetResource(res.ResourceID.Value, marketName, res.isRepresentative ?? false);
                        var t = new ResidentialListingMedia
                        {
                            Caption = res.Description,
                            Data = resource,
                            Order = count,
                            IsPrimary = res.IsPrimaryPic ?? false,
                            Id = res.ResourceID.Value.ToString(),
                            Extension = GetExtension(resource)
                        };

                        if (!string.IsNullOrWhiteSpace(t.Extension))
                        {
                            if (t.IsPrimary)
                            {
                                result.Insert(0, t);
                                t.Order = 1;
                            }
                            else
                            {
                                result.Add(t);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load the data for image" + res.ResourceID + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        Log.Error(ex, "Failed to load the data for image {ResourceId}", res.ResourceID);
                    }
                }
                else if (!String.IsNullOrEmpty(res.ExternalUrl))
                {
                    var t = new ResidentialListingMedia
                    {
                        Caption = res.Description != null ? res.Description : "",
                        Data = GetByteArrayFromURL(res.ExternalUrl),
                        Order = count,
                        IsPrimary = res.IsPrimaryPic ?? false,
                        Id = DateTime.Now.Ticks.ToString(),
                        Extension = System.IO.Path.GetExtension(res.ExternalUrl.Split('?')[0]),
                        ExternalUrl = res.ExternalUrl
                    };

                    result.Add(t);
                }
                else
                {
                    try
                    {
                        var t = new ResidentialListingVirtualTour
                        {
                            VirtualTourAddress = res.VirtualTourAddress
                        };

                        result.Add(t);
                    }
                    catch (Exception ex)
                    {
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load the data for image" + res.ResourceID + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        Log.Error(ex, "Failed to load the data for image {ResourceId}", res.ResourceID);
                    }
                }
            }

            PrepareImages(result.OfType<ResidentialListingMedia>(), marketName);

            return result;
        }

        public IEnumerable<IListingMedia> GetLeasingMedia(Guid residentialListingRequestId, string marketName)
        {
            IEnumerable<MlsResource> data;
            try
            {
                var sql = new SqlConnection(_connectionString);

                sql.Open();
                data = sql.Query<MlsResource>(string.Format(
                    @"
                    SELECT * 
                    FROM [LEASE].[dbo].[ResidentialLeaseResource] as r
                      LEFT JOIN  [Mediaman].[dbo].[Resource] as mr on r.[ResourceID] = mr.[ResourceID] 
                    WHERE (r.[ResourceID] IS NOT NULL OR r.[VirtualTourAddress] IS NOT NULL) AND 
                      r.[SysStatusID] <> 2 AND r.[SysState] <> 'D' AND r.[ResidentialLeaseRequestID] = {0}
                    ORDER BY r.[Order] ASC, r.[IsPrimaryPic] DESC ", residentialListingRequestId.ToString(CultureInfo.InvariantCulture.ToString())));
                sql.Close();
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load Media for listing " + residentialListingRequestId + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Error(ex, "Failed to load Media for leaseing {ResidentialListingRequestId} using {ConnectionString}", residentialListingRequestId, _connectionString);
                throw;
            }

            var result = new List<IListingMedia>();

            int count = 0;

            data = ConsolidateImages(data.ToList());
            foreach (var res in data)
            {
                if (res.ResourceID.HasValue)
                {
                    count++;
                    try
                    {
                        var resource = GetResource(res.ResourceID.Value, marketName, res.isRepresentative ?? false);
                        var t = new ResidentialListingMedia
                        {
                            Caption = res.Description,
                            Data = resource,
                            Order = count,
                            IsPrimary = res.IsPrimaryPic ?? false,
                            Id = res.ResourceID.Value.ToString(),
                            Extension = GetExtension(resource)
                        };

                        if (!string.IsNullOrWhiteSpace(t.Extension))
                        {
                            if (t.IsPrimary)
                            {
                                result.Insert(0, t);
                                t.Order = 1;
                            }
                            else
                            {
                                result.Add(t);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load the data for image" + res.ResourceID + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        Log.Error(ex, "Failed to load the data for image {ResourceId}", res.ResourceID);
                    }
                }
                else
                {
                    try
                    {
                        var t = new ResidentialListingVirtualTour
                        {
                            VirtualTourAddress = res.VirtualTourAddress
                        };

                        result.Add(t);
                    }
                    catch (Exception ex)
                    {
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load the data for image" + res.ResourceID + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        Log.Error(ex, "Failed to load the data for image {ResourceId}", res.ResourceID);
                    }
                }
            }

            PrepareImages(result.OfType<ResidentialListingMedia>(), marketName);

            return result;
        }

        private List<DateTime> GetSystemHolidays()
        {
            IEnumerable<DateTime> holidayDates = new List<DateTime>();
            try
            {
                var sql = new SqlConnection(_connectionString);
                sql.Open();
                holidayDates = sql.Query<DateTime>(string.Format(
                    @" SELECT [Date] FROM [MLS].[dbo].[SystemHoliday] WHERE  [SysState] = 'A' ORDER BY [Date] DESC "));
                sql.Close();
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load the holidays data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Error(ex, "Failed to load the holidays data.\n\n");
            }
            return holidayDates.ToList();
        }

        private static List<MlsResource> ConsolidateImages(List<MlsResource> resources)
        {
            List<MlsResource> mlsResToReturn = new List<MlsResource>();
            List<MlsResource> mlsResInconsistent = new List<MlsResource>();

            foreach (var res in resources)
            {
                if (res.Order == null || res.IsPrimaryPic == null)
                    mlsResInconsistent.Add(res);
                else
                    mlsResToReturn.Add(res);
            }
            if (mlsResInconsistent.Count > 0)
                mlsResToReturn.AddRange(mlsResInconsistent);

            return mlsResToReturn;
        }

        private static void PrepareImages(IEnumerable<ResidentialListingMedia> media, string marketName)
        {
            var folder = Path.Combine(Path.GetTempPath(), "Husa.Core.Uploader", Path.GetRandomFileName());

            try
            {
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to create the temporary folders for image data at " + folder + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Error(ex, "Failed to create the temporary folders for image data at {Path}", folder);
                throw;
            }
            foreach (ResidentialListingMedia image in media.OrderBy(x => x.Order))
            {
                var filePath = Path.Combine(folder, image.Id);
                try
                {
                    File.WriteAllBytes(filePath + image.Extension, image.Data);

                    if (image.Extension.Equals(".gif") || image.Extension.Equals(".png"))
                    {
                        convertFilePngToJpg(filePath, image.Extension, 1280, 1024);
                        image.Extension = ".jpg";
                    }
                    else if (marketName == "San Antonio" && image.Extension.Equals(".jpg"))
                    {
                        convertFileJpgToPng(filePath, image.Extension, 1280, 1024);
                        image.Extension = ".png";
                    }

                    // UP-72
                    image.PathOnDisk = filePath + image.Extension;

                    // UP-61
                    if (marketName == "Houston")
                    {
                        ResidentialListingMedia img = changeSize(filePath, image);
                        image.Extension = img.Extension;
                        image.PathOnDisk = img.PathOnDisk;
                    }
                }
                catch (Exception ex)
                {
                    EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to create the on-disk file for the image with " + image.Id + " in this " + filePath + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                    Log.Error(ex, "Failed to create the on-disk file for the image with {ResourceId} in this {Path}", image.Id, filePath);
                    throw;
                }
            }
        }

        private byte[] GetByteArrayFromURL(string url)
        {
            var webClient = new WebClient();
            return webClient.DownloadData(url);
        }

        public static ResidentialListingMedia changeSize(String pathFile, ResidentialListingMedia img)
        {
            String newFileName = pathFile + "_modified_" + DateTime.Now.Ticks.ToString();
            File.Move(pathFile + img.Extension, newFileName + img.Extension);

            Image newImage = Image.FromFile(newFileName + img.Extension);
            int newImageWidth = newImage.Width;
            int newImageHeight = newImage.Height;

            int positionX = ((newImageWidth) / 2);
            int positionY = ((newImageHeight) / 2);

            if ((newImageWidth * newImageHeight) >= 16000000)
            {
                int percentage = (100 - int.Parse((Math.Abs((15000000 * 100) / (newImageWidth * newImageHeight))).ToString())) / 2;
                newImageWidth = newImageWidth - ((newImageWidth * percentage) / 100);
                newImageHeight = newImageHeight - ((newImageHeight * percentage) / 100);

                Size newFileSize = new Size(newImageWidth, newImageHeight);
                // Get a bitmap.

                Bitmap photoFile = new System.Drawing.Bitmap(newImage, newFileSize);
                photoFile.SetResolution(newImage.HorizontalResolution, newImage.VerticalResolution);
                using (var g = Graphics.FromImage(photoFile))
                {
                    g.Clear(Color.White);
                    g.DrawImageUnscaled(newImage, 0, 0);
                }

                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

                // Create an Encoder object based on the GUID
                // for the Quality parameter category.
                System.Drawing.Imaging.Encoder myEncoder =
                    System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object.
                // An EncoderParameters object has an array of EncoderParameter
                // objects. In this case, there is only one
                // EncoderParameter object in the array.
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                // Save the bitmap as a JPG file with 100% quality level.
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                photoFile.Save(@pathFile + ".jpg", jgpEncoder,
                myEncoderParameters);

                img.Extension = ".jpg";
                img.PathOnDisk = @pathFile + ".jpg";

                photoFile.Dispose();
                newImage.Dispose();

                // Delete the original file
                File.Delete(newFileName + img.Extension);
            }
            else
            {
                newImage.Dispose();

                img.PathOnDisk = newFileName + img.Extension;
            }

            return img;
        }

        public static void convertFilePngToJpg(String pathFile, String actualExt, int width, int height)
        {
            String newFileName = pathFile + "_backup_" + DateTime.Now.Ticks.ToString();
            File.Move(pathFile + actualExt, newFileName + actualExt);

            Image newImage = Image.FromFile(newFileName + actualExt);
            int newImageWidth = newImage.Width;
            int newImageHeight = newImage.Height;

            int positionX = ((width - newImageWidth) / 2);
            int positionY = ((height - newImageHeight) / 2);

            Size newFileSize = new Size(newImageWidth, newImageHeight);
            // Get a bitmap.

            Bitmap photoFile = new System.Drawing.Bitmap(newImage, newFileSize);
            photoFile.SetResolution(newImage.HorizontalResolution, newImage.VerticalResolution);
            using (var g = Graphics.FromImage(photoFile))
            {
                g.Clear(Color.White);
                g.DrawImageUnscaled(newImage, 0, 0);
            }

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPG file with 100% quality level.
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            photoFile.Save(@pathFile + ".jpg", jgpEncoder,
                myEncoderParameters);

            photoFile.Dispose();
            newImage.Dispose();

            // Delete the original file
            File.Delete(newFileName + actualExt);
        }

        public static void convertFileJpgToPng(String pathFile, String actualExt, int width, int height)
        {
            String newFileName = pathFile + "_backup_" + DateTime.Now.Ticks.ToString();
            File.Move(pathFile + actualExt, newFileName + actualExt);

            Image newImage = Image.FromFile(newFileName + actualExt);
            int newImageWidth = newImage.Width;
            int newImageHeight = newImage.Height;

            int positionX = ((width - newImageWidth) / 2);
            int positionY = ((height - newImageHeight) / 2);

            Size newFileSize = new Size(newImageWidth, newImageHeight);
            // Get a bitmap.

            Bitmap photoFile = new System.Drawing.Bitmap(newImage, newFileSize);
            photoFile.SetResolution(newImage.HorizontalResolution, newImage.VerticalResolution);
            using (var g = Graphics.FromImage(photoFile))
            {
                g.Clear(Color.White);
                g.DrawImageUnscaled(newImage, 0, 0);
            }

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Png);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a PNG file with 100% quality level.
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            photoFile.Save(@pathFile + ".png", jgpEncoder,
                myEncoderParameters);

            photoFile.Dispose();
            newImage.Dispose();

            // Delete the original file
            File.Delete(newFileName + actualExt);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static byte[] GetResource(Guid resourceId, string marketName, bool isRepresentative = false)
        {
            try
            {
                var id = resourceId.ToString();
                //var digits = Regex.Matches(id, @"\d").Cast<Match>().Select(x => x.Value).Take(2).ToArray();
                //var letters =
                //    Regex.Matches(id, @"[A-Z]", RegexOptions.IgnoreCase)
                //        .Cast<Match>()
                //        .Select(x => x.Value)
                //        .Take(2)
                //        .ToArray();
                //var folder = digits[1] + @"\" + letters[0] + @"\" + digits[0] + @"\" + letters[1] + @"\";

                String resId = id.Replace("-", "");
                String fullPath = "https://resourcesweb.homesusa.com//" + resId + ".rscx";
                if (isRepresentative)
                {
                    fullPath += "?u=7689";
                }
                if (!String.IsNullOrEmpty(marketName) && marketName == "Houston")
                    fullPath += ((fullPath.Contains("?")) ? "&is=9999" : "?is=9999");
                else if (!String.IsNullOrEmpty(marketName) && marketName == "San Antonio")
                    fullPath += ((fullPath.Contains("?")) ? "&is=1914" : "?is=1914");

                var webClient = new WebClient();
                //byte[] imageBytes = webClient.DownloadData(fullPath);
                Stream stream = webClient.OpenRead(fullPath);
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }

                //return imageBytes;
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to load the data for image" + resourceId + ". Error while getting Media data.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Fatal(ex, "");
            }
            return null;
        }

        private static string GetExtension(byte[] data)
        {
            if (IsPng(data))
                return ".png";

            if (IsJpg(data))
                return ".jpg";

            if (IsGif(data))
                return ".gif";

            return null;
        }

        private static bool IsPng(byte[] data)
        {
            return data != null && data.Length > 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E &&
                   data[3] == 0x47 && data[4] == 0x0D && data[5] == 0x0A && data[6] == 0x1A && data[7] == 0x0A;
        }
        private static bool IsJpg(byte[] data)
        {
            return data != null && data.Length > 4 && data[0] == 0xff && data[1] == 0xd8 && data[data.Length - 2] == 0xff && data[data.Length - 1] == 0xd9;
        }
        private static bool IsGif(byte[] data)
        {
            return data != null && data.Length > 6 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38 && (data[4] == 0x39 || data[4] == 0x37) && data[5] == 0x61;
        }

        private class MlsResource
        {
            public Guid? ResourceID { get; set; }
            public string Description { get; set; }
            public int? Order { get; set; }
            public bool? IsPrimaryPic { get; set; }
            public string VirtualTourAddress { get; set; }
            public bool? isRepresentative { get; set; }
            public string ExternalUrl { get; set; }
        }
    }

    public enum DatabaseState
    {
        Online,
        Failed
    }
}
