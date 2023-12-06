namespace Husa.Uploader.Crosscutting.Extensions.Har
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
