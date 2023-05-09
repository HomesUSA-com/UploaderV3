namespace Husa.Uploader.Crosscutting.Enums.Sabor
{
    using System.Runtime.Serialization;

    public enum OpenHouseTypeEnum
    {
        [EnumMember(Value = "Monday")]
        Monday,
        [EnumMember(Value = "Tuesday")]
        Tuesday,
        [EnumMember(Value = "Wednesday")]
        Wednesday,
        [EnumMember(Value = "Thursday")]
        Thursday,
        [EnumMember(Value = "Friday")]
        Friday,
        [EnumMember(Value = "Saturday")]
        Saturday,
        [EnumMember(Value = "Sunday")]
        Sunday,
    }
}
