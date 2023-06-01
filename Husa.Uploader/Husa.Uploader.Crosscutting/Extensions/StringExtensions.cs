namespace Husa.Uploader.Crosscutting.Extensions
{
    using Husa.Quicklister.CTX.Domain.Enums.Entities;

    public static class StringExtensions
    {
        public static string RemoveSlash(this string stringWithSlashes)
        {
            if (string.IsNullOrWhiteSpace(stringWithSlashes))
            {
                return stringWithSlashes;
            }

            if (stringWithSlashes.Contains('/'))
            {
                stringWithSlashes = stringWithSlashes.Replace('/', newChar: '-');
            }
            else if (stringWithSlashes.Contains('\\'))
            {
                stringWithSlashes = stringWithSlashes.Replace('\\', newChar: '-');
            }
            else if (stringWithSlashes.Contains('"'))
            {
                stringWithSlashes = stringWithSlashes.Replace("\"", newValue: string.Empty);
            }

            return stringWithSlashes;
        }

        public static string ToNullableBonusType(this CommissionType? commissionType)
        {
            if (!commissionType.HasValue)
            {
                return string.Empty;
            }

            return commissionType.Value.ToBonusType();
        }

        public static string ToBonusType(this CommissionType commissionType)
        {
            return commissionType switch
            {
                CommissionType.Amount => "Dollars",
                CommissionType.Percent => "Pct",
                _ => string.Empty,
            };
        }

        public static string DecimalToString(this decimal? amount) => amount.HasValue ? ((int)amount.Value).ToString() : string.Empty;
        public static string StrictDecimalToString(this decimal? amount) => amount.HasValue ? amount.Value.ToString() : string.Empty;

        public static string IntegerToString(this int? amount) => amount.HasValue ? amount.Value.ToString() : string.Empty;
    }
}
