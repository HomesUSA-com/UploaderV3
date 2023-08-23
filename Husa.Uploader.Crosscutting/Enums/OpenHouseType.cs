namespace Husa.Uploader.Crosscutting.Enums
{
    using System.Runtime.Serialization;

    public enum OpenHouseType
    {
        [EnumMember(Value = "Virtual Public")]
        Vir,
        [EnumMember(Value = "Public")]
        Public,
        [EnumMember(Value = "Realtor Only-Caravan")]
        Caravan,
    }
}
