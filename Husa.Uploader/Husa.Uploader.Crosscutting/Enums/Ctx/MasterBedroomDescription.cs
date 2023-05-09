namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MasterBedroomDescription
    {
        [EnumMember(Value = "CEILINGFAN")]
        [Description("Ceiling Fan")]
        CeilingFan,
        [EnumMember(Value = "CLOSETMILTI")]
        [Description("Closets - Multi")]
        ClosetsMulti,
        [EnumMember(Value = "CLOSETWALKIN")]
        [Description("Closets - Walk-In")]
        ClosetsWalkIn,
        [EnumMember(Value = "DUALMS")]
        [Description("Dual Masters")]
        DualMasters,
        [EnumMember(Value = "OUTSIDEAC")]
        [Description("Outside Access")]
        OutsideAccess,
        [EnumMember(Value = "SHOWERSEPT")]
        [Description("Shower - Separate")]
        ShowerSeparate,
        [EnumMember(Value = "SHOWERONLY")]
        [Description("Shower Only")]
        ShowerOnly,
        [EnumMember(Value = "SHOWERTUB")]
        [Description("Shower/Tub Combo")]
        ShowerTubCombo,
        [EnumMember(Value = "SITTINGSTUDY")]
        [Description("Sitting/Study Room")]
        SittingStudyRoom,
        [EnumMember(Value = "SPLIT")]
        [Description("Split")]
        Split,
        [EnumMember(Value = "TUBGARDEN")]
        [Description("Tub - Garden")]
        TubGarden,
        [EnumMember(Value = "TUBJET")]
        [Description("Tub - Jetted")]
        TubJetted,
        [EnumMember(Value = "TUBONLY")]
        [Description("Tub Only")]
        TubOnly,
        [EnumMember(Value = "DBLVANITY")]
        [Description("Vanity - Double")]
        VanityDouble,
        [EnumMember(Value = "SPTVANITY")]
        [Description("Vanity - Separate")]
        VanitySeparate,
        [EnumMember(Value = "SNGLVANITY")]
        [Description("Vanity - Single")]
        VanitySingle,
    }
}
