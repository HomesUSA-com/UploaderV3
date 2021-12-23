using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using Husa.Core.UploaderBase;
using OpenQA.Selenium.Chrome;

namespace Husa.Core.UploaderBase
{
    public class OpenHouseBase
    {
        private System.Diagnostics.EventLog eventLog1;
        public OpenHouseBase()
        {
            //eventLog1 = new System.Diagnostics.EventLog();
            //if (!System.Diagnostics.EventLog.SourceExists("OpenHouseUpdater"))
            //{
            //    System.Diagnostics.EventLog.CreateEventSource("OpenHouseUpdater", "OpenHouseUpdater");
            //}
            //eventLog1.Source = "OpenHouseUpdater";
            //eventLog1.Log = "OpenHouseUpdater";

        }

        public List<DateTime> getNextDate(ResidentialListingRequest listing, int max)
        {
            List<DateTime> date = new List<DateTime>();
            DateTime start = DateTime.Today.AddDays(1);
            DateTime end = start.AddDays(max);
            for (var dt = start; dt < end; dt = dt.AddDays(1))
                date.Add(dt);

            return date;
        }

        public List<DateTime> getDatesToUpdate(int max)
        {
            List<DateTime> date = new List<DateTime>();
            // TODO : Calls method to get the System Holidays
            List<DateTime> holidayDates = new List<DateTime>();
            DateTime start = DateTime.Today;
            DateTime end = start.AddDays(max);

            for (var dt = start; dt < end; dt = dt.AddDays(1))
            {
                if (!holidayDates.Contains(dt))
                    date.Add(dt);
            }
            return date;
        }

        public string[] GetOpenHouseTime(string time, TypeOpenHouseHour type, bool changeOHHours)
        {
            string[] aux = new string[2];

            DateTime dt = new DateTime(2000, 1, 1, Convert.ToInt32(time.Substring(0, 2)), Convert.ToInt32(time.Substring(2, 2)), 0, 0);
            if (changeOHHours)
            {
                if (TypeOpenHouseHour.START.Equals(type))
                {
                    dt.AddMinutes(30);
                }
                else
                {
                    dt.AddMinutes(-30);
                }
            }

            time = dt.ToString("HH:mm"); //dt.TimeOfDay.ToString("hh:mm");

            if (Convert.ToInt32(time.Substring(0, 2)) >= 12)
            {
                if (Convert.ToInt32(time.Substring(0, 2)) == 12)
                {
                    aux[0] = (Convert.ToInt32(time.Substring(0, 2))).ToString().TrimStart('0') + ":" + time.Substring(3, 2);
                    aux[1] = "PM";
                }
                else
                {
                    aux[0] = (Convert.ToInt32(time.Substring(0, 2)) - 12).ToString().TrimStart('0') + ":" + time.Substring(3, 2);
                    aux[1] = "PM";
                }
            }
            else
            {
                aux[0] = time.Substring(0, 2).TrimStart('0') + ":" + time.Substring(3, 2);
                aux[1] = "AM";
            }

            return aux;
        }

        public string[] GetOpenHouseTime(string time)
        {
            string[] aux = new string[2];

            if (Convert.ToInt32(time.Substring(0, 2)) >= 12)
            {
                if (Convert.ToInt32(time.Substring(0, 2)) == 12)
                {
                    aux[0] = (Convert.ToInt32(time.Substring(0, 2))).ToString().TrimStart('0') + ":" + time.Substring(2, 2);
                    aux[1] = "PM";
                }
                else
                {
                    aux[0] = (Convert.ToInt32(time.Substring(0, 2)) - 12).ToString().TrimStart('0') + ":" + time.Substring(2, 2);
                    aux[1] = "PM";
                }
            }
            else
            {
                aux[0] = time.Substring(0, 2).TrimStart('0') + ":" + time.Substring(2, 2);
                aux[1] = "AM";
            }

            return aux;
        }
    }
}
