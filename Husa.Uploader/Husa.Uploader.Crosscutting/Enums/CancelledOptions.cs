namespace Husa.Uploader.Crosscutting.Enums
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CancelledOptions
    {
        [EnumMember(Value = "BALOT")]
        Building_Another_Lot,
        [EnumMember(Value = "NBOTL")]
        Not_Building_ThisLot,
        [EnumMember(Value = "OTHER")]
        Other,
        [EnumMember(Value = "REPAIRS")]
        Repairs,
        [EnumMember(Value = "CPLAN")]
        Changing_Plans,
        [EnumMember(Value = "RDOM")]
        Reset_DOM,
        [EnumMember(Value = "STRAGIC")]
        Something_tragic,
        [EnumMember(Value = "LWREALTOR")]
        Listing_with_Realtor,
    }
}
