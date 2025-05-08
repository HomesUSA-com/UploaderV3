namespace Husa.Uploader.Crosscutting.Extensions
{
    using Husa.Quicklister.Har.Domain.Enums.Domain;

    public static class EnumExtensions
    {
        public static bool ConvertToBoolean(this HoaRequirement hoaRequirement) => hoaRequirement != HoaRequirement.No;
    }
}
