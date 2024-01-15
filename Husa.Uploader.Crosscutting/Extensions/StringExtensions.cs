namespace Husa.Uploader.Crosscutting.Extensions
{
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

        public static string DecimalToString(this decimal? amount) => amount.HasValue ? ((int)amount.Value).ToString() : string.Empty;
        public static string StrictDecimalToString(this decimal? amount) => amount.HasValue ? amount.Value.ToString() : string.Empty;

        public static string IntegerToString(this int? amount) => amount.HasValue ? amount.Value.ToString() : string.Empty;
        public static string PriceWithDollarSign(this string price) => price is not null ? $"${price}" : price;

        public static string AmountByType(this string amount, string amountType)
        {
            if (!string.IsNullOrWhiteSpace(amountType) && decimal.TryParse(amount, out decimal agentBonusAmount))
            {
                return amountType == "$" ? string.Format("${0:n2}", agentBonusAmount) : string.Format("{0}%", agentBonusAmount);
            }

            return string.Empty;
        }

        public static string FormatPhoneNumber(this string phoneNumber)
        {
            var cleanNumber = new string(phoneNumber.Where(char.IsDigit).ToArray());

            return $"{cleanNumber.Substring(0, 3)}-{cleanNumber.Substring(3, 3)}-{cleanNumber.Substring(6)}";
        }
    }
}
