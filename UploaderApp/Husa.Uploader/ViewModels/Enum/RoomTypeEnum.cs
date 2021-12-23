namespace Husa.Cargador.ViewModels.Enum
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomTypeEnum
    {
        [EnumMember(Value = "BEDROO")]
        BEDROO,
        [EnumMember(Value = "BREROO")]
        BREROO,
        [EnumMember(Value = "DINROO")]
        DINROO,
        [EnumMember(Value = "EXEROO")]
        EXEROO,
        [EnumMember(Value = "EXSTRO")]
        EXSTRO,
        [EnumMember(Value = "FULBAT")]
        FULBAT,
        [EnumMember(Value = "GAMROO")]
        GAMROO,
        [EnumMember(Value = "GUESUI")]
        GUESUI,
        [EnumMember(Value = "HALBAT")]
        HALBAT,
        [EnumMember(Value = "KITCHE")]
        KITCHE,
        [EnumMember(Value = "LIBSTU")]
        LIBSTU,
        [EnumMember(Value = "LIVROO")]
        LIVROO,
        [EnumMember(Value = "MASBED")]
        MASBED,
        [EnumMember(Value = "MEDROO")]
        MEDROO,
        [EnumMember(Value = "MUDROO")]
        MUDROO,
        [EnumMember(Value = "MUSROO")]
        MUSROO,
        [EnumMember(Value = "OFFICE")]
        OFFICE,
        [EnumMember(Value = "OTHER")]
        OTHER,
        [EnumMember(Value = "SASTRO")]
        SASTRO,
        [EnumMember(Value = "SECMAS")]
        SECMAS,
        [EnumMember(Value = "SHTR")]
        SHTR,
        [EnumMember(Value = "SOLSUN")]
        SOLSUN,
        [EnumMember(Value = "STUDEN")]
        STUDEN,
        [EnumMember(Value = "UTIROO")]
        UTIROO,
        [EnumMember(Value = "WINCEL")]
        WINCEL,
    }
}
