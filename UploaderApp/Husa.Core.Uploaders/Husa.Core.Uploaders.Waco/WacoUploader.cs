using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Husa.Core.Uploaders.Waco
{
    public partial class WacoUploader : IUploader,
                                        IEditor,
                                        IPriceUploader,
                                        IStatusUploader,
                                        IImageUploader,
                                        ICompletionDateUploader,
                                        IUpdateOpenHouseUploader,
                                        IUploadVirtualTourUploader
    {
        OpenHouseBase OH = new OpenHouseBase();

        /// <summary>
        /// Returns whether this uploader needs to have Adobe Flash functionality enabled. In Waco this always returns false.
        /// </summary>
        public bool IsFlashRequired { get { return false; } }

        /// <summary>
        /// Determines if a particular listing can be uploaded with the Waco Uploader
        /// </summary>
        /// <param name="listing">The listing to test for upload</param>
        /// <returns>True if the listing can be uploaded by the Waco Uploader, false if not.</returns>
        public bool CanUpload(ResidentialListingRequest listing)
        {
            return listing.MarketName == "Waco";
        }

        /// <summary>
        /// Inserts or updates a listing into the Waco MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <param name="media">The media files (mostly images) related to the listing</param>
        /// <returns>The final status of the upload operation and whether it succeeded or not</returns>
        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            //This method uploads the house to the system. 
            //The lifecycle of the WebDriver is managed by the host application.
            //Do not initialize or terminate the driver.

            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            if (driver.UploadInformation.IsNewListing)
            {
                // If operation is insert
                StartInsert(driver);
            }
            else
            {
                // If operation is update
                StartUpdate(driver, listing);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_dlInputList_ctl00_m_btnSelect")));
                driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect")); // Tab: Input | Option: Residential
            }

            EditGeneralTab(driver, listing);
            EditFeaturesTab(driver, listing);
            EditRoomsTab(driver, listing);
            EditRemarksTab(driver, listing);
            EditContractInfoTab(driver, listing);

            return UploadResult.Success;
        }

        public UploadResult Edit(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            if (driver.UploadInformation.IsNewListing)
            {
                // If operation is insert
                StartInsert(driver);
            }
            else
            {
                // If operation is update
                StartUpdate(driver, listing);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
                driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect")); // Tab: Input | Option: Residential
            }

            return UploadResult.Success;
        }
        /// <summary>
        /// Login an session in the MLS system
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <returns>The final status of the login operation and whether it succeeded or not</returns>
        public LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Navigate("https://matrix.waor.realtor/");
            driver.wait.Until(x => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_imgbtnLogin")));

            char[] username = listing.MarketUsername.ToArray();
            foreach (var charact in username)
            {
                Thread.Sleep(400);
                driver.FindElement(By.Id("m_tbName")).SendKeys(charact.ToString());
            }
            driver.FindElement(By.Id("m_tbName")).SendKeys(Keys.Tab);

            char[] password = listing.MarketPassword.ToArray();
            foreach (var charact in password)
            {
                Thread.Sleep(400);
                driver.FindElement(By.Id("m_tbPassword")).SendKeys(charact.ToString());
            }
            driver.FindElement(By.Id("m_tbPassword")).SendKeys(Keys.Tab);

            driver.ExecuteScript(" $('#m_imgbtnLogin').click();");
            Thread.Sleep(1000);

            return LoginResult.Logged;
        }

        public UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            StartUpdate(driver, listing);

            Thread.Sleep(1000);

            driver.Click(By.LinkText("Open House"));

            Thread.Sleep(1000);

            DeleteOpenHouses(driver, listing);
            if (listing.EnableOpenHouse)
            {
                AddOpenHouses(driver, listing);
            }

            return UploadResult.Success;
        }

        public void DeleteOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            bool flag = true;
            int attempt = 0;
            while (flag && attempt < 3)
            {
                driver.ExecuteScript("jQuery('html,body').animate({ scrollTop: 0, scrollLeft: 500 });");
                Thread.Sleep(1000);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Input")));
                driver.Click(By.LinkText("Input"));
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")));
                driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Edit")));
                driver.Click(By.LinkText("Edit")); // Link "Edit"
                driver.ScrollDown();
                Thread.Sleep(1000);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Open Houses")));
                driver.Click(By.LinkText("Open Houses"));
                Thread.Sleep(500);

                int previous = 0;
                while (driver.FindElements(By.LinkText("Delete")).Count() > 1)
                {
                    previous = driver.FindElements(By.LinkText("Delete")).Count();
                    try
                    {
                        driver.ExecuteScript("jQuery('html,body').animate({ scrollTop: 250, scrollLeft: 1000 });");
                        Thread.Sleep(1000);
                        driver.Click(By.LinkText("Delete"));
                    }
                    catch
                    {
                        return;
                    }
                    driver.Click(By.LinkText("Delete"));

                    if (previous == driver.FindElements(By.LinkText("Delete")).Count() &&
                        driver.FindElements(By.LinkText("Delete")).Count() != 1)
                    {
                        attempt++;
                        break;
                    }
                }
                if (driver.FindElements(By.LinkText("Delete")).Count() == 1)
                    flag = false;
                driver.ScrollDown();
                driver.ExecuteScript("jQuery('html,body').animate({ scrollLeft: 0 });");
                Thread.Sleep(1000);
                driver.Click(By.Id("m_lbSubmit"));
                Thread.Sleep(500);
                if (IsElementPresent(driver, By.Id("m_lbSubmit")))
                {
                    driver.ScrollDown();
                    driver.ExecuteScript("jQuery('html,body').animate({ scrollLeft: 0 });");
                    Thread.Sleep(1000);
                    driver.Click(By.Id("m_lbSubmit"));
                    Thread.Sleep(1000);
                }
            }
            driver.ExecuteScript("jQuery('html,body').animate({ scrollTop: 0, scrollLeft: 500 });");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Input")));
            driver.Click(By.LinkText("Input"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")));
            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Edit")));
            driver.Click(By.LinkText("Edit")); // Link "Edit"
            driver.ScrollDown();
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Open Houses")));
            driver.Click(By.LinkText("Open Houses"));
            Thread.Sleep(500);
        }

        #region Open House

        #region Virtual Tour

        public UploadResult UploadVirtualTour(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            StartUpdate(driver, listing);

            Thread.Sleep(1000);

            driver.Click(By.PartialLinkText("Virtual Tours/URLs"));

            Thread.Sleep(1000);

            var virtualTour = media.OfType<ResidentialListingVirtualTour>().FirstOrDefault();
            if (virtualTour != null)
                driver.WriteTextbox(By.Id("Input_453"), virtualTour.VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded

            return UploadResult.Success;
        }

        #endregion

        public bool IsElementPresent(CoreWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void AddOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(200);
            int max = 9;
            driver.ScrollDown();
            List<DateTime> date = OH.getNextDate(listing, max);

            // HCS-596
            listing.OHTypeMonOH = "PUBLIC";
            listing.OHTypeTueOH = "PUBLIC";
            listing.OHTypeWedOH = "PUBLIC";
            listing.OHTypeThuOH = "PUBLIC";
            listing.OHTypeFriOH = "PUBLIC";
            listing.OHTypeSatOH = "PUBLIC";
            listing.OHTypeSunOH = "PUBLIC";

            int i = 0;
            foreach (var local in date)
            {
                string[] openhousestart;
                string[] openhouseend;

                string day = local.DayOfWeek.ToString().Substring(0, 3);
                if (listing.GetType().GetProperty("OHStartTime" + day + "OH").GetValue(listing, null) != null && listing.GetType().GetProperty("OHEndTime" + day + "OH").GetValue(listing, null) != null)
                {
                    openhousestart = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHStartTime" + day + "OH").GetValue(listing, null).ToString());
                    openhouseend = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHEndTime" + day + "OH").GetValue(listing, null).ToString());

                    driver.WriteTextbox(By.Id("_Input_327__REPEAT" + i + "_321"), local.ToShortDateString());
                    driver.WriteTextbox(By.Id("_Input_327__REPEAT" + i + "_TextBox_322"), openhousestart[0]);

                    if (openhousestart[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id("_Input_327__REPEAT" + i + "_RadioButtonList_322_0"), openhousestart[1]);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id("_Input_327__REPEAT" + i + "_RadioButtonList_322_1"), openhousestart[1]);
                    }
                    driver.WriteTextbox(By.Id("_Input_327__REPEAT" + i + "_TextBox_323"), openhouseend[0]);
                    if (openhouseend[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id("_Input_327__REPEAT" + i + "_RadioButtonList_323_0"), openhouseend[1]);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id("_Input_327__REPEAT" + i + "_RadioButtonList_323_1"), openhouseend[1]);
                    }

                    driver.SetSelect(By.Id("_Input_327__REPEAT" + i + "_324"), "1");

                    if (listing.GetType().GetProperty("OHType" + day + "OH").GetValue(listing, null) != null && listing.GetType().GetProperty("OHType" + day + "OH").GetValue(listing, null).ToString() != "None")
                        driver.SetSelect(By.Id("_Input_327__REPEAT" + i + "_320"), listing.GetType().GetProperty("OHType" + day + "OH").GetValue(listing, null).ToString());

                    /* if (Data.GetType().GetProperty("OHRefreshments" + day).GetValue(Data, null) != null)
                            driver.SetMultipleCheckboxById("_Input_327__REPEAT" + i + "_325", Data.GetType().GetProperty("OHRefreshments" + day).GetValue(Data, null).ToString());*/

                    if (listing.GetType().GetProperty("OHComments" + day + "OH").GetValue(listing, null) != null)
                        driver.WriteTextbox(By.Id("_Input_327__REPEAT" + i + "_326"), listing.GetType().GetProperty("OHComments" + day + "OH").GetValue(listing, null).ToString());

                    i++;
                    if (max >= (i + 1))
                    {
                        driver.ScrollDown();
                        driver.ScrollDown();
                        driver.ExecuteScript("jQuery('html,body').animate({ scrollLeft: 1000 });");
                        Thread.Sleep(1000);
                        driver.ScrollDownPosition(2000);
                        driver.Click(By.LinkText("More"));
                    }

                    driver.ScrollDown();
                }
            }
        }

        #endregion

        /// <summary>
        /// Logs out an existing session from the MLS system
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <returns>The final status of the logout operation and whether it succeeded or not</returns>
        public UploadResult Logout(CoreWebDriver driver)
        {
            Thread.Sleep(1000);
            driver.Navigate("https://matrix.waor.realtor/Matrix/Logout.aspx");
            return UploadResult.Success;
        }

        private static string TransformStatus(string status, ref string linkText)
        {
            switch (status)
            {
                case "A":
                    linkText = "Change to Active";
                    return "A";
                case "AC":
                    linkText = "Change to Active Contingent";
                    return "AC";
                case "AKO":
                    linkText = "Change to Active Kick Out";
                    return "AKO";
                case "AOC":
                    linkText = "Change to Under Contract W/Contingency";
                    return "AOC";
                case "C":
                    linkText = "Change to Cancelled";
                    return "CAN";
                case "X":
                    return "EXP";
                case "P":
                    linkText = "Change to Under Contract";
                    return "PND";
                case "S":
                    linkText = "Change to Sold";
                    return "SLD";
                case "T":
                case "W":
                case "WS":
                    linkText = "Change to Temp Off Market";
                    return "TOM";
                default:
                    throw new ArgumentOutOfRangeException("status", status, @"The status '" + status + @"' is not configured for Waco");
            }
        }

        #region Upload Code

        private void StartInsert(CoreWebDriver driver)
        {
            driver.Navigate("https://matrix.waor.realtor/Matrix/Input");

            driver.Click(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbAddNewItem"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_dlInputList_ctl00_m_btnSelect")));
            driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")));
            driver.Click(By.Id("m_rpFillFromList_ctl03_m_lbPageLink"));
        }

        private void StartUpdate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Navigate("https://matrix.waor.realtor/Matrix/Input");
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.Click(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit"));
        }

        private void EditGeneralTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_rpPageList_ctl00_lbPageLink")));
            driver.Click(By.Id("m_rpPageList_ctl00_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            /*
            #region Listing Informacion
            driver.ScrollDown(100);
            driver.SetSelect(By.Id("Input_57"), listing.PropSubType); //Property Type
            driver.SetSelect(By.Id("Input_92"), "EXCAGE"); // Listing Type
            driver.SetSelect(By.Id("Input_94"), "FS"); //Transaction Type
            driver.WriteTextbox(By.Name("Input_308"), String.Empty); //Lease MLS#

            driver.SetMultipleCheckboxById("Input_90", listing.HousingTypeDesc); //Housing Type
            driver.SetMultipleCheckboxById("Input_103", listing.HousingStyleDesc); //Style of Houses
            driver.SetMultipleCheckboxById("Input_112", listing.ConstructionDesc); // Construction

            // Update Construction Status
            UpdateConstructionStatusInGeneralTab(driver, listing);

            driver.SetSelect(By.Id("Input_546"), "NO"); // Will Subdivide
            

            #region ListDate

            if (driver.UploadInformation.IsNewListing)
            {
                DateTime? listDate = null;
                switch (listing.ListStatus)
                {
                    case "A":
                    case "AC":
                    case "AKO":
                    case "AOC":
                        listDate = DateTime.Now;
                        break;
                    case "P":
                        listDate = DateTime.Now.AddDays(-2);
                        break;
                    case "S":
                        listDate = DateTime.Now.AddDays(-HusaMarketConstants.ListDateSold);
                        break;
                }

                if (listDate.HasValue)
                {
                    driver.WriteTextbox(By.Id("Input_93"), listDate.Value.ToShortDateString()); // List Date
                }
            }

            driver.WriteTextbox(By.Id("Input_95"), DateTime.Today.AddYears(1).ToShortDateString()); // Expire Date

            #endregion

            if (listing.SqFtTotal != null)
                driver.WriteTextbox(By.Id("Input_104"), listing.SqFtTotal); // SqFt

            driver.SetSelect(By.Id("Input_105"), listing.SqFtSource); // SqFt Source
            driver.WriteTextbox(By.Id("Input_75"), string.IsNullOrWhiteSpace(listing.TaxID) ? "NA" : listing.TaxID); // Parcel ID

            driver.SetSelect(By.Id("Input_72"), "0"); // Multi Parcel ID

            #endregion
            */
            #region Location Information

            var window = driver.WindowHandles.FirstOrDefault(c => c == driver.CurrentWindowHandle);

            if (driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_153"), listing.StreetNum); // Street/Box Number
                driver.Click(By.CssSelector("a[title='Find a Street Name']"));
                driver.SwitchTo().Window(driver.WindowHandles.Last());

                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lbSearch")));
                driver.WriteTextbox(By.Id("Fm69_Ctrl481_DictionaryLookup"), listing.StreetName.Replace('\'', ' ')); // Street Name
                driver.Click(By.Id("m_lbSearch"));

                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_DGSearchResults")));
                if (driver.FindElements(By.LinkText("Fill")).Count() > 0)
                {
                    driver.FindElements(By.LinkText("Fill"))[0].Click();
                }
                driver.SwitchTo().Window(window);
                Thread.Sleep(100);

                // driver.SetSelect(By.Id("Input_154"), listing.); // Dir Prefix
                if (!string.IsNullOrWhiteSpace(listing.StreetType))
                {
                    driver.SetSelect(By.Id("Input_156"), listing.StreetType); // Street Type
                }
                // driver.SetSelect(By.Id("Input_459"), listing.);  // Dir Suffix
                driver.SetSelect(By.Id("Input_152"), listing.County); // County
            }
            driver.SetSelect(By.Id("Input_158"), listing.CityCode); // City
            driver.SetSelect(By.Id("Input_163"), listing.State); // State
            driver.WriteTextbox(By.Id("Input_159"), listing.Zip); // Zip
            driver.WriteTextbox(By.Id("Input_157"), listing.UnitNum); // Unit Number

            driver.Click(By.CssSelector("a[title='Find a Subdivision']"));
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lbSearch")));
            driver.WriteTextbox(By.Id("Fm70_Ctrl482_DictionaryLookup"), listing.Subdivision); // Subdivision
            driver.Click(By.Id("m_lbSearch"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_DGSearchResults")));
            if (driver.FindElements(By.LinkText("Fill")).Count() > 0)
            {
                driver.FindElements(By.LinkText("Fill"))[0].Click();
            }
            driver.SwitchTo().Window(window);
            Thread.Sleep(100);

            driver.WriteTextbox(By.Id("Input_160"), !String.IsNullOrEmpty(listing.Block) ? listing.Block : ""); // Block
            driver.WriteTextbox(By.Id("Input_161"), !String.IsNullOrEmpty(listing.LotNum) ? listing.LotNum : ""); // Lot

            SetLongitudeAndLatitudeValues(driver, listing);

            #endregion

            #region School Information

            Thread.Sleep(500);

            driver.SetSelect(By.Id("Input_164"), listing.SchoolDistrict); // School District

            Thread.Sleep(500);

            driver.SetSelect(By.Id("Input_299"), listing.SchoolName1); // Elementary School

            Thread.Sleep(500);

            /*driver.SetSelect(By.Id("Input_97"), listing.SchoolName2); // Middle School

            Thread.Sleep(500);

            driver.SetSelect(By.Id("Input_98"), listing.SchoolName3); // High School 

            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_458"), listing.SchoolName7); // Intermediate School 

            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_459"), listing.SchoolName5); // Junior School 

            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_592"), listing.SchoolName6); // Senior High School */

            #endregion

            driver.WriteTextbox(By.Id("Input_166"), string.IsNullOrWhiteSpace(listing.TaxID) ? "NA" : listing.TaxID); // Parcel ID

            // driver.SetSelect(By.Id("Input_461"), listing.); // Reason for TBD
            driver.WriteTextbox(By.Id("Input_184"), listing.TaxUnexempt); // Tax (No Exemptions)
            driver.SetSelect(By.Id("Input_185"), "0"); // Foreclosure
            driver.SetSelect(By.Id("Input_186"), "0"); // Short Sale
            driver.WriteTextbox(By.Id("Input_341"), listing.RateYear); // Tax (with Exemptions)
            driver.WriteTextbox(By.Id("Input_340"), listing.TaxYear); // TaxYear
            driver.WriteTextbox(By.Id("Input_167"), listing.ETJDesc); // Legal Description

            #region Property Details

            string propSubType = listing.PropSubType; // Wrong mapped on database
            switch (listing.PropSubType)
            {
                case "RESCON":
                    propSubType = "CND";
                    break;
                case "RESTOW":
                    propSubType = "TNX";
                    break;
                case "RESFAM":
                    propSubType = "SFM";
                    break;
            }
            driver.SetSelect(By.Id("Input_149"), propSubType); // Property Sub Type
            driver.SetSelect(By.Id("Input_168"), "0"); // Manufactured Home
            // driver.SetMultipleCheckboxById("_Input_253__REPEAT" + i + "_252", roomType.Features); // Manufactured Info
            driver.SetSelect(By.Id("Input_170"), "1"); // New Construction
            Thread.Sleep(500);
            driver.ScrollDown();
            driver.WriteTextbox(By.Id("Input_171"), listing.YearBuilt != null ? listing.YearBuilt.ToString() : "0"); // Year Built
            driver.WriteTextbox(By.Id("Input_173"), listing.Beds); // Beds Total
            driver.WriteTextbox(By.Id("Input_174"), listing.BathsFull); // Baths Full
            driver.WriteTextbox(By.Id("Input_175"), listing.BathsHalf); // Baths Half
            driver.WriteTextbox(By.Id("Input_284"), listing.NumStories); // # of Stories (levels)
            // driver.SetMultipleCheckboxById(By.Id("Input_386_"), listing.); // Style
            driver.WriteTextbox(By.Id("Input_176"), listing.SqFtTotal); // Main House Total SqFt (Not Guest House SqFt)
            driver.SetSelect(By.Id("Input_177"), "BUILD"); // SqFt Source
            driver.WriteTextbox(By.Id("Input_179"), listing.Acres); // Apx Acreage
            driver.SetSelect(By.Id("Input_181"), listing.HasOtherFees); // HOA 
            driver.WriteTextbox(By.Id("Input_182"), listing.AssocFee);  // HOA Amount
            driver.SetSelect(By.Id("Input_183"), listing.HOA); // HOA Term
            // driver.SetSelect(By.Id("Input_385"), listing.); // Package Deal

            #endregion Property Details
        }

        private void EditRoomsTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl04_lbPageLink")); // Tab: Input | Subtab: Rooms
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer"))); // Look if the footer elements has been loaded
            Thread.Sleep(2000);

            if (!driver.UploadInformation.IsNewListing)
            {
                var elems = driver.FindElements(By.CssSelector("table[id^=_Input_257__del_REPEAT] a"));
                int index = 0;

                while (driver.FindElements(By.LinkText("Delete")) != null &&
                    driver.FindElements(By.LinkText("Delete")).Count > 1)
                {
                    try
                    {
                        driver.ScrollToTop();
                        driver.Click(By.LinkText("Delete"));
                    }
                    catch
                    {
                        driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink"));
                        driver.ExecuteScript("Subforms['s_257'].deleteRow('_Input_257__del_REPEAT" + index + "_');");
                        Thread.Sleep(400);
                        continue;
                    }
                }
            }

            var roomTypes = ReadRoomAndFeatures(listing);

            driver.Click(By.Id("m_rpPageList_ctl04_lbPageLink"));
            Thread.Sleep(400);

            var i = 0;

            foreach (var roomType in roomTypes.Where(c => c.IsValid()))
            {
                if (i > 0)
                {
                    driver.ScrollDown();
                    driver.Click(By.Id("_Input_257_more"));
                    Thread.Sleep(400);
                }

                driver.SetSelect(By.Id("_Input_257__REPEAT" + i + "_255"), roomType.Value, true); // Room
                Thread.Sleep(400);
                driver.ScrollDown();
                driver.SetSelect(By.Id("_Input_257__REPEAT" + i + "_256"), roomType.Level, true); // Level
                Thread.Sleep(400);
                driver.ScrollDown();
                driver.SetSelect(By.Id("_Input_257__REPEAT" + i + "_379"), roomType.Features, true); // Notes

                i++;
            }
        }

        private void EditContractInfoTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl08_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer"))); // Look if the footer elements has been loaded
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Id("Input_239"), listing.ListPrice); // List Price

            if (driver.UploadInformation.IsNewListing)
            {
                DateTime? listDate = null;
                switch (listing.ListStatus)
                {
                    case "A":
                    case "AC":
                    case "AKO":
                    case "AOC":
                        listDate = DateTime.Now;
                        break;
                    case "P":
                        listDate = DateTime.Now.AddDays(-2);
                        break;
                    case "S":
                        listDate = DateTime.Now.AddDays(-HusaMarketConstants.ListDateSold);
                        break;
                }

                if (listDate.HasValue)
                {
                    driver.WriteTextbox(By.Id("Input_240"), listDate.Value.ToShortDateString()); // List Date
                }
            }

            driver.WriteTextbox(By.Id("Input_241"), DateTime.Today.AddYears(1).ToShortDateString()); // Expire Date
            driver.SetSelect(By.Id("Input_242"), "EXAGN"); // Type of Listing
            driver.SetSelect(By.Id("Input_244"), "0"); // Owner Finance
            driver.WriteTextbox(By.Id("Input_238"), listing.OwnerName); // Seller(s) Name
            driver.SetSelect(By.Id("Input_243"), "VACAN"); // Occupancy
            string buyerAgentAmount = listing.CompBuy.Replace("%", "").Replace("%", "");
            driver.WriteTextbox(By.Id("Input_251"), buyerAgentAmount); // Buyer Agency Amount
            if (listing.CompBuy.EndsWith("%"))
            {
                driver.SetSelect(By.Id("Input_252"), "Pct"); // Buyer Agency Comp Type
            }
            else if (listing.CompBuy.EndsWith("$"))
            {
                driver.SetSelect(By.Id("Input_252"), "Dollars"); // Buyer Agency Comp Type
            }

            // driver.SetSelect(By.Id("Input_456"), listing.); // Named Exclusions
            driver.SetSelect(By.Id("Input_346"), "0"); // Variable Commission Fee
            driver.WriteTextbox(By.Id("Input_253"), "0.00"); // Sub Agent Amount
            driver.SetSelect(By.Id("Input_254"), "Pct"); // Sub Agent Comp Type
            driver.WriteTextbox(By.Id("Input_246"), listing.ShowingInstructions); // Showing Instructions
            driver.SetMultipleCheckboxById("Input_245", "NEGOT"); // Possession
            driver.SetMultipleCheckboxById("Input_237", listing.LockboxLocDesc); // Lockbox
            driver.SetMultipleCheckboxById("Input_192", listing.AppliancesDesc); // Financial Terms
            // driver.SetSelect(By.Id("Input_249"), listing.); // IDX Opt In
            driver.SetSelect(By.Id("Input_483"), "0"); // Animals on Premises
            // driver.SetSelect(By.Id("Input_250"), listing.); // Zillow
            // driver.SetSelect(By.Id("Input_338"), listing.); // Realtor.com
            // driver.WriteTextbox(By.Id("Input_482"), listing.); // Animals Description
            // driver.WriteTextbox(By.Id("Input_247"), listing.); // Lockbox Combo
            // driver.WriteTextbox(By.Id("Input_248"), listing.); // Gate Code

            driver.ScrollDown();
            // driver.WriteTextbox(By.Id("Input_220"), listing.); // List Agent
            //driver.Click(By.Id("Input_220_Refresh"));

            //driver.ScrollDown();
            // driver.WriteTextbox(By.Id("Input_222"), listing.); // Co - List Agent
            //driver.Click(By.Id("Input_222_Refresh"));

            //driver.ScrollDown();
            // driver.WriteTextbox(By.Id("Input_463"), listing.); // Co-List Agent 2
            //driver.Click(By.Id("Input_463_Refresh"));

            //driver.ScrollDown();
            // driver.WriteTextbox(By.Id("Input_447"), listing.); // List Team ID
            //driver.Click(By.Id("Input_447_Refresh"));

            //driver.ScrollDown();
            // driver.WriteTextbox(By.Id("Input_451"), listing.); // Co-List Team ID
            //driver.Click(By.Id("Input_451_Refresh"));


        }

        private void EditFeaturesTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink"));
            Thread.Sleep(1000);
            driver.SetMultipleCheckboxById("Input_188", listing.ConstructionDesc); // Construction Exterior
            driver.SetSelect(By.Id("Input_494"), listing.IsGatedCommunity); // Gated Community
            driver.SetMultipleCheckboxById("Input_189", listing.EnergyDesc.ToUpper()); // Energy Features
            driver.SetSelect(By.Id("Input_492"), listing.SolarPanels_YN); // Solar Panels Y/N
            driver.SetSelect(By.Id("Input_493"), listing.SolarPanels); // Solar Panels
            driver.SetMultipleCheckboxById("Input_495", listing.ExteriorDesc); // Exterior Features
            driver.SetMultipleCheckboxById("Input_191", listing.FenceDesc); // Fencing
            driver.SetSelect(By.Id("Input_205"), "0");  // Pool
            // driver.SetSelect(By.Id("Input_457"), listing.); // Pool Type
            // driver.SetSelect(By.Id("Input_458"), listing.); //Pool Class
            // driver.SetMultipleCheckboxById("Input_206", listing.);  // Pool Features
            driver.ScrollDown(250);
            driver.SetMultipleCheckboxById("Input_194", listing.FloorsDesc.Replace("OTHER", "OTH")); // Flooring Type
            driver.SetMultipleCheckboxById("Input_195", listing.FoundationDesc); // Foundation Type

            driver.ScrollDown(100);

            driver.SetMultipleCheckboxById("Input_196", listing.GarageDesc); // Garage Carport
            driver.SetMultipleCheckboxById("Input_203", !string.IsNullOrEmpty(listing.ParkingDesc) ? listing.ParkingDesc.Replace("PAVED", "PAVD") : ""); // Parking
            driver.SetMultipleCheckboxById("Input_207", listing.RoofDesc); // Roof Type

            driver.WriteTextbox(By.Id("Input_288"), listing.GarageCapacity); // Garage / Carport Capacity
            driver.SetSelect(By.Id("Input_489"), "0"); // Guest Quarters Y/N
            driver.SetMultipleCheckboxById("Input_193", listing.FireplaceDesc); // Fireplace
            driver.SetMultipleCheckboxById("Input_210", listing.UtilityRoomDesc); // Utility Room/Laundry
            driver.SetMultipleCheckboxById("Input_199", listing.InteriorDesc); // Interior Features
            driver.SetMultipleCheckboxById("Input_200", !string.IsNullOrEmpty(listing.KitchenDesc) ? listing.KitchenDesc.Replace("ICE", "ICEM").Replace("WINEF", "WINE") : ""); // Kitchen Features

            driver.ScrollDown(200);
            driver.SetMultipleCheckboxById("Input_202", !string.IsNullOrEmpty(listing.BedBathDesc) ? listing.BedBathDesc.Replace("STUB", "SEPT") : ""); // Master Suite Features
            // driver.WriteTextbox(By.Id("Input_487"), listing.); // Guest Quarters SqFt
            driver.SetSelect(By.Id("Input_208"), listing.AppliancesYN); // Sewer
            driver.SetSelect(By.Id("Input_209"), (bool)listing.HasGas ? "1" : "0"); // Septic
            driver.SetMultipleCheckboxById("Input_198", listing.HeatSystemDesc); // HVAC
            driver.SetMultipleCheckboxById("Input_214", !string.IsNullOrEmpty(listing.WaterExtras) ? listing.WaterExtras.Replace("TANK", "TANKL") : ""); // Water Heater
            driver.SetSelect(By.Id("Input_211"), listing.HasWaterAccess); // Water Frontage/Access
            driver.SetSelect(By.Id("Input_212"), listing.WaterView); // Water View
            driver.SetMultipleCheckboxById("Input_187", listing.CommonFeatures);  // Comm/Internet
            driver.WriteTextbox(By.Id("Input_345"), "NA"); // Present Internet Co
        }

        private void EditLotUtilityEnvironmentTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl06_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            #region Lot Information

            driver.WriteTextbox(By.Id("Input_138"), listing.Acres); // Acres

            driver.WriteTextbox(By.Id("Input_135"), listing.LotDim); // Lot Dimensions
            driver.SetSelect(By.Id("Input_136"), listing.LotSize); // Lot Size/Acreage
            driver.SetMultipleCheckboxById("Input_132", listing.LotDesc); // Lot Description
            driver.SetMultipleCheckboxById("Input_131", listing.ExteriorDesc); // Exterior Features
            driver.SetMultipleCheckboxById("Input_133", listing.Restrictions); // Restrictions

            driver.ScrollDown(250);
            driver.SetMultipleCheckboxById("Input_129", listing.FenceDesc); // Type of Fence
            driver.SetMultipleCheckboxById("Input_128", listing.SoilType); // Soil

            #endregion

            #region Waterfront 

            // Waterfront Features
            if (listing.LotDesc != null)
            {
                if (listing.LotDesc.Contains("LFMB") && listing.LotDesc.Contains("LAKFRO"))
                    driver.SetSelect(By.Id("Input_731"), 1);

                if (listing.LotDesc.Contains("LFMB"))
                    driver.Click(By.Id("Input_735_LFMB"));
                if (listing.LotDesc.Contains("LAKFRO"))
                    driver.Click(By.Id("Input_735_LF"));
            }
            #endregion

            #region Utility Information
            driver.ScrollDown(250);
            driver.SetMultipleCheckboxById("Input_141", listing.UtilitiesDesc); // Street/Utilities
            driver.SetMultipleCheckboxById("Input_101", listing.HeatSystemDesc); // Heating/Cooling
            driver.SetSelect(By.Id("Input_127"), listing.MUDDistrict); // MUD District

            #endregion

            #region Environment Information
            driver.ScrollDown();
            driver.SetMultipleCheckboxById("Input_144", listing.GreenFeatures); // Green Features
            driver.SetMultipleCheckboxById("Input_143", listing.GreenCerts); // Green Certification
            driver.SetMultipleCheckboxById("Input_142", listing.EnergyDesc); // Energy Efficiency

            #endregion
        }

        private void EditFinancialTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl08_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.SetMultipleCheckboxById("Input_167", "CASH,CONVEN,FHA,TEXVET,VA,OTHER"); // Proposed Financing

            driver.SetSelect(By.Id("Input_207"), listing.HOA); // HOA
            driver.SetSelect(By.Id("Input_155"), listing.AssocFeePaid); // HOA Billing Freq
            driver.WriteTextbox(By.Id("Input_170"), listing.AssocFee); // HOA Dues
            driver.WriteTextbox(By.Id("Input_670"), listing.AssocName); // HOA Management Co
            driver.WriteTextbox(By.Id("Input_671"), listing.AssocPhone); // HOA Managemt Co Phone	

            driver.SetMultipleCheckboxById("Input_161", "NEGOTI"); // Posession
            driver.WriteTextbox(By.Id("Input_153"), listing.LoanBalance); // Balance
            driver.WriteTextbox(By.Id("Input_159"), listing.TitleCo); // Preferred Title Company
            driver.WriteTextbox(By.Id("Input_165"), listing.TitleCoPhone); // Title Co Phone
            driver.SetSelect(By.Id("Input_160"), "0"); // Possible Short Sale
            driver.SetSelect(By.Id("Input_158"), "TRASCL"); // Loan Type
            driver.SetSelect(By.Id("Input_163"), listing.LoanPaymentType); // Payment Type
            driver.WriteTextbox(By.Id("Input_162"), listing.LoanPayment); // Payment
            driver.WriteTextbox(By.Id("Input_164"), listing.TitleCoLocation); // Title Company Location
            driver.SetSelect(By.Id("Input_264"), "0"); // 2nd Mortgage 
            driver.ScrollDown();
            driver.SetMultipleCheckboxById("Input_156", listing.AssocFeeIncludes); // HOA Includes
        }

        private void EditShowingTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl10_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.WriteTextbox(By.Id("Input_175"), listing.BrokerLicenseNum);  // List Agent MLS ID
            driver.ExecuteScript("javascript:document.getElementById('Input_175_Refresh').value='1';RefreshToSamePage();");

            Thread.Sleep(1000);

            driver.WriteTextbox(By.Id("Input_538"), "Ben Caballero"); // Office Supervisor

            driver.WriteTextbox(By.Id("Input_739"), listing.BrokerLicenseNum); // Office Supervisor License #

            driver.ScrollDown();
            driver.SetSelect(By.Id("Input_201"), "0"); // Variable Fee

            if (!String.IsNullOrWhiteSpace(listing.CompBuy))
                driver.WriteTextbox(By.Id("Input_191"), listing.CompBuy);
            else
                driver.WriteTextbox(By.Id("Input_191"), "3%"); // Buyers Agency Commission

            driver.WriteTextbox(By.Id("Input_188"), "0%"); // SubAgency Commission
            driver.SetSelect(By.Id("Input_119"), "NONE"); // Keybox Type
            driver.WriteTextbox(By.Id("Input_185"), "0"); // Keybox #
            //driver.WriteTextbox(By.Id("Input_196"), (!String.IsNullOrEmpty(listing.AltPhoneCommunity) ? listing.AltPhoneCommunity : listing.AlternatePhoneFromCompany)); // Owner Alt Phone

            //UP-190
            if (String.IsNullOrEmpty(listing.AltPhoneCommunity))
                driver.WriteTextbox(By.Id("Input_196"), "");
            else
                driver.WriteTextbox(By.Id("Input_196"), listing.AltPhoneCommunity);
            driver.SetSelect(By.Id("Input_200"), "BUILDE"); // Seller Type

            driver.WriteTextbox(By.Id("Input_198"), listing.OwnerName); // Owner Name
            driver.SetMultiSelect(By.Id("Input_678"), listing.Showing); //Showing

            String realtorContactEmail = String.Empty;
            if (!String.IsNullOrEmpty(listing.ContactEmailFromCompany))
                realtorContactEmail = listing.ContactEmailFromCompany;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmail))
                realtorContactEmail = listing.RealtorContactEmail;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmailFromCommunityProfile))
                realtorContactEmail = listing.RealtorContactEmailFromCommunityProfile;

            //UP-78
            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !listing.ShowingInstructions.RemoveSlash().ToLower().Contains("email contact") &&
                !listing.ShowingInstructions.RemoveSlash().ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";
            driver.WriteTextbox(By.Id("Input_204"), listing.ShowingInstructions.RemoveSlash() + realtorContactEmail); // Showing Instructions

            var apptPhone = listing.AgentListApptPhone;
            driver.WriteTextbox(By.Id("Input_189"), (!string.IsNullOrEmpty(apptPhone) ? apptPhone : "")); //  Appt Phone
            // END BEGIN UP-73
        }

        private void EditRemarksTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl06_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            UpdatePublicRemarksInRemarksTab(driver, listing);

            #region Private Remark

            // 1. BonusCheckBox
            // 2. BonusWAmountCheckBox
            // 3. BuyerCheckBox
            String bonusMessage = "";
            if (listing.BonusCheckBox.Equals(true) && listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Bonus & Buyer Incentives; ask Builder for details. ";
            else if (listing.BonusWAmountCheckBox.Equals(true) && listing.BuyerCheckBox.Equals(true))
                bonusMessage = bonusMessage + "$" + listing.AgentBonusAmount + " Agent Bonus. Possible Buyer Incentive; ask Builder for details. ";
            else if (listing.BonusCheckBox.Equals(true))
                bonusMessage = "Possible Bonus; ask Builder for details. ";
            else if (listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Buyer Incentives; ask Builder for details. ";
            else if (listing.BonusWAmountCheckBox.Equals(true))
                bonusMessage = bonusMessage + "$" + listing.AgentBonusAmount + " Agent Bonus; ask Builder for details. ";
            else
                bonusMessage = "";

            String realtorContactEmail = String.Empty;
            if (!String.IsNullOrEmpty(listing.EmailRealtorsContact))
                realtorContactEmail = listing.EmailRealtorsContact;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmail))
                realtorContactEmail = listing.RealtorContactEmail;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmailFromCommunityProfile))
                realtorContactEmail = listing.RealtorContactEmailFromCommunityProfile;

            //UP-78
            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";
            if (!String.IsNullOrEmpty(listing.AgentPrivateRemarks) && !driver.UploadInformation.IsNewListing) //Allow Comments/Reviews/Email contact/Agent Private Remarks
                driver.WriteTextbox(By.Id("Input_216"), listing.AgentPrivateRemarks);
            else
                driver.WriteTextbox(By.Id("Input_216"), "");

            string message = bonusMessage + listing.GetPrivateRemarks(false) + realtorContactEmail;

            string incompletedBuiltNote = "";
            if (listing.YearBuiltDesc == "NCI"
                && !message.Contains("Home is under construction. For your safety, call appt number for showings"))
            {
                incompletedBuiltNote = "Home is under construction. For your safety, call appt number for showings. ";
            }

            driver.WriteTextbox(By.Id("Input_216"), incompletedBuiltNote + message);

            #endregion

            #region Direction

            string direction = listing.Directions;
            if (!string.IsNullOrEmpty(direction))
            {
                direction = direction.RemoveSlash();
                int dirLen = direction.Length;
                if (direction.ElementAt(dirLen - 1) == '.')
                    direction = direction.Remove(dirLen - 1);
                else
                    direction = direction + ".";
            }

            driver.WriteTextbox(By.Id("Input_217"), direction); // Directions

            #endregion

            //driver.WriteTextbox(By.Id("Input_217"), listing.); // 	Virtual Tour
        }

        private void UpdateConstructionStatusInGeneralTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_170")));
            //driver.SetSelect(By.Id("Input_113"), listing.YearBuiltDesc); //Construction Status
            driver.SetSelect(By.Id("Input_170"), "1"); // New Construction
            driver.WriteTextbox(By.Id("Input_171"), listing.YearBuilt != null ? listing.YearBuilt.ToString() : "0");
        }

        private void UpdatePublicRemarksInRemarksTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            var status = BuiltStatus.WithCompletion;

            switch (listing.YearBuiltDesc)
            {
                case "PROPOS":
                    status = BuiltStatus.ToBeBuilt;
                    break;
                case "NCC":
                    status = BuiltStatus.ReadyNow;
                    break;
                case "NCI":
                    status = BuiltStatus.WithCompletion;
                    break;
            }

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_215")));
            driver.WriteTextbox(By.Id("Input_215"), listing.GetPublicRemarks(status)); // Property Description
        }

        #region Rooms Tab

        private IEnumerable<RoomType> ReadRoomAndFeatures(ResidentialListingRequest listing)
        {
            return new List<RoomType>()
            {
                #region Living
                new RoomType("LIVIN", listing.LivingRoom1Level, null, null, listing.LivingRoom1Dim),
                new RoomType("LVRM2", listing.Bed3Level, null, null, listing.Bed3Dim),
                new RoomType("GAMER", listing.OtherRoom2Level, null, null, listing.OtherRoom2Dim),
                #endregion

                #region Dining
                new RoomType("DININ", listing.DiningRoomLevel, null, null, listing.DiningRoomDim),
                #endregion

                #region Breakfast
                new RoomType("BKFRO", listing.BreakfastLevel, null, null, listing.BreakfastDim),
                #endregion

                #region Kitchen
                new RoomType("KITCH", listing.KitchenLevel, null, null, listing.KitchenDim),
                #endregion

                #region Master Bedroom
                new RoomType("MBDRO", listing.Bed2Level, null, null, listing.Bed1Dim),
                #endregion

                #region Bedroom
                new RoomType("BEDRO", listing.Bed1Level, null, null, listing.Bath1Dim),
                new RoomType("BEDRO", listing.BedRoom2Level, null, null, listing.BedRoom2Dim),
                new RoomType("BEDRO", listing.BedRoom3Level, null, null, listing.BedRoom3Dim),
                new RoomType("BEDRO", listing.BedRoom4Level, null, null, listing.BedRoom4Dim),
                #endregion

                #region Office
                new RoomType("OFFIC", listing.StudyLevel, null, null, listing.StudyDim),
                #endregion

                #region Utility
                //new RoomType("UTIROO", listing.UtilityRoomLevel, listing.UtilityRoomLength, listing.UtilityRoomWidth,listing.UtilityRoomDesc),
                #endregion

                #region Other
                new RoomType("THEAT", listing.UtilityRoomLevel, null, null, listing.UtilityRoomDim),
                new RoomType("LOFT", listing.LivingRoom2Level, null, null, listing.LivingRoom2Dim),
                new RoomType("LIVDIN", listing.LivingRoom3Level, null, null, listing.Bed5Dim),
                new RoomType("MUDRO", listing.Bed4Level, null, null, listing.Bed4Dim),
                new RoomType("OTHER", listing.OtherRoom1Level, null, null, listing.OtherRoom1Dim),
                #endregion
            };
        }

        private class RoomType
        {
            internal readonly string Value;
            internal readonly string Level;
            internal readonly string Length;
            internal readonly string Width;
            internal readonly string Features;

            public RoomType(string value, string level, int? length, int? width, string features)
            {
                Value = value;
                Level = level ?? string.Empty;
                Length = length == null ? string.Empty : length.ToString();
                Width = width == null ? string.Empty : width.ToString();
                Features = features ?? string.Empty;
            }

            public bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(Level)/* && !string.IsNullOrWhiteSpace(Features)*/;
            }
        }

        #endregion

        /// <summary>
        /// This method makes set of values to the Longitude and Latitud fields 
        /// </summary>
        /// <param name="driver"></param>
        private void SetLongitudeAndLatitudeValues(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_rpPageList_ctl00_lbPageLink")));
            driver.Click(By.Id("m_rpPageList_ctl00_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            if (String.IsNullOrEmpty(listing.MLSNum))
            {
                driver.WriteTextbox(By.Id("INPUT__93"), listing.Latitude); // Latitude
                driver.WriteTextbox(By.Id("INPUT__94"), listing.Longitude); // Longitude
            }
        }

        #endregion

        /// <summary>
        /// Updates a listing's completion date in the Waco MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <returns>The final status of the Completion Date update operation and whether it succeeded or not</returns>
        public UploadResult UpdateCompletionDate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Login(driver, listing);

            StartUpdate(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_dlInputList_ctl00_m_btnSelect")));
            driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect"));

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_rpPageList_ctl00_lbPageLink")));
            driver.Click(By.Id("m_rpPageList_ctl00_lbPageLink"));
            UpdateConstructionStatusInGeneralTab(driver, listing);

            //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_rpPageList_ctl12_lbPageLink")));
            driver.Click(By.Id("m_rpPageList_ctl06_lbPageLink"));
            UpdatePublicRemarksInRemarksTab(driver, listing);

            return UploadResult.Success;
        }

        /// <summary>
        /// Updates a listing's images in the Waco MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <param name="media">The media files (mostly images) related to the listing</param>
        /// <returns>The final status of the image update operation and whether it succeeded or not</returns>
        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            StartUpdate(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            //if (media.Any(c => c is ResidentialListingVirtualTour))
            //{
            //    // Enter Image Management
            //    driver.Click(By.LinkText("Virtual Tours/URLs"));

            //    //Prepare Media

            //    var virtualTour = media.OfType<ResidentialListingVirtualTour>().First();

            //    driver.WriteTextbox(By.Id("Input_453"), virtualTour.VirtualTourAddress.Replace("http://", "")); // Virtual Tour URL Unbranded
            //    driver.Click(By.Id("m_lbSubmit"));
            //}

            if (media.Any(c => c is ResidentialListingMedia))
            {
                // Enter Image Management
                driver.Click(By.Id("m_lbManagePhotos"));

                //Prepare Media
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lbSave")));
                DeleteAllImages(driver);
                UploadNewImages(driver, media.OfType<ResidentialListingMedia>());
            }

            return UploadResult.Success;
        }

        private void DeleteAllImages(CoreWebDriver driver)
        {
            if (driver.FindElements(By.Id("cbxCheckAll")).Count() != 0)
            {
                driver.Click(By.Id("cbxCheckAll"));
                driver.Click(By.Id("m_lbDeleteChecked"));
                Thread.Sleep(1000);
                driver.SwitchTo().Alert().Accept();
            }

        }

        private void UploadNewImages(CoreWebDriver driver, IEnumerable<ResidentialListingMedia> media)
        {
            var js = (IJavaScriptExecutor)driver.InternalWebDriver;
            var i = 0;

            //Upload Images
            //foreach (var image in media.OrderBy(x => x.Order))
            foreach (var image in media)
            {
                // MQ-311 - Uploader - Convert PNG for NTREIS and ACTRIS
                if (!string.IsNullOrWhiteSpace(image.Extension) && (image.Extension.ToLower().Equals(".gif") || image.Extension.ToLower().Equals(".png")))
                {
                    image.Extension = ".jpg";
                    String[] elements = Regex.Split(image.PathOnDisk, ".");
                    if (elements.Length == 2)
                    {
                        var fileName = elements[0].ToString();
                        image.PathOnDisk = fileName + ".jpg";
                    }
                }

                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_ucImageLoader_m_tblImageLoader")));

                driver.FindElement(By.Id("m_ucImageLoader_m_tblImageLoader")).FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);

                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("photoCell_" + i)));

                //js.ExecuteScript("javascript:ManageMediaJS.showDetails(" + i + ");");
                js.ExecuteScript("jQuery('#photoCell_" + i + " > table > tbody > tr > td:nth-child(2) > #m_lbDetails')[0].click();");

                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DescriptionDiv")));

                var wait = driver.GetWait();
                var descriptionBtn = wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Enter description")));

                descriptionBtn.Click();
                driver.WriteTextbox(By.Id("m_tbxDescription"), image.Caption);
                driver.Click(By.LinkText("Done"));

                driver.Click(By.Id("m_ucDetailsView_m_btnSave"));

                i++;
            }
        }

        /// <summary>
        /// Updates a listing's price in the Waco MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <returns>The final status of the price update operation and whether it succeeded or not</returns>
        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Login(driver, listing);
            StartUpdate(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Price Change")));
            driver.Click(By.LinkText("Price Change"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_9")));
            driver.WriteTextbox(By.Id("Input_9"), listing.ListPrice);

            return UploadResult.Success;
        }

        /// <summary>
        /// Updates a listing's status in the Waco MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <returns>The final status of the status update operation and whether it succeeded or not</returns>
        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Login(driver, listing);
            StartUpdate(driver, listing);
            string linkText = string.Empty;
            var transformedStatus = TransformStatus(listing.ListStatus, ref linkText);
            if (!string.IsNullOrWhiteSpace(linkText))
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(linkText)));
                driver.Click(By.PartialLinkText(linkText));
                //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("footer_div")));
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("stickypush")));

                switch (transformedStatus)
                {
                    #region Sold
                    case "SLD":

                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_307")));

                        if (listing.ClosedDate != null)
                            driver.WriteTextbox(By.Id("Input_302"), listing.ClosedDate.Value.ToShortDateString()); //Sold Date
                        if (listing.PendingDate != null)
                            driver.WriteTextbox(By.Id("Input_593"), listing.PendingDate.Value.ToShortDateString()); //Contract Date

                        driver.WriteTextbox(By.Id("Input_303"), listing.SalesPrice); //Sold Price
                        driver.WriteTextbox(By.Id("Input_304"), listing.SellerPaid); //Seller Contribution

                        if (listing.SqFtTotal != null)
                            driver.WriteTextbox(By.Id("Input_104"), listing.SqFtTotal); //Sqft

                        driver.SetSelect(By.Id("Input_307"), "0"); //Third Party Assistance Program                    
                        driver.SetSelect(By.Id("Input_105"), listing.SqFtSource); //Sqft Source
                        driver.SetSelect(By.Id("Input_587"), listing.MFinancing); //1st Financing:
                        driver.WriteTextbox(By.Id("Input_337"), listing.MortgageCoSold); //Mortgage Company
                        driver.WriteTextbox(By.Id("Input_306"), listing.TitleCoSold); //Closing Title Company
                        driver.WriteTextbox(By.Id("Input_287"), listing.SellingAgentLicenseNum ?? "99999999"); //Selling Agent ID

                        driver.WriteTextbox(By.Id("Input_803"), listing.SellingAgent2ID); // Selling Agent 2 ID
                        driver.WriteTextbox(By.Id("Input_794"), listing.SellTeamID); // Sell Team ID
                        driver.WriteTextbox(By.Id("Input_801"), listing.SellingAgentSupervisor); // Selling Agent Supervisor

                        break;

                    #endregion

                    #region Pending
                    case "PND":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_263")));
                        if (listing.PendingDate != null)
                        {
                            driver.WriteTextbox(By.Id("Input_263"), listing.PendingDate.Value.ToShortDateString()); //Contract Date
                        }
                        if (listing.EstClosedDate != null)
                        {
                            driver.WriteTextbox(By.Id("Input_491"), listing.EstClosedDate.Value.ToShortDateString()); //Proposed Closed Date
                        }
                        break;
                    #endregion

                    #region Active
                    // Active
                    case "A":
                        var expirationDate1 = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_5")));
                        driver.WriteTextbox(By.Id("Input_5"), expirationDate1);
                        break;
                    #endregion

                    #region Active Contingent
                    // Active Contingent
                    case "AC":
                        //var expirationDate2 = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_291")));

                        driver.WriteTextbox(By.Id("Input_682"), listing.ContractDate); // Contract Date

                        driver.WriteTextbox(By.Id("Input_291"), listing.ContingencyInfo); // 	Contingency Info

                        break;
                    #endregion

                    #region Active
                    //Active Contingent
                    case "AKO":
                        //var expirationDate3 = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_291")));

                        // driver.WriteTextbox(By.Id("Input_682"), ""); // Contract Date

                        // driver.WriteTextbox(By.Id("Input_291"), ""); // 	Kick Out Information

                        break;
                    #endregion

                    #region Active Option Contract
                    // Active Option Contract
                    case "AOC":
                        //var expirationDate4 = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_263")));

                        driver.WriteTextbox(By.Id("Input_263"), listing.ContractDate); // Contract Date

                        //driver.WriteTextbox(By.Id("Input_328"), listing.ExpiredDateOption); // 	Option Expire Date

                        break;
                    #endregion

                    #region Cancelled

                    case "CAN":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_290")));
                        driver.WriteTextbox(By.Id("Input_290"), DateTime.Now.ToShortDateString());
                        break;
                    #endregion

                    #region Temp off Market
                    case "TOM":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_279")));
                        driver.WriteTextbox(By.Id("Input_279"), DateTime.Now.ToShortDateString()); // 	Temp Off Market Date
                        break;
                    #endregion

                    default:
                        throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for Waco Listing with Id '" + listing.ResidentialListingID + "'");
                }
            }

            return UploadResult.Success;
        }
    }
}
