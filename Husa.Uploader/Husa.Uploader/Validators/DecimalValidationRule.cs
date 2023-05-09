using System.Globalization;
using System.Windows.Controls;

namespace Husa.Uploader.Validators
{
    public class DecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue))
            {
                return new ValidationResult(false, "The value cannot be empty.");
            }

            if (!decimal.TryParse(stringValue, NumberStyles.Any, cultureInfo, out decimal _))
            {
                return new ValidationResult(false, $"The value '{stringValue}' is invalid and must be decimal.");
            }

            return ValidationResult.ValidResult;
        }
    }
}
