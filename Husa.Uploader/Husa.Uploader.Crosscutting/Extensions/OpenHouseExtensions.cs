namespace Husa.Uploader.Crosscutting.Extensions
{
    using Husa.Uploader.Crosscutting.Enums;

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

        public static List<DateTime> GetNextDate(int max)
        {
            var date = new List<DateTime>();
            var start = DateTime.Today.AddDays(1);
            var end = start.AddDays(max);
            for (var nextDay = start; nextDay < end; nextDay = nextDay.AddDays(1))
            {
                date.Add(nextDay);
            }

            return date;
        }

        public static string GetNextWeekday(DateTime startDate, DayOfWeek weekDay)
        {
            var daysToAdd = ((int)weekDay - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(daysToAdd).ToString("MM/dd/yyyy");
        }

        public static string[] GetOpenHouseTime(string time, TypeOpenHouseHour type, bool changeOHHours)
        {
            string[] aux = new string[2];
            var hours = Convert.ToInt32(time[..2]);
            var minutes = Convert.ToInt32(time.Substring(2, 2));
            var openHouseDateTime = new DateTime(year: 2000, month: 1, day: 1, hours, minutes, second: 0, millisecond: 0);
            if (changeOHHours)
            {
                if (TypeOpenHouseHour.START.Equals(type))
                {
                    openHouseDateTime.AddMinutes(30);
                }
                else
                {
                    openHouseDateTime.AddMinutes(-30);
                }
            }

            time = openHouseDateTime.ToString("HH:mm");

            if (hours >= 12)
            {
                if (hours == 12)
                {
                    aux[0] = $"{hours.ToString().TrimStart('0')}:{time.Substring(3, 2)}";
                    aux[1] = "PM";
                }
                else
                {
                    aux[0] = $"{(hours - 12).ToString().TrimStart('0')}:{time.Substring(3, 2)}";
                    aux[1] = "PM";
                }
            }
            else
            {
                aux[0] = $"{time[..2].TrimStart('0')}:{time.Substring(3, 2)}";
                aux[1] = "AM";
            }

            return aux;
        }

        public static string[] GetOpenHouseTime(string time)
        {
            string[] aux = new string[2];

            var hours = Convert.ToInt32(time[..2]);
            if (hours >= 12)
            {
                if (hours == 12)
                {
                    aux[0] = $"{hours.ToString().TrimStart('0')}:{time.Substring(2, 2)}";
                    aux[1] = "PM";
                }
                else
                {
                    aux[0] = $"{(hours - 12).ToString().TrimStart('0')}:{time.Substring(2, 2)}";
                    aux[1] = "PM";
                }
            }
            else
            {
                aux[0] = $"{time[..2].TrimStart('0')}:{time.Substring(2, 2)}";
                aux[1] = "AM";
            }

            return aux;
        }

        public static string GetOpenHouseTimeHours(string[] openHouseTimes)
        {
            if (openHouseTimes is null)
            {
                throw new ArgumentNullException(nameof(openHouseTimes));
            }

            if (!openHouseTimes.Any())
            {
                return "0";
            }

            var firstOpenHouseTime = openHouseTimes[0].Split(':').FirstOrDefault();
            return firstOpenHouseTime != null && !firstOpenHouseTime.Equals("12", StringComparison.InvariantCultureIgnoreCase) ?
                firstOpenHouseTime :
                "0";
        }

        public static string GetOpenHouseTimeMinutes(string[] openHouseTimes)
        {
            if (openHouseTimes is null)
            {
                throw new ArgumentNullException(nameof(openHouseTimes));
            }

            if (!openHouseTimes.Any())
            {
                return "0";
            }

            var lastOpenHouseTime = openHouseTimes[0].Split(':').LastOrDefault();
            return lastOpenHouseTime != null && !lastOpenHouseTime.Equals("00", StringComparison.InvariantCultureIgnoreCase) ?
                lastOpenHouseTime :
                "0";
        }
    }
}
