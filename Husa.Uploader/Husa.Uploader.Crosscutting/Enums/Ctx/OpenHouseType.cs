namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.Runtime.Serialization;

    public enum OpenHouseType
    {
        [EnumMember(Value = "Monday")]
        Monday = 0,
        [EnumMember(Value = "Tuesday")]
        Tuesday = 1,
        [EnumMember(Value = "Wednesday")]
        Wednesday = 2,
        [EnumMember(Value = "Thursday")]
        Thursday = 3,
        [EnumMember(Value = "Friday")]
        Friday = 4,
        [EnumMember(Value = "Saturday")]
        Saturday = 5,
        [EnumMember(Value = "Sunday")]
        Sunday = 6,
    }
}
