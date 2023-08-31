namespace Husa.Uploader.Crosscutting.Extensions.Abor
{
    public static class StringExtensions
    {
        public static string ToCommissionType(this string commissionType)
        {
            return commissionType switch
            {
                "$" => "Dollar",
                "%" => "Percent",
                _ => string.Empty,
            };
        }
    }
}
