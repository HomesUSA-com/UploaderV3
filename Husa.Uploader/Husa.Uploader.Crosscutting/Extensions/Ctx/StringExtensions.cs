namespace Husa.Uploader.Crosscutting.Extensions.Ctx
{
    public static class StringExtensions
    {
        public static string ToCommissionType(this string commissionType)
        {
            return commissionType switch
            {
                "$" => "Dollars",
                "%" => "Pct",
                _ => string.Empty,
            };
        }
    }
}
