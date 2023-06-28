namespace Husa.Uploader.Crosscutting.Extensions
{
    using System.Text.RegularExpressions;

    public static partial class PhoneExtensions
    {
        public static string PhoneFormat(this string strPhone)
        {
            if (string.IsNullOrWhiteSpace(strPhone))
            {
                return strPhone;
            }

            var phone = CleanPhoneNumberRegex().Replace(strPhone, replacement: string.Empty);
            phone = PhoneNumberRegex().Replace(phone, "($1) $2-$3");
            return phone;
        }

        [GeneratedRegex("(\\d{3})(\\d{3})(\\d{4})")]
        private static partial Regex PhoneNumberRegex();

        [GeneratedRegex("[^0-9]")]
        private static partial Regex CleanPhoneNumberRegex();
    }
}
