namespace Husa.Cargador.ViewModels.Enum
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomType
    {
        Entry,
        Living,
        Family,
        Other,
        Game,
        Media,
        Dining,
        Breakfast,
        Kitchen,
        Study,
        MasterBedroom,
        MasterBedroomCloset,
        Bed,
        MasterBath,
        Utility,
        FullBath,
        Office,
        Studen,
        HalfBath,
    }
}
