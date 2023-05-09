namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupplierOtherDescription
    {
        [EnumMember(Value = "ABOVEGROUND")]
        [Description("Above Ground")]
        AboveGround,
        [EnumMember(Value = "CABLE")]
        [Description("Cable TV")]
        CableTV,
        [EnumMember(Value = "CITYE")]
        [Description("City Electric")]
        CityElectric,
        [EnumMember(Value = "CITYG")]
        [Description("City Garbage")]
        CityGarbage,
        [EnumMember(Value = "COOPE")]
        [Description("Co-Op Electric")]
        CoOpElectric,
        [EnumMember(Value = "ELECTAVAIL")]
        [Description("Electricity Available")]
        ElectricityAvailable,
        [EnumMember(Value = "FIBEROPTIC")]
        [Description("Fiber Optic")]
        FiberOptic,
        [EnumMember(Value = "GASAV")]
        [Description("Gas Available")]
        GasAvailable,
        [EnumMember(Value = "HIGHS")]
        [Description("High Speed Internet ")]
        HighSpeedInternet,
        [EnumMember(Value = "NATUR")]
        [Description("Natural Gas Available ")]
        NaturalGasAvailable,
        [EnumMember(Value = "ONSEL")]
        [Description("On-Site Electric")]
        OnSiteElectric,
        [EnumMember(Value = "PBTLE")]
        [Description("Propane/Butane Tank-Leased")]
        PropaneButaneTankLeased,
        [EnumMember(Value = "PBTOW")]
        [Description("Propane/Butane Tank-Owned")]
        PropaneButaneTankOwned,
        [EnumMember(Value = "TELEP")]
        [Description("Telephone Available")]
        TelephoneAvailable,
        [EnumMember(Value = "UGUTILITY")]
        [Description("Underground Utilities")]
        UndergroundUtilities,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
