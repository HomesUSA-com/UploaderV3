namespace Husa.Uploader.Crosscutting.Extensions
{
    public static class OpenHouseExtensions
    {
        public static string GetComments(bool refreshments, bool lunch)
        {
            var comments = refreshments ? "refreshments" : string.Empty;

            if (lunch)
            {
                comments = string.Join(", ", comments, "lunch");
            }

            return comments;
        }

        public static string GetNextWeekday(DateTime startDate, DayOfWeek weekDay)
        {
            var daysToAdd = ((int)weekDay - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(daysToAdd).ToString("MM/dd/yyyy");
        }

        public static string GetOpenHouseHours(string openHouseTimes)
        {
            if (openHouseTimes is null)
            {
                throw new ArgumentNullException(nameof(openHouseTimes));
            }

            if (!openHouseTimes.Any())
            {
                return "0";
            }

            var firstOpenHouseTime = openHouseTimes.Split(':').FirstOrDefault();
            return firstOpenHouseTime != null && !firstOpenHouseTime.Equals("12", StringComparison.InvariantCultureIgnoreCase) ?
                firstOpenHouseTime :
                "0";
        }
    }
}
