using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenHouseBase;
using Husa.Core.UploaderBase;
using System.Threading;
using TypeOpenHouseHour = OpenHouseBase.TypeOpenHouseHour;

namespace OpenHouseServiceSA
{
    public class OpenHouseSA
    {
        OpenHouseBase.OpenHouseBase OH = new OpenHouseBase.OpenHouseBase();
        CoreWebDriver driver = new OpenHouseBase.OpenHouseBase().getChromeDriver();
        private System.Diagnostics.EventLog eventLog1 = new System.Diagnostics.EventLog();
        int maxDays = 5;
        List<DateTime> dateToUpdate = new List<DateTime>();

        public OpenHouseSA()
        {
            OH.CheckLog();
            eventLog1.Source = "San Antonio";
            eventLog1.Log = "HomesUSA Open House Service";

            dateToUpdate = OH.getDatesToUpdate(maxDays);
            dateToUpdate.RemoveAt(0);
        }
        
        public void OpenHouse()
        {
            eventLog1.WriteEntry("San Antonio Open House has started", System.Diagnostics.EventLogEntryType.Information, 2);
            IEnumerable<BrokerData> brokers = OH.GetBrokers((int)markets.SanAntonio);
            foreach (BrokerData broker in brokers)
            {
                IEnumerable<OpenHouseData> listings = OH.GetOpenHouseData((int)markets.SanAntonio, broker.BrokerID);
                if (listings.Count() == 0)
                    continue;

                eventLog1.WriteEntry("Running BrokerID: " + broker.BrokerID.ToString() + " \n\nMLS numbers to be updated  : \n\n" + string.Join("\n", listings.Select(x => x.MLSNum)), System.Diagnostics.EventLogEntryType.Information);
                try
                {
                    Login(driver, broker.SiteUsername, broker.SitePassword, broker.SiteLoginUrl);
                }
                catch (Exception e)
                {
                    eventLog1.WriteEntry("Error login San Antonio website: " + e.Message + "\nStack Trace: " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                    continue;
                }
                foreach (OpenHouseData listing in listings)
                {
                    if (listing.ListStatus == "PND" && listing.AllowPendingList != "Y")
                        continue;

                    try
                    {
                        if (listing.EnableOpenHouse && listing.AgreeOpenHouseConditions)
                        {
                            eventLog1.WriteEntry("San Antonio MLS Number: " + listing.MLSNum, System.Diagnostics.EventLogEntryType.Information);
                            StartUpdate(driver, listing);
                            DeleteOpenHouses(driver, listing);
                            AddOpenHouses(driver, listing);
                            eventLog1.WriteEntry("Updated Open house data for San Antonio MLS Number: " + listing.MLSNum, System.Diagnostics.EventLogEntryType.Information);
                        }
                    }
                    catch (Exception e)
                    {
                        eventLog1.WriteEntry("Error BrokerID: " + broker.BrokerID.ToString() + " \nMLS number: " + listing.MLSNum + "\nError: " + e.Message + "\nStack Trace: " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error, 1);
                        // last attempt
                        try
                        {
                            Logout(driver);
                            Thread.Sleep(2000);
                            if (listing.EnableOpenHouse && listing.AgreeOpenHouseConditions)
                            {
                                Login(driver, broker.SiteUsername, broker.SitePassword, broker.SiteLoginUrl);
                                StartUpdate(driver, listing);
                                DeleteOpenHouses(driver, listing);
                                AddOpenHouses(driver, listing);
                            }
                        }
                        catch (Exception w)
                        {
                            eventLog1.WriteEntry("Error BrokerID: " + broker.BrokerID.ToString() + " \nMLS number: " + listing.MLSNum + "\nError: " + e.Message + "\nStack Trace: " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error, 1);
                        }

                    }
                }

                Logout(driver);
            }
            eventLog1.WriteEntry("San Antonio Open House has ended", System.Diagnostics.EventLogEntryType.Information, 2);
        }

        public void Login(CoreWebDriver driver, string SiteUsername, string SitePassword, string SiteLoginUrl)
        {
            driver.Navigate().GoToUrl("http://sabor.connectmls.com/slogin.jsp");
            driver.wait.Until(x => driver.FindElement(By.Name("go")).Displayed);
            // Fill the login and password
            driver.WriteTextbox(By.Id("j_username"), SiteUsername);
            driver.WriteTextbox(By.Id("j_password"), SitePassword);
            DoClick(driver, driver.FindElement(By.Name("go")));
            try
            {
                driver.FindElement(By.Name("remindLater")).Click();
            }
            catch { }
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("listadmin")));
            DoClick(driver, driver.FindElement(By.Id("listadmin")));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("linksPane")));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("main")));
            driver.SwitchTo("main");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("workspace")));
            driver.SwitchTo("workspace");
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Add/Edit a Tour or Open House")));
            DoClick(driver, driver.FindElement(By.LinkText("Add/Edit a Tour or Open House")));
        }

        public void Logout(CoreWebDriver driver)
        {
            Thread.Sleep(1000);
            driver.Navigate("http://sabor.connectmls.com/login.jsp?signout=1");
        }

        private void StartUpdate(CoreWebDriver driver, OpenHouseData listing)
        {
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("FIND")));
            driver.WriteTextbox(By.Id("MLSNO"), listing.MLSNum);
            DoClick(driver, driver.FindElement(By.Id("FIND")));
        }

        public void AddOpenHouses(CoreWebDriver driver, OpenHouseData listing)
        {
            Thread.Sleep(1000);
            // HCS-596
            String openHouseType = "O";

            foreach (var local in dateToUpdate)
            {
                string[] openhousestart;
                string[] openhouseend;

                driver.Click(By.Id("addTourLink"));
                Thread.Sleep(1000);
                var window = driver.WindowHandles.Last();
                driver.SwitchTo().Window(window);
                Thread.Sleep(1000);

                string day = local.DayOfWeek.ToString().Substring(0, 3);

                openhousestart = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.START, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);
                openhouseend = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.END, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);

                // Date
                driver.WriteTextbox(By.Id("dayOfEvent"), local.ToString("MM/dd/yyyy"));

                // Start Time
                driver.SetSelect(By.Id("startTimeHour"), openhousestart[0].Split(':').FirstOrDefault() == "12" ? "0" : openhousestart[0].Split(':').FirstOrDefault(), "", "");
                driver.SetSelect(By.Id("startTimeMin"), openhousestart[0].Split(':').LastOrDefault() == "00" ? "0" : openhousestart[0].Split(':').LastOrDefault(), "", "");
                driver.SetSelect(By.Id("startTimeAmPm"), openhousestart[1], "", "");

                // Stop Time
                driver.SetSelect(By.Id("stopTimeHour"), openhouseend[0].Split(':').FirstOrDefault() == "12" ? "0" : openhouseend[0].Split(':').FirstOrDefault(), "", "");
                driver.SetSelect(By.Id("stopTimeMin"), openhouseend[0].Split(':').LastOrDefault() == "00" ? "0" : openhouseend[0].Split(':').LastOrDefault(), "", "");
                driver.SetSelect(By.Id("stopTimeAmPm"), openhouseend[1], "", "");

                if (listing.GetType().GetProperty("OHLunch" + day).GetValue(listing, null) != null &&
                    listing.GetType().GetProperty("OHRefreshments" + day).GetValue(listing, null) != null)
                {
                    // Type
                    driver.Click(By.Id("type" + openHouseType));

                    // Lunch
                    driver.Click(By.Id("lunch" + listing.GetType().GetProperty("OHLunch" + day).GetValue(listing, null).ToString()));

                    // Refreshments
                    driver.Click(By.Id("refreshments" + listing.GetType().GetProperty("OHRefreshments" + day).GetValue(listing, null).ToString()));
                }
                driver.Click(By.Id(" Save "));
                Thread.Sleep(2000);

                window = driver.WindowHandles.FirstOrDefault();
                driver.SwitchTo().Window(window);
            }

            Thread.Sleep(1000);
            driver.ExecuteScript("saveTours(this);");

            eventLog1.WriteEntry("San Antonio Open House updated successfully for MLS Number: " + listing.MLSNum, System.Diagnostics.EventLogEntryType.Information);
        }

        public void DeleteOpenHouses(CoreWebDriver driver, OpenHouseData Data)
        {
            Thread.Sleep(1000);
            driver.SwitchTo("main");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("workspace")));
            driver.SwitchTo("workspace");
            Thread.Sleep(1000);
            bool loopComplete = false;
            while (!loopComplete)
            {
                if (driver.FindElements(By.Name("dc")) != null)
                {
                    var element = driver.FindElements(By.Name("dc")).FirstOrDefault().FindElements(By.TagName("a")).FirstOrDefault(x => x.GetAttribute("href").Contains("delTour("));

                    if (element != null)
                    {
                        element.Click();
                        Thread.Sleep(1000);
                    }
                    else
                        loopComplete = true;
                }
                else
                    loopComplete = true;
            }
            //driver.ExecuteScript("saveTours(this);");
        }

        private void DoClick(CoreWebDriver driverLocal, IWebElement element)
        {
            // 1. click event
            element.Click();

            // 2. wait until page has fully loaded (Asynchronous events are not controlled)
            driverLocal.wait.Until(x => ((IJavaScriptExecutor)driverLocal).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
