namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ListType
    {
        [EnumMember(Value = "RE")]
        [Description("Residential")]
        Residential,
        [EnumMember(Value = "LOT")]
        [Description("Lot")]
        Lot,
        [EnumMember(Value = "LEASE")]
        [Description("Lease")]
        Lease,
    }
}
