namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum WaterDescription
    {
        [EnumMember(Value = "BLTLK")]
        [Description("Belton Lake")]
        BeltonLake,
        [EnumMember(Value = "BLANC")]
        [Description("Blanco River")]
        BlancoRiver,
        [EnumMember(Value = "BLUFF")]
        [Description("Bluff")]
        Bluff,
        [EnumMember(Value = "BRAZ")]
        [Description("Brazos River")]
        BrazosRiver,
        [EnumMember(Value = "CANAL")]
        [Description("Canal")]
        Canal,
        [EnumMember(Value = "CANYO")]
        [Description("Canyon Lake/U.S. Corp of Engineers")]
        CanyonLakeUSCorpofEngineers,
        [EnumMember(Value = "CITYS")]
        [Description("City Skyline View")]
        CitySkylineView,
        [EnumMember(Value = "COLOR")]
        [Description("Colorado River")]
        ColoradoRiver,
        [EnumMember(Value = "COMAL")]
        [Description("Comal River")]
        ComalRiver,
        [EnumMember(Value = "COUNT")]
        [Description("Countryside View")]
        CountrysideView,
        [EnumMember(Value = "CREKS")]
        [Description("Creek-Seasonal")]
        CreekSeasonal,
        [EnumMember(Value = "CREK")]
        [Description("Creek/Stream")]
        CreekStream,
        [EnumMember(Value = "FIELDS")]
        [Description("Fields")]
        Fields,
        [EnumMember(Value = "GOLFCOURSE")]
        [Description("Golf Course")]
        GolfCourse,
        [EnumMember(Value = "GREENBELT")]
        [Description("Greenbelt")]
        Greenbelt,
        [EnumMember(Value = "GUADA")]
        [Description("Guadalupe River")]
        GuadalupeRiver,
        [EnumMember(Value = "HILLC")]
        [Description("Hill Country View")]
        HillCountryView,
        [EnumMember(Value = "LAKE")]
        [Description("Lake")]
        Lake,
        [EnumMember(Value = "LKAUS")]
        [Description("Lake Austin")]
        LakeAustin,
        [EnumMember(Value = "BAST")]
        [Description("Lake Bastrop")]
        LakeBastrop,
        [EnumMember(Value = "BUCKN")]
        [Description("Lake Buchanan")]
        LakeBuchanan,
        [EnumMember(Value = "LAKED")]
        [Description("Lake Dunlap")]
        LakeDunlap,
        [EnumMember(Value = "LAKEL")]
        [Description("Lake LBJ")]
        LakeLBJ,
        [EnumMember(Value = "LAKEM")]
        [Description("Lake McQueeney")]
        LakeMcQueeney,
        [EnumMember(Value = "LAKEP")]
        [Description("Lake Placid")]
        LakePlacid,
        [EnumMember(Value = "LAKES")]
        [Description("Lake Seguin")]
        LakeSeguin,
        [EnumMember(Value = "TRAV")]
        [Description("Lake Travis")]
        LakeTravis,
        [EnumMember(Value = "MEADO")]
        [Description("Meadow Lake")]
        MeadowLake,
        [EnumMember(Value = "NONEWTRFTUR")]
        [Description("None Water Features")]
        NoneWaterFeatures,
        [EnumMember(Value = "PANORAMIC")]
        [Description("Panoramic")]
        Panoramic,
        [EnumMember(Value = "PEDER")]
        [Description("Pedernales River")]
        PedernalesRiver,
        [EnumMember(Value = "PONDS")]
        [Description("Pond/Stock Tank")]
        PondStockTank,
        [EnumMember(Value = "RIVER")]
        [Description("River")]
        River,
        [EnumMember(Value = "SANMA")]
        [Description("San Marcos River")]
        SanMarcosRiver,
        [EnumMember(Value = "SEEAGENT")]
        [Description("See Agent")]
        SeeAgent,
        [EnumMember(Value = "STHLK")]
        [Description("Stillhouse Hollow Lake")]
        StillhouseHollowLake,
        [EnumMember(Value = "WTRAC")]
        [Description("Water Access")]
        WaterAccess,
        [EnumMember(Value = "WTRVI")]
        [Description("Water View")]
        WaterView,
        [EnumMember(Value = "WTRFR")]
        [Description("Waterfront")]
        Waterfront,
        [EnumMember(Value = "WOODS")]
        [Description("Woods")]
        Woods,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
        [EnumMember(Value = "OTHSE")]
        [Description("Other-See Remarks")]
        OtherSeeRemarks,
    }
}
