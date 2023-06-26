namespace Husa.Uploader.Crosscutting.Extensions
{
    using System;

    public static class TimeSpanExtensions
    {
        public static string To12Format(this TimeSpan value)
        {
            var hours = value.Hours == 12 ? 0 : value.Hours;
            var minutes = value.Minutes < 10 ? $"0{value.Minutes}" : value.Minutes.ToString();

            if (hours > 12)
            {
                hours -= 12;
            }

            return string.Format("{0}:{1}", hours, minutes);
        }
    }
}
