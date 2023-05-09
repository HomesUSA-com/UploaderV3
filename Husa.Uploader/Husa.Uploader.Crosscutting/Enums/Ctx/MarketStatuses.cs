namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarketStatuses
    {
        Active,
        ActiveUnderContract,
        Canceled,
        Closed,
        ComingSoon,
        CompSold,
        Contingent,
        Delete,
        Expired,
        Hold,
        Incomplete,
        Leased,
        Pending,
        Terminated,
        Withdrawn,
    }
}
