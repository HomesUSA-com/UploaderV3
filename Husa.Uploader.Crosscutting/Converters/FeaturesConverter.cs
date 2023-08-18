namespace Husa.Uploader.Crosscutting.Converters
{
    using Husa.Quicklister.CTX.Domain.Enums.Entities;

    public static class FeaturesConverter
    {
        public static string ToStringFromCarport(this CarportCapacity carport) => carport switch
        {
            CarportCapacity.One => "1",
            CarportCapacity.Two => "2",
            CarportCapacity.Three => "3PLUS",
            CarportCapacity.None => "NONE",
            _ => throw new ArgumentException(nameof(carport)),
        };
    }
}
