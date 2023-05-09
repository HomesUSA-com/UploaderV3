namespace Husa.Uploader.Crosscutting.Enums.Ctx
{
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FireplaceDescription
    {
        [EnumMember(Value = "BATHROOM")]
        [Description("Bathroom")]
        Bathroom,
        [EnumMember(Value = "BEDRO")]
        [Description("Bedroom")]
        Bedroom,
        [EnumMember(Value = "CIRCULATING")]
        [Description("Circulating")]
        Circulating,
        [EnumMember(Value = "DECORATIVE")]
        [Description("Decorative")]
        Decorative,
        [EnumMember(Value = "DEN")]
        [Description("Den")]
        Den,
        [EnumMember(Value = "DININGROOM")]
        [Description("Dining Room")]
        DiningRoom,
        [EnumMember(Value = "DOUBLESIDED")]
        [Description("Double Sided")]
        DoubleSided,
        [EnumMember(Value = "ELECT")]
        [Description("Electric")]
        Electric,
        [EnumMember(Value = "FAMILYROOM")]
        [Description("Family Room")]
        FamilyRoom,
        [EnumMember(Value = "GAMER")]
        [Description("Game Room")]
        GameRoom,
        [EnumMember(Value = "GAS")]
        [Description("Gas")]
        Gas,
        [EnumMember(Value = "GASLO")]
        [Description("Gas Logs")]
        GasLogs,
        [EnumMember(Value = "GASST")]
        [Description("Gas Starter")]
        GasStarter,
        [EnumMember(Value = "GLASS")]
        [Description("Glass/Enclosed Screen")]
        GlassEnclosedScreen,
        [EnumMember(Value = "GREAT")]
        [Description("Great/Family Room")]
        GreatFamilyRoom,
        [EnumMember(Value = "HEATI")]
        [Description("Heatilator")]
        Heatilator,
        [EnumMember(Value = "INSERT")]
        [Description("Insert")]
        Insert,
        [EnumMember(Value = "LIVIN")]
        [Description("Living Room")]
        LivingRoom,
        [EnumMember(Value = "MASONRY")]
        [Description("Masonry")]
        Masonry,
        [EnumMember(Value = "MASTER")]
        [Description("Master")]
        Master,
        [EnumMember(Value = "METAL")]
        [Description("Metal")]
        Metal,
        [EnumMember(Value = "OUTSIDE")]
        [Description("Outside")]
        Outside,
        [EnumMember(Value = "SEETHROUGH")]
        [Description("See Through")]
        SeeThrough,
        [EnumMember(Value = "STONE")]
        [Description("Stone/Rock/Brick")]
        StoneRockBrick,
        [EnumMember(Value = "VENTLESS")]
        [Description("Ventless")]
        Ventless,
        [EnumMember(Value = "WOODB")]
        [Description("Wood Burning")]
        WoodBurning,
        [EnumMember(Value = "NONE")]
        [Description("None")]
        None,
    }
}
