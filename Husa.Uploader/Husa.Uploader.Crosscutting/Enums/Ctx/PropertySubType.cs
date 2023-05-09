namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PropertySubType
    {
        [EnumMember(Value = "BARNDOMINIUM")]
        [Description("Barndominium")]
        Barndominium,
        [EnumMember(Value = "CNDMI")]
        [Description("Condominium")]
        Condominium,
        [EnumMember(Value = "Garden_Patio_Home")]
        [Description("Garden/Patio Home")]
        GardenPatioHome,
        [EnumMember(Value = "MNFHO")]
        [Description("Manufactured Home")]
        ManufacturedHome,
        [EnumMember(Value = "MH")]
        [Description("Mobile Home")]
        MobileHome,
        [EnumMember(Value = "Modular")]
        [Description("Modular/Prefab")]
        ModularPrefab,
        [EnumMember(Value = "SFM")]
        [Description("Single Family")]
        SFM,
        [EnumMember(Value = "TNX")]
        [Description("Townhouse")]
        Townhouse,
    }
}
