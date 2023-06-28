namespace Husa.Uploader.Crosscutting.Converters
{
    using Husa.Quicklister.CTX.Domain.Enums.Entities;

    public static class HoaConverter
    {
        public static string ToStringFromHasMultipleHOA(this int numHoas) => numHoas > 1 ? "YES" : "NO";

        public static string ToStringFromHOARequirementCTX(this HOARequirement requirement) => requirement switch
        {
            HOARequirement.Mandatory => "MAN",
            HOARequirement.Voluntary => "VOL",
            HOARequirement.None => "NONE",
            _ => throw new ArgumentException(nameof(HOARequirement)),
        };
    }
}
