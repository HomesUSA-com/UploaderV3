namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpecialtyRoomsDescription
    {
        [EnumMember(Value = "ACCES")]
        [Description("Access Only")]
        AccessOnly,
        [EnumMember(Value = "ATTFN")]
        [Description("Attic Fan")]
        AtticFan,
        [EnumMember(Value = "FLOOR")]
        [Description("Floored")]
        Floored,
        [EnumMember(Value = "PRTFL")]
        [Description("Floored-Partially ")]
        FlooredPartially,
        [EnumMember(Value = "PERMA")]
        [Description("Permanent Stairs")]
        PermanentStairs,
        [EnumMember(Value = "PULLD")]
        [Description("Pull Down Stairs")]
        PullDownStairs,
        [EnumMember(Value = "RADIA")]
        [Description("Radiant Barrier")]
        RadiantBarrier,
        [EnumMember(Value = "STORA")]
        [Description("Storage Only")]
        StorageOnly,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
