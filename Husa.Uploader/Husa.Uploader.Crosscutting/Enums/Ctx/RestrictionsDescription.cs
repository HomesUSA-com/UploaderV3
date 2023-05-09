namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RestrictionsDescription
    {
        [EnumMember(Value = "ADULT55")]
        [Description("Adult 55+")]
        Adult55,
        [EnumMember(Value = "CITYRESTRIC")]
        [Description("City Restrictions")]
        CityRestrictions,
        [EnumMember(Value = "CONVENANT")]
        [Description("Covenant/Deed")]
        CovenantDeed,
        [EnumMember(Value = "SENIOR")]
        [Description("Senior")]
        Senior,
        [EnumMember(Value = "SUBDIVISION")]
        [Description("Subdivision")]
        Subdivision,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
