using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

// ReSharper disable UnusedMember.Global

namespace Husa.Core.Uploaders.Dfw
{
    public partial class DfwUploader : IUploader,
                                        IEditor,
                                        IPriceUploader,
                                        IStatusUploader,
                                        IImageUploader,
                                        ICompletionDateUploader,
                                        IUpdateOpenHouseUploader,
                                        IUploadVirtualTourUploader,
                                        ILeaseUploader,
                                        IStatusLeaseUploader,
                                        ILotUploader,
                                        IStatusLotUploader

    {
        OpenHouseBase OH = new OpenHouseBase();

        /// <summary>
        /// Returns whether this uploader needs to have Adobe Flash functionality enabled. In DFW this always returns false.
        /// </summary>
        public bool IsFlashRequired { get { return false; } }

        /// <summary>
        /// Determines if a particular listing can be uploaded with the DFW Uploader
        /// </summary>
        /// <param name="listing">The listing to test for upload</param>
        /// <returns>True if the listing can be uploaded by the DFW Uploader, false if not.</returns>
        public bool CanUpload(ResidentialListingRequest listing)
        {
            return listing.MarketName == "DFW";
        }

        /// <summary>
        /// Inserts or updates a listing into the DFW MLS system.
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
            EditRoomsTab(driver, listing);
            EditFeaturesTab(driver, listing);
            EditLotUtilityEnvironmentTab(driver, listing);
            EditFinancialTab(driver, listing);
            EditShowingTab(driver, listing);
            EditRemarksTab(driver, listing);

            if (driver.UploadInformation.IsNewListing)
            {
                EditStatusTab(driver, listing);
            }

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
            // Remove all cookies
            driver.Manage().Cookies.DeleteAllCookies();

            driver.Navigate("http://matrix.ntreis.net/");
            Thread.Sleep(1500);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("loginbtn")));
            driver.ExecuteScript("document.getElementById('clareity').focus();");

            // Pacesetter Homes
            if (listing.SysOwnedBy == 1295)
            {
                listing.MarketUsername = "D110185";
                listing.MarketPassword = "Husa17a$";
            }
            
            char[] username = listing.MarketUsername.ToArray();

            foreach (var charact in username)
            {
                Thread.Sleep(200);
                driver.FindElement(By.Id("clareity")).SendKeys(charact.ToString());
            }

            driver.FindElement(By.Id("clareity")).SendKeys(Keys.Tab);

            char[] password = listing.MarketPassword.ToArray();

            foreach (var charact in password)
            {
                Thread.Sleep(200);
                driver.FindElement(By.Id("security")).SendKeys(charact.ToString());
            }

            driver.FindElement(By.Id("security")).SendKeys(Keys.Tab);

            driver.Click(By.Id("loginbtn"));

            driver.ExecuteScript(" $('.tour-backdrop').remove();$('#step-0').remove();");

            Thread.Sleep(1500);

            driver.Navigate("http://matrix.ntreis.net/");

            try
            {
                driver.Click(By.LinkText("Skip"), true);
            }
            catch { }

            Thread.Sleep(500);

            driver.Click(By.Id("NewsDetailDismiss"), true);

            return LoginResult.Logged;
        }

        public UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);

            Thread.Sleep(1000);
            driver.Navigate("http://matrix.ntreis.net/");
            Thread.Sleep(1000);
            try { driver.Click(By.LinkText("Skip"), true); } catch { }
            try { driver.Click(By.Id("NewsDetailDismiss"), true); } catch { }

            driver.Navigate("http://matrix.ntreis.net/Matrix/Input");
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.Click(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit"));
            driver.ScrollDown();
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Open Houses")));
            driver.Click(By.LinkText("Open Houses"));
            Thread.Sleep(1000);

            DeleteOpenHouses(driver, listing);

            //if (listing.EnableOpenHouse)
            //{
                //driver.Click(By.Id("Open Houses"));
                Thread.Sleep(2000);
                AddOpenHouses(driver, listing);
            //}

            return UploadResult.Success;
        }

        public void DeleteOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            int countDeleteButtons = driver.FindElements(By.LinkText("Delete")).Count();
            for (int i = 0; i < countDeleteButtons; i++)
            {
                try
                {
                    driver.ScrollToTop();
                    try { driver.ExecuteScript("Subforms['s_327'].deleteRow('_Input_327__del_REPEAT" + i + "_');"); } catch { }

                }
                catch { }
            }

            driver.ScrollDown();
            Thread.Sleep(2000);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_lbSubmit")));
            driver.Click(By.Id("m_lbSubmit"));
            Thread.Sleep(2000);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_lblInputCompletedMessage")));
            driver.Click(By.Id("m_lbContinueEdit"));
            Thread.Sleep(2000);
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

            List<ResidentialListingVirtualTour> virtualTour = media.OfType<ResidentialListingVirtualTour>().ToList();
            if (virtualTour != null && virtualTour.Count > 0)
            {
                driver.WriteTextbox(By.Id("Input_453"), virtualTour[0].VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded
                if(virtualTour.Count > 1)
                {
                    driver.WriteTextbox(By.Id("Input_454"), virtualTour[1].VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded
                }
            }

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
            int max = 9;

            List<DateTime> date = OH.getNextDate(listing, max);

            int i = 0;
            Thread.Sleep(1000);
            driver.ScrollToTop();

            // HCS-596
            String openHouseType = "Public";

            foreach (var local in date)
            {
                string[] openhousestart;
                string[] openhouseend;

                string day = local.DayOfWeek.ToString().Substring(0, 3);
                if (listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null) != null && listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null) != null)
                {
                    openhousestart = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.START, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);
                    openhouseend = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.END, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);

                    string elementId = "_Input_327__REPEAT" + i.ToString();

                    // 	Date
                    driver.WriteTextbox(By.Id(elementId + "_321"), local.ToString("MM/dd/yyyy"));

                    // Start Time
                    driver.WriteTextbox(By.Id(elementId + "_TextBox_322"), openhousestart[0]);
                    if (openhousestart[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id(elementId + "_RadioButtonList_322_0"), openhousestart[1]);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id(elementId + "_RadioButtonList_322_1"), openhousestart[1]);
                    }

                    // End Time
                    driver.WriteTextbox(By.Id(elementId + "_TextBox_323"), openhouseend[0]);
                    if (openhouseend[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id(elementId + "_RadioButtonList_323_0"), openhouseend[1]);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id(elementId + "_RadioButtonList_323_1"), openhouseend[1]);
                    }

                    // Active
                    driver.SetSelect(By.Id(elementId + "_324"), "1");

                    // Open House Type
                    driver.SetSelect(By.Id(elementId + "_320"), openHouseType, false);

                    // Comments
                    if (listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null) != null)
                    {
                        driver.WriteTextbox(By.Id(elementId + "_326"), listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null));
                    }

                    i++;
                    int countButtons = driver.FindElements(By.LinkText("Delete")).Count();
                    if (max >= (i + 1) && (countButtons < (i + 1)))
                    {
                        try
                        {
                            if (driver.FindElements(By.Id("_Input_337__REPEAT" + i + "_320")).Count() <= 0)
                            {
                                driver.ScrollDown();
                                driver.Click(By.LinkText("More"));
                            }
                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }

                    driver.ScrollDown();
                }
            }

            // removing extra data
            for (int j = 0; j < 10; j++)
            {
                driver.ScrollDown();
                string elementId = "_Input_327__REPEAT" + i.ToString();
                try
                {
                    if (driver.FindElements(By.Id(elementId + "_320")).Count() > 0)
                    {
                        string js = "javascript: Subforms['s_327'].deleteRow('_Input_327__del_REPEAT" + i.ToString() + "_');";
                        ((IJavaScriptExecutor)driver).ExecuteScript(@js);

                    }
                }
                catch { }
                i++;
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
            driver.Navigate("http://matrix.ntreis.net/Matrix/Logout.aspx");
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
                    linkText = "Change to Active Option Contract";
                    return "AOC";
                case "C":
                    linkText = "Change to Cancelled";
                    return "CAN";
                case "X":
                    return "EXP";
                case "P":
                    linkText = "Change to Pending";
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
                    throw new ArgumentOutOfRangeException("status", status, @"The status '" + status + @"' is not configured for DFW");
            }
        }

        private static string TransformLeaseStatus(string status, ref string linkText)
        {
            switch (status)
            {
                case "A":
                    linkText = "Change to Active";
                    return "A";
                case "C":
                    linkText = "Change to Cancelled";
                    return "CAN";
                case "P":
                    linkText = "Change to Pending";
                    return "PND";
                case "L":
                    linkText = "Change to Leased";
                    return "SLD";
                case "T":
                    linkText = "Change to Temp Off Market";
                    return "TOM";
                default:
                    throw new ArgumentOutOfRangeException("status", status, @"The status '" + status + @"' is not configured for DFW");
            }
        }

        #region Upload Code

        private void StartInsert(CoreWebDriver driver)
        {
            driver.Navigate("http://matrix.ntreis.net/Matrix/Input");

            driver.Click(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbAddNewItem"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_dlInputList_ctl00_m_btnSelect")));
            driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")));
            driver.Click(By.Id("m_rpFillFromList_ctl03_m_lbPageLink"));
        }

        private void StartInsertLeasing(CoreWebDriver driver)
        {
            driver.Navigate("http://matrix.ntreis.net/Matrix/Input");

            driver.Click(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbAddNewItem"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_dlInputList_ctl04_m_btnSelect")));
            driver.Click(By.Id("m_dlInputList_ctl04_m_btnSelect"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")));
            driver.Click(By.Id("m_rpFillFromList_ctl03_m_lbPageLink"));
        }

        private void StartInsertLot(CoreWebDriver driver)
        {
            driver.Navigate("http://matrix.ntreis.net/Matrix/Input");

            driver.Click(By.Id("m_lvInputUISections_ctrl0_lvInputUISubsections_ctrl0_lbAddNewItem"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_dlInputList_ctl02_m_btnSelect")));
            driver.Click(By.Id("m_dlInputList_ctl02_m_btnSelect"));
            // wait for link to be displayed
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")));
            driver.Click(By.Id("m_rpFillFromList_ctl03_m_lbPageLink"));
        }

        private void StartUpdate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Navigate("http://matrix.ntreis.net/Matrix/Input");
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.Click(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit"));
        }

        private void StartLeaseUpdate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Navigate("http://matrix.ntreis.net/Matrix/Input");
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.Click(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit"));
        }

        private void EditLeasingGeneralTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.SetSelect(By.Id("Input_462"), "LSEHOU"); // Property Type
            driver.SetMultipleCheckboxById("Input_90", listing.HousingTypeDesc); //Housing Type
            driver.SetMultipleCheckboxById("Input_103", listing.HousingStyleDesc); //Style of Houses
            driver.WriteTextbox(By.Id("Input_73"), listing.ListPrice); // List Price
            driver.SetSelect(By.Id("Input_92"), "EXCAGE"); // Listing Type
            //driver.SetSelect(By.Id("Input_461"), listing.TransactionType); // Transaction Type
            driver.WriteTextbox(By.Id("Input_308"), ""); // For Sale MLS#
            driver.WriteTextbox(By.Id("Input_463"), listing.Date); // Date Available
            // driver.WriteTextbox(By.Id("Input_467"), listing.);  // Days Guests Allowed
            driver.WriteTextbox(By.Id("Input_475"), listing.NumberOfVehicles); // # of Vehicles
            driver.SetSelect(By.Id("Input_465"), listing.AppliancesYN); // Appliances
            driver.WriteTextbox(By.Id("Input_468"), listing.NumberOfPetsAllowed); // # Pets Allowed
            driver.SetSelect(By.Id("Input_464"), listing.Furnished); // Furnished
            driver.SetSelect(By.Id("Input_113"), listing.YearBuiltDesc); //Construction Status
            driver.SetMultipleCheckboxById("Input_112", listing.ConstructionDesc); // Construction
            if (driver.UploadInformation.IsNewListing)
            {
                DateTime? listDate = null;
                switch (listing.LeaseStatus)
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

            driver.WriteTextbox(By.Id("Input_100"), listing.YearBuiltLease); // Year Built
            if (listing.SqFtTotal != null)
                driver.WriteTextbox(By.Id("Input_104"), listing.SqFtTotal); // SqFt

            driver.SetSelect(By.Id("Input_105"), listing.SqFtSource); // SqFt Source
            driver.WriteTextbox(By.Id("Input_75"), string.IsNullOrWhiteSpace(listing.TaxID) ? "NA" : listing.TaxID); // Parcel ID
            //driver.WriteTextbox(By.Name("Input_738"), ); // Additional Parcel ID
            driver.SetSelect(By.Id("Input_72"), listing.IsMultiParcel); // Multi Parcel ID
            driver.WriteTextbox(By.Name("Input_470"), listing.DepositPet); // Pet Deposit
            driver.SetMultipleCheckboxById("Input_802", listing.PetPolicy); // Pet Policy
            driver.WriteTextbox(By.Name("Input_723"), listing.FloorLocationNumber);  // Floor Location
            driver.SetSelect(By.Id("Input_533"), listing.CompensationPaid); // When Compensation Pd
            driver.SetSelect(By.Id("Input_546"), "NO"); // Will Subdivide
            driver.WriteTextbox(By.Name("Input_534"), listing.LeaseTerms);  // Lease Terms
            driver.WriteTextbox(By.Name("Input_472"), listing.MonthlyPetFee); // Monthly Pet Fee
            driver.WriteTextbox(By.Name("Input_744"), listing.AppFeeAmount); // Application Fee Amount
            driver.WriteTextbox(By.Name("Input_474"), listing.DepositAmount); // Deposit Amount
            driver.SetSelect(By.Id("Input_473"), listing.NonRefunPetFee); // Non-Refund Pet Fee
            driver.WriteTextbox(By.Name("Input_741"), listing.ApplicationFeePay); // Application Fee Payable To
            driver.SetSelect(By.Id("Input_742"), "1"); // Application Fee Per Person 18 + years

            #region Location Information

            if (driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_58"), listing.StreetNum); // Street/Box Number
                driver.SetSelect(By.Id("Input_76"), listing.County); // County
                driver.SetSelect(By.Id("Input_59"), listing.StreetDir); // Street Direction
                driver.WriteTextbox(By.Id("Input_60"), listing.StreetName.Replace('\'', ' ')); // Street Direction

                if (!string.IsNullOrWhiteSpace(listing.StreetType))
                {
                    driver.SetSelect(By.Id("Input_61"), listing.StreetType); // Street Type
                }
            }

            driver.WriteTextbox(By.Id("Input_134"), !String.IsNullOrEmpty(listing.LotNum) ? listing.LotNum : ""); // Lot
            driver.WriteTextbox(By.Id("Input_56"), !String.IsNullOrEmpty(listing.Block) ? listing.Block : ""); // Block
            driver.SetSelect(By.Id("Input_64"), listing.CityCode); // City

            driver.SetSelect(By.Id("Input_77"), listing.MLSArea); // Area
            driver.SetSelect(By.Id("Input_78"), listing.MLSSubArea); // Sub Area

            driver.WriteTextbox(By.Id("Input_54"), listing.Subdivision); // Subdivision
            driver.WriteTextbox(By.Id("Input_55"), listing.PlannedDevelopment); // Planned Development
            driver.WriteTextbox(By.Id("Input_91"), listing.Legal); // Additional Legal

            driver.SetSelect(By.Id("Input_62"), listing.StreetDir); // Street Directional Suffix 

            driver.WriteTextbox(By.Id("Input_63"), listing.UnitNum); // Unit #
            driver.WriteTextbox(By.Id("Input_66"), listing.Zip); // Zip
            driver.WriteTextbox(By.Id("Input_67"), string.Empty); // Zip + 4

            driver.WriteTextbox(By.Id("Input_67"), listing.NumLakes); //Lake Name

            SetLongitudeAndLatitudeValues(driver, listing);
            #endregion

            #region School Information
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_99"), listing.SchoolDistrict); // School District
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_96"), listing.SchoolName1); // Elementary School
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_97"), listing.SchoolName2); // Middle School
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_98"), listing.SchoolName3); // High School 
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_458"), listing.SchoolName7); // Intermediate School 
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_459"), listing.SchoolName5); // Junior School 
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_592"), listing.SchoolName6); // Senior High School 
            #endregion
        }

        private void EditGeneralTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_rpPageList_ctl00_lbPageLink")));
            driver.Click(By.Id("m_rpPageList_ctl00_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

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
            driver.SetSelect(By.Id("Input_810"), (listing.SeniorCommunity != null && listing.SeniorCommunity != "NONE") ? listing.SeniorCommunity : ""); // Senior Community
            driver.WriteTextbox(By.Id("Input_73"), listing.ListPrice); // List Price

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
                    case "CS":
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

            #region Location Information

            if (driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_58"), listing.StreetNum); // Street/Box Number
                driver.SetSelect(By.Id("Input_76"), listing.County); // County
                driver.SetSelect(By.Id("Input_59"), listing.StreetDir); // Street Direction
                driver.WriteTextbox(By.Id("Input_60"), listing.StreetName.Replace('\'', ' ')); // Street Direction

                if (!string.IsNullOrWhiteSpace(listing.StreetType))
                {
                    driver.SetSelect(By.Id("Input_61"), listing.StreetType); // Street Type
                }
            }

            driver.WriteTextbox(By.Id("Input_134"), !String.IsNullOrEmpty(listing.LotNum) ? listing.LotNum : ""); // Lot
            driver.WriteTextbox(By.Id("Input_56"), !String.IsNullOrEmpty(listing.Block) ? listing.Block : ""); // Block
            driver.SetSelect(By.Id("Input_64"), listing.CityCode); // City

            driver.SetSelect(By.Id("Input_77"), listing.MLSArea); // Area
            driver.SetSelect(By.Id("Input_78"), listing.MLSSubArea); // Sub Area

            driver.WriteTextbox(By.Id("Input_54"), listing.Subdivision); // Subdivision
            driver.WriteTextbox(By.Id("Input_55"), listing.PlannedDevelopment); // Planned Development
            driver.WriteTextbox(By.Id("Input_91"), listing.Legal); // Additional Legal

            driver.WriteTextbox(By.Id("Input_62"), listing.StreetDir); // Street Directional Suffix 

            driver.WriteTextbox(By.Id("Input_63"), listing.UnitNum); // Unit #
            driver.WriteTextbox(By.Id("Input_66"), listing.Zip); // Zip
            driver.WriteTextbox(By.Id("Input_67"), string.Empty); // Zip + 4

            driver.WriteTextbox(By.Id("Input_67"), listing.NumLakes); //Lake Name

            //driver.WriteTextbox(By.Id("INPUT_206_93"), listing.Latitude); // Latitude
            //driver.WriteTextbox(By.Id("INPUT_206_94"), listing.Longitude); // Longitude

            SetLongitudeAndLatitudeValues(driver, listing);

            #endregion

            #region School Information

            Thread.Sleep(500);

            driver.SetSelect(By.Id("Input_99"), listing.SchoolDistrict); // School District

            Thread.Sleep(500);

            driver.SetSelect(By.Id("Input_96"), listing.SchoolName1); // Elementary School

            Thread.Sleep(500);

            driver.SetSelect(By.Id("Input_97"), listing.SchoolName2); // Middle School

            Thread.Sleep(500);

            driver.SetSelect(By.Id("Input_98"), listing.SchoolName3); // High School 

            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_458"), listing.SchoolName7); // Intermediate School 

            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_459"), listing.SchoolName5); // Junior School 

            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_592"), listing.SchoolName6); // Senior High School 

            #endregion
        }

        private void EditRoomsTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink")); // Tab: Input | Subtab: Rooms
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer"))); // Look if the footer elements has been loaded
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Id("Input_254"), listing.Beds); // Bedrooms
            driver.WriteTextbox(By.Id("Input_256"), listing.BathsFull); // Full Baths
            driver.WriteTextbox(By.Id("Input_255"), listing.BathsHalf); // Half Baths

            driver.WriteTextbox(By.Id("Input_260"), listing.NumStories); // # Stories

            driver.WriteTextbox(By.Id("Input_258"), listing.NumLivingAreas); // # Living Areas

            driver.WriteTextbox(By.Id("Input_259"), listing.NumDiningAreas); // # Dining Areas

            if (!driver.UploadInformation.IsNewListing)
            {
                var elems = driver.FindElements(By.CssSelector("table[id^=_Input_253__del_REPEAT] a"));
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
                        driver.ExecuteScript("Subforms['s_253'].deleteRow('_Input_253__del_REPEAT" + index + "_');");
                        Thread.Sleep(400);
                        continue;
                    }
                }
            }

            var roomTypes = ReadRoomAndFeatures(listing);

            driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink"));
            Thread.Sleep(400);

            var i = 0;

            foreach (var roomType in roomTypes.Where(c => c.IsValid()))
            {
                if (i > 0)
                {
                    driver.Click(By.Id("_Input_253_more"));
                    Thread.Sleep(400);
                }

                driver.SetSelect(By.Id("_Input_253__REPEAT" + i + "_249"), roomType.Value, true); // FieldName
                Thread.Sleep(400);
                driver.ScrollDown();
                driver.SetSelect(By.Id("_Input_253__REPEAT" + i + "_309"), roomType.Level, true);
                Thread.Sleep(400);
                driver.ScrollDown();
                driver.WriteTextbox(By.Id("_Input_253__REPEAT" + i + "_250"), roomType.Length, true);
                Thread.Sleep(400);
                driver.ScrollDown();
                driver.WriteTextbox(By.Id("_Input_253__REPEAT" + i + "_251"), roomType.Width, true);
                Thread.Sleep(400);
                driver.ScrollDown();
                driver.SetMultipleCheckboxById("_Input_253__REPEAT" + i + "_252", roomType.Features);
                Thread.Sleep(400);
                driver.ScrollDown();

                i++;
            }
        }

        private void EditFeaturesLeasingTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl04_lbPageLink"));
            Thread.Sleep(1000);

            driver.SetMultipleCheckboxById("Input_476", listing.LeaseConditions); // Lease Requirements
            driver.SetMultipleCheckboxById("Input_532", listing.LeaseType); // Lease Type
            driver.SetMultipleCheckboxById("Input_477", listing.MoniesRequired);  // Monies Required
            driver.SetMultipleCheckboxById("Input_149", listing.GarageDesc); // Parking Features
            driver.SetSelect(By.Id("Input_748"), listing.SMARTFEATURESAPP); // Smart Home Features-App or Password Dependent
            driver.SetSelect(By.Id("Input_122"), listing.HasSecuritySys); //Alarm/Security Y/N
            driver.SetSelect(By.Id("Input_117"), listing.HasHandicapAmenities); //Handicap Y/N
            driver.WriteTextbox(By.Id("Input_110"), listing.NumFireplaces); // # Fireplaces
            driver.WriteTextbox(By.Id("Input_108"), listing.CarportCapacity); // # Carport Spaces
            driver.SetMultipleCheckboxById("Input_123", listing.SecurityDesc); // Alarm/Security Type
            driver.SetMultipleCheckboxById("Input_118", listing.HandicapDesc); // Handicap Amenities
            driver.SetMultipleCheckboxById("Input_148", listing.FireplaceDesc); // Fireplaces Features
            driver.WriteTextbox(By.Id("Input_109"), listing.GarageCapacity); // # Garage Spaces
            driver.WriteTextbox(By.Id("Input_146"), listing.GarageLength); // Garage Length
            driver.WriteTextbox(By.Id("Input_147"), listing.GarageWidth); // Garage Width
            driver.WriteTextbox(By.Id("Input_150"), listing.GarageCapacity); // Total Covered Parking
            driver.SetMultipleCheckboxById("Input_107", listing.InteriorDesc); // Interior Features
            driver.SetMultipleCheckboxById("Input_115", listing.FloorsDesc); // Flooring
            driver.SetMultipleCheckboxById("Input_478", listing.TenantPays);  // Tenant Expenses
            driver.SetMultipleCheckboxById("Input_203", ""); //Special Notes 
            driver.SetMultipleCheckboxById("Input_531", listing.KitchenEquipmentDesc); // Kitchen Equipment
            driver.SetSelect(By.Id("Input_125"), listing.HasPool == "Y" ? "1" : "0"); //Pool on Property
            driver.SetMultipleCheckboxById("Input_126", listing.PoolDesc); // Pool Features
            driver.SetMultipleCheckboxById("Input_111", listing.CommonFeatures); // Common Features
        }

        private void EditFeaturesTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl04_lbPageLink"));
            Thread.Sleep(1000);

            driver.SetMultipleCheckboxById("Input_107", listing.InteriorDesc); // Interior Features
            driver.WriteTextbox(By.Id("Input_108"), listing.CarportCapacity); // # Carport Spaces
            driver.WriteTextbox(By.Id("Input_109"), listing.GarageCapacity); // # Garage Spaces
            driver.WriteTextbox(By.Id("Input_146"), listing.GarageLength); // Garage Length
            driver.WriteTextbox(By.Id("Input_147"), listing.GarageWidth); // Garage Width

            driver.WriteTextbox(By.Id("Input_150"), listing.GarageCapacity); // Total Parking = Garage Spaces (MQ-313)
            driver.WriteTextbox(By.Id("Input_110"), listing.NumFireplaces); // # Fireplaces

            driver.SetSelect(By.Id("Input_125"), listing.HasPool == "Y" ? "1" : "0"); //Pool on Property
            driver.SetSelect(By.Id("Input_122"), listing.HasSecuritySys); //Alarm/Security Y/N
            driver.SetSelect(By.Id("Input_117"), listing.HasHandicapAmenities); //Handicap Y/N
            driver.SetMultipleCheckboxById("Input_126", listing.PoolDesc); // Pool Features
            driver.SetMultipleCheckboxById("Input_149", listing.GarageDesc); // Parking Features

            driver.ScrollDown(250);
            driver.SetMultipleCheckboxById("Input_123", listing.SecurityDesc); // Alarm/Security Type
            driver.SetMultipleCheckboxById("Input_148", listing.FireplaceDesc); // Fireplaces Features
            driver.SetMultipleCheckboxById("Input_111", listing.CommonFeatures); // Common Features
            driver.SetMultipleCheckboxById("Input_118", listing.HandicapDesc); // Handicap Amenities

            driver.ScrollDown(350);
            driver.SetMultipleCheckboxById("Input_124", listing.RoofDesc); // Roof
            driver.SetMultipleCheckboxById("Input_115", listing.FloorsDesc); // Flooring
            driver.SetMultipleCheckboxById("Input_116", listing.FoundationDesc); // Foundation
            driver.SetMultipleCheckboxById("Input_203", ""); //Special Notes 

            Thread.Sleep(400);

            driver.ScrollDown(450);
            driver.SetMultipleCheckboxById("Input_531", listing.KitchenEquipmentDesc); // Kitchen Equipment
            driver.SetSelect(By.Id("Input_748"), listing.SMARTFEATURESAPP); // Smart Home Features-App or Password Dependent
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
        private void EditFinancialLeasingTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl08_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.SetSelect(By.Id("Input_207"), listing.HOA); // HOA
            driver.WriteTextbox(By.Id("Input_676"), listing.AssocName); // HOA Management Co
            driver.WriteTextbox(By.Id("Input_677"), listing.AssocPhone); // HOA Managemt Co Phone	
            driver.SetMultipleCheckboxById("Input_156", listing.AssocFeeIncludes); // HOA Includes
        }

        private void EditFinancialTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl08_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.SetMultipleCheckboxById("Input_167", listing.FinancingProposed); // Proposed Financing

            driver.SetSelect(By.Id("Input_207"), listing.HOA); // HOA
            driver.SetSelect(By.Id("Input_155"), listing.AssocFeePaid); // HOA Billing Freq
            driver.WriteTextbox(By.Id("Input_170"), listing.AssocFee); // HOA Dues
            driver.WriteTextbox(By.Id("Input_670"), listing.AssocName); // HOA Management Co
            driver.WriteTextbox(By.Id("Input_671"), listing.AssocPhone); // HOA Managemt Co Phone	

            driver.SetMultipleCheckboxById("Input_161", "CLOFUN"); // Posession
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

            if(driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_175"), listing.BrokerLicenseNum);  // List Agent MLS ID
                driver.ExecuteScript("javascript:document.getElementById('Input_175_Refresh').value='1';RefreshToSamePage();");

                Thread.Sleep(1000);

                driver.WriteTextbox(By.Id("Input_538"), "Ben Caballero"); // Office Supervisor

                driver.WriteTextbox(By.Id("Input_739"), listing.BrokerLicenseNum); // Office Supervisor License #
            }

            driver.ScrollDown();
            driver.SetSelect(By.Id("Input_201"), "0"); // Variable Fee

            if (!String.IsNullOrWhiteSpace(listing.CompBuy))
                driver.WriteTextbox(By.Id("Input_191"), listing.CompBuy);
            else
                driver.WriteTextbox(By.Id("Input_191"), "3%"); // Buyers Agency Commission

            driver.WriteTextbox(By.Id("Input_188"), "0%"); // SubAgency Commission
            driver.SetSelect(By.Id("Input_119"), "NONE"); // Keybox Type
            driver.WriteTextbox(By.Id("Input_185"), "0"); // Keybox #

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

            string message = listing.ShowingInstructions.RemoveSlash() + realtorContactEmail;

            string builtNote = "";
            if (listing.YearBuiltDesc == "NCI" && 
                !message.Contains("Home is under construction. For your safety, call appt number for showings") )
            {
                builtNote = "Home is under construction. For your safety, call appt number for showings. ";
            }

            driver.WriteTextbox(By.Id("Input_204"), builtNote + message); // Showing Instructions

            var apptPhone = listing.AgentListApptPhone;
            driver.WriteTextbox(By.Id("Input_189"), (!string.IsNullOrEmpty(apptPhone) ? apptPhone : "")); //  Appt Phone
        }

        private void EditShowingLeasingTab(CoreWebDriver driver, ResidentialListingRequest listing)
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

            // Buyers Agency Commission
            /* MLS-45 CompBuy Deprecated
            if (!String.IsNullOrWhiteSpace(listing.CompBuy))
            {
                driver.WriteTextbox(By.Id("Input_191"), "%" + listing.CompBuy);
            } 
            */
            if (!String.IsNullOrWhiteSpace(listing.CommissionLease))
            {
                string[] items = new string[2] { "$", "%" };
                if (items.Any(i => listing.CommissionLease.Contains(i)))
                {
                    driver.WriteTextbox(By.Id("Input_191"), listing.CommissionLease);
                }
            } 
            else 
            {
                driver.WriteTextbox(By.Id("Input_191"), ""); 
            }

            driver.WriteTextbox(By.Id("Input_188"), "0%"); // SubAgency Commission
            driver.SetSelect(By.Id("Input_119"), "NONE"); // Keybox Type
            driver.WriteTextbox(By.Id("Input_185"), "0"); // Keybox #

            //UP-190
            if (String.IsNullOrEmpty(listing.AltPhoneCommunity))
                driver.WriteTextbox(By.Id("Input_196"), "");
            else
                driver.WriteTextbox(By.Id("Input_196"), listing.AltPhoneCommunity);
            driver.SetSelect(By.Id("Input_200"), "BUILDE"); // Seller Type

            driver.WriteTextbox(By.Id("Input_198"), listing.OwnerName); // Owner Name
            driver.SetMultiSelect(By.Id("Input_679"), listing.Showing); //Showing

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
        }

        private void EditStatusTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl16_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            
            driver.SetSelect(By.Id("Input_49"), listing.ListStatus); // Status
        }

        private void EditRemarksTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl12_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.SetSelect(By.Id("Input_85"), "1"); // Allow Address Display
            driver.SetSelect(By.Id("Input_86"), "1"); // Allow AVM
            driver.SetSelect(By.Id("Input_87"), "1"); // Allow Internet Display
            driver.SetSelect(By.Id("Input_88"), "1"); // Allow Comments/Reviews

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

            // BEGIN UP-73
            String realtorContactEmail = String.Empty;
            if (!String.IsNullOrEmpty(listing.EmailRealtorsContact))
                realtorContactEmail = listing.EmailRealtorsContact;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmail))
                realtorContactEmail = listing.RealtorContactEmail;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmailFromCommunityProfile))
                realtorContactEmail = listing.RealtorContactEmailFromCommunityProfile;
            // END UP-73

            //UP-78
            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";

            driver.WriteTextbox(By.Id("Input_80"), "");
            driver.WriteTextbox(By.Id("Input_80"), bonusMessage + listing.GetPrivateRemarks(false) + realtorContactEmail);

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

            driver.WriteTextbox(By.Id("Input_81"), direction); //Allow Comments/Reviews

            #endregion

            #region Public Remark

            UpdatePublicRemarksInRemarksTab(driver, listing);

            #endregion

            #region Intra Office

            string compSaleText = String.Empty;

            if ((listing.ListStatus == "PND" || listing.ListStatus == "SLD") && driver.UploadInformation.IsNewListing)
            {
                compSaleText = "M-";
            }

            if (driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_82"), (!string.IsNullOrWhiteSpace(listing.PlanProfileName) ? (compSaleText + listing.PlanProfileName) : (compSaleText + " ")).RemoveSlash());
            }

            #endregion

            #region Excludes

            driver.WriteTextbox(By.Id("Input_114"), listing.Excludes.RemoveSlash()); // Property Description

            #endregion
        }

        private void EditRemarksLeaseTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl12_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.SetSelect(By.Id("Input_85"), "1"); // Allow Address Display
            driver.SetSelect(By.Id("Input_86"), "1"); // Allow AVM
            driver.SetSelect(By.Id("Input_87"), "1"); // Allow Internet Display
            driver.SetSelect(By.Id("Input_88"), "1"); // Allow Comments/Reviews

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
            if (!String.IsNullOrEmpty(listing.ContactEmailFromCompany))
                realtorContactEmail = listing.ContactEmailFromCompany;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmail))
                realtorContactEmail = listing.RealtorContactEmail;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmailFromCommunityProfile))
                realtorContactEmail = listing.RealtorContactEmailFromCommunityProfile;
            // END UP-73
            //UP-78
            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";

            driver.WriteTextbox(By.Id("Input_80"), "");
            driver.WriteTextbox(By.Id("Input_80"), bonusMessage + listing.GetPrivateRemarks(false) + realtorContactEmail);

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

            driver.WriteTextbox(By.Id("Input_81"), direction); //Allow Comments/Reviews

            #endregion

            #region Public Remark

            UpdatePublicRemarksInRemarksLeasingTab(driver, listing);

            #endregion

            #region Intra Office

            string compSaleText = String.Empty;

            if ((listing.ListStatus == "PND" || listing.ListStatus == "SLD") && driver.UploadInformation.IsNewListing)
            {
                compSaleText = "M-";
            }

            if (driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_82"), (!string.IsNullOrWhiteSpace(listing.PlanProfileName) ? (compSaleText + listing.PlanProfileName) : (compSaleText + " ")).RemoveSlash());
            }

            #endregion

            #region Excludes

            driver.WriteTextbox(By.Id("Input_114"), listing.Excludes.RemoveSlash()); // Property Description

            #endregion
        }

        private void UpdateConstructionStatusInGeneralTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_113")));
            driver.SetSelect(By.Id("Input_113"), listing.YearBuiltDesc); //Construction Status
            driver.WriteTextbox(By.Id("Input_100"), listing.YearBuilt); // Year Built
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

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_83")));
            driver.WriteTextbox(By.Id("Input_83"), listing.GetPublicRemarks(status)); // Property Description
        }

        private void UpdatePublicRemarksInRemarksLeasingTab(CoreWebDriver driver, ResidentialListingRequest listing)
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

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_83")));
            driver.WriteTextbox(By.Id("Input_83"), listing.GetPublicRemarksLeasing(status)); // Property Description
        }

        #region Rooms Tab

        private IEnumerable<RoomType> ReadRoomAndFeatures(ResidentialListingRequest listing)
        {
            return new List<RoomType>()
            {
                #region Living
                new RoomType("LIVROO", listing.LivingRoom1Level, listing.LivingRoom1Length, listing.LivingRoom1Width, null),
                new RoomType("LIVROO", listing.LivingRoom2Level, listing.LivingRoom2Length, listing.LivingRoom2Width, null),
                new RoomType("GAMROO", listing.LivingRoom3Level, listing.LivingRoom3Length, listing.LivingRoom3Width, null),
                #endregion

                #region Dining
                new RoomType("DINROO", listing.DiningRoomLevel, listing.DiningRoomLength, listing.DiningRoomWidth, null),
                #endregion

                #region Breakfast
                new RoomType("BREROO", listing.BreakfastLevel, listing.BreakfastLength, listing.BreakfastWidth, listing.OtherRoomDesc),
                #endregion

                #region Kitchen
                new RoomType("KITCHE", listing.KitchenLevel, listing.KitchenLength, listing.KitchenWidth,listing.KitchenDesc),
                #endregion

                #region Master Bedroom
                new RoomType("MASBED", listing.Bed1Level, listing.Bed1Length, listing.Bed1Width, listing.BedBathDesc),
                #endregion

                #region Bedroom
                new RoomType("BEDROO", listing.Bed2Level, listing.Bed2Length, listing.Bed2Width, null),
                new RoomType("BEDROO", listing.Bed3Level, listing.Bed3Length, listing.Bed3Width, null),
                new RoomType("BEDROO", listing.Bed4Level, listing.Bed4Length, listing.Bed4Width, null),
                new RoomType("BEDROO", listing.Bed5Level, listing.Bed5Length, listing.Bed5Width, null),
                #endregion

                #region Study
                new RoomType("STUDEN", listing.StudyLevel, listing.StudyLength, listing.StudyWidth, null),
                #endregion

                #region Utility
                new RoomType("UTIROO", listing.UtilityRoomLevel, listing.UtilityRoomLength, listing.UtilityRoomWidth,listing.UtilityRoomDesc),
                #endregion

                #region Other
                new RoomType("MEDROO", listing.OtherRoom1Level, listing.OtherRoom1Length, listing.OtherRoom1Width, null),
                new RoomType("OTHER", listing.OtherRoom2Level, listing.OtherRoom2Length, listing.OtherRoom2Width, null)
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
                return !string.IsNullOrWhiteSpace(Level) && !string.IsNullOrWhiteSpace(Length);
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
            if ((listing.IsForLease == "Yes" && String.IsNullOrEmpty(listing.MLSNum)) || String.IsNullOrEmpty(listing.MLSNum))
            {
                driver.WriteTextbox(By.Id("INPUT__93"), listing.Latitude); // Latitude
                driver.WriteTextbox(By.Id("INPUT__94"), listing.Longitude); // Longitude
            }
        }

        #endregion

        /// <summary>
        /// Updates a listing's completion date in the DFW MLS system.
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
            driver.Click(By.Id("m_rpPageList_ctl12_lbPageLink"));
            UpdatePublicRemarksInRemarksTab(driver, listing);

            return UploadResult.Success;
        }

        /// <summary>
        /// Updates a listing's images in the DFW MLS system.
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
                Thread.Sleep(3000);
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
                string descriptionFieldId = js.ExecuteScript("return jQuery('#photoCell_" + i + " > div > table > tbody > tr:nth-child(3) > td > textarea').attr('id');").ToString();

                //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DescriptionDiv")));

                //var wait = driver.GetWait();
                //var descriptionBtn = wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Enter description")));

                //descriptionBtn.Click();
                driver.WriteTextbox(By.Id(descriptionFieldId), image.Caption);
                driver.FindElement(By.Id(descriptionFieldId)).SendKeys(Keys.Enter);

                //driver.Click(By.LinkText("Done"));

                //driver.Click(By.Id("m_ucDetailsView_m_btnSave"));

                i++;
            }
        }

        /// <summary>
        /// Updates a listing's price in the DFW MLS system.
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
        /// Updates a listing's status in the DFW MLS system.
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

                        driver.WriteTextbox(By.Id("Input_803"), listing.SellingAgent2UID); // Selling Agent 2 ID
                        driver.WriteTextbox(By.Id("Input_794"), listing.SellTeamID); // Sell Team ID
                        driver.WriteTextbox(By.Id("Input_801"), listing.SellingAgentSupervisor); // Selling Agent Supervisor

                        break;

                    #endregion

                    #region Pending
                    case "PND":
                        var pendingDate = listing.PendingDate == null ? string.Empty : listing.PendingDate.Value.ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_285")));
                        driver.WriteTextbox(By.Id("Input_285"), pendingDate); //Contract Date
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
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_328")));

                        driver.WriteTextbox(By.Id("Input_682"), listing.ContractDate); // Contract Date

                        driver.WriteTextbox(By.Id("Input_328"), listing.ExpiredDateOption); // 	Option Expire Date

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
                        if(listing.OffMarketDate != null)
                        {
                            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_312")));
                            driver.WriteTextbox(By.Id("Input_312"), listing.OffMarketDate);
                        }
                        break;
                    #endregion

                    default:
                        throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for DFW Listing with Id '" + listing.ResidentialListingID + "'");
                }
            }

            return UploadResult.Success;
        }

        public UploadResult UpdateLease(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            if (driver.UploadInformation.IsNewListing)
            {
                StartInsertLeasing(driver);
            }
            else
            {
                StartUpdate(driver, listing);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
                driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect")); // Tab: Input | Option: Residential
            }

            EditLeasingGeneralTab(driver, listing);
            EditRoomsTab(driver, listing);
            EditFeaturesLeasingTab(driver, listing);
            EditLotUtilityEnvironmentTab(driver, listing);
            EditFinancialLeasingTab(driver, listing);
            EditShowingLeasingTab(driver, listing);
            EditRemarksLeaseTab(driver, listing);

            return UploadResult.Success;
        }

        public UploadResult UpdateStatusLease(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Login(driver, listing);
            StartLeaseUpdate(driver, listing);
            string linkText = string.Empty;
            var transformedStatus = TransformLeaseStatus(listing.ListStatus, ref linkText);
            
            
            if (!string.IsNullOrWhiteSpace(linkText))
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(linkText)));
                driver.Click(By.PartialLinkText(linkText));
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("stickypush")));

                switch (transformedStatus)
                {
                    // Sold
                    case "SLD":

                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_794")));

                        if(listing.LeasedDate != null)
                            driver.WriteTextbox(By.Id("Input_298"), listing.LeasedDate.Value.ToShortDateString()); // Leased Date

                        driver.WriteTextbox(By.Id("Input_303"), listing.LeasedPrice); // Leased Price
                        if(listing.MoveInDate != null)
                            driver.WriteTextbox(By.Id("Input_641"), listing.MoveInDate.Value.ToShortDateString()); // Move in Date

                        if (listing.SqFtTotal != null)
                        {
                            driver.WriteTextbox(By.Id("Input_642"), listing.SqFtTotal); // Sqft
                            driver.SetSelect(By.Id("Input_105"), listing.SqFtSource); // Sqft Source
                        }
                            
                        driver.WriteTextbox(By.Id("Input_287"), listing.SellingAgentLicenseNum ?? "99999999"); //Selling Agent ID

                        driver.WriteTextbox(By.Id("Input_803"), listing.SellingAgent2UID); // Selling Agent 2 ID
                        driver.WriteTextbox(By.Id("Input_794"), listing.SellTeamID); // Sell Team ID
                        //driver.WriteTextbox(By.Id("Input_801"), listing.SellingAgentSupervisor); // Selling Agent Supervisor

                        break;
                    // Pending
                    case "PND":
                        if (listing.ContractDate != null) {
                            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_285")));
                            driver.WriteTextbox(By.Id("Input_285"), listing.ContractDate.Value.ToShortDateString()); //Contract Date
                        }
                        break;
                    // Active
                    case "A":
                        var expirationDate1 = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_5")));
                        driver.WriteTextbox(By.Id("Input_5"), expirationDate1);
                        break;

                    // Cancelled
                    case "CAN":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_290")));
                        driver.WriteTextbox(By.Id("Input_290"), DateTime.Now.ToShortDateString());
                        break;

                    // Temp Off Market
                    case "TOM":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_312")));
                        driver.WriteTextbox(By.Id("Input_312"), DateTime.Now.ToShortDateString());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for DFW Listing with Id '" + listing.ResidentialListingID + "'");
                }
            }

            return UploadResult.Success;
        }

        #region Lot Actions

        public UploadResult UpdateLot(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            if (driver.UploadInformation.IsNewListing)
            {
                StartInsertLot(driver);
            }
            else
            {
                StartUpdate(driver, listing);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
                driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect")); // Tab: Input | Option: Residential
            }

            EditLotGeneralTab(driver, listing);
            EditLotLotUtilityEnvironmentTab(driver, listing);
            EditLotFinancialInformationTab(driver, listing);
            EditLotShowingTab(driver, listing);
            EditLotRemarksTab(driver, listing);

            return UploadResult.Success;
        }

        public UploadResult UpdateStatusLot(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            StartUpdate(driver, listing);
            string linkText = string.Empty;
            var transformedStatus = TransformStatus(listing.ListStatus, ref linkText);
            if (!string.IsNullOrWhiteSpace(linkText))
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(linkText)));
                driver.Click(By.PartialLinkText(linkText));
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("stickypush")));

                switch (transformedStatus)
                {
                    // Sold
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

                        driver.SetSelect(By.Id("Input_307"), listing.ListingFinancing); //First Financing

                        driver.SetSelect(By.Id("Input_307"), "0"); //Third Party Assistance Program                    
                        driver.SetSelect(By.Id("Input_105"), listing.SqFtSource); //Sqft Source
                        driver.SetSelect(By.Id("Input_587"), listing.MFinancing); //1st Financing:
                        driver.WriteTextbox(By.Id("Input_337"), listing.MortgageCoSold); //Mortgage Company
                        driver.WriteTextbox(By.Id("Input_306"), listing.TitleCoSold); //Closing Title Company
                        driver.WriteTextbox(By.Id("Input_287"), listing.SellingAgentLicenseNum ?? "99999999"); //Selling Agent ID

                        driver.WriteTextbox(By.Id("Input_803"), listing.SellingAgent2UID); // Selling Agent 2 ID
                        driver.WriteTextbox(By.Id("Input_794"), listing.SellTeamID); // Sell Team ID
                        driver.WriteTextbox(By.Id("Input_801"), listing.SellingAgentSupervisor); // Selling Agent Supervisor

                        break;

                    // Pending
                    case "PND":
                        var pendingDate = listing.PendingDate == null ? string.Empty : listing.PendingDate.Value.ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_285")));
                        driver.WriteTextbox(By.Id("Input_285"), pendingDate); //Contract Date
                        break;

                    // Active
                    case "A":
                        var expirationDate1 = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1).ToShortDateString();
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_5")));
                        driver.WriteTextbox(By.Id("Input_5"), expirationDate1);
                        break;

                    // Active Contingent
                    // Active Contingent
                    case "AC":
                    case "AKO":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_291")));

                        driver.WriteTextbox(By.Id("Input_682"), listing.ContractDate); // Contract Date

                        driver.WriteTextbox(By.Id("Input_291"), listing.ContingencyInfo); // 	Contingency Info

                        break;

                    // Active Option Contract
                    case "AOC":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_328")));

                        driver.WriteTextbox(By.Id("Input_682"), listing.ContractDate); // Contract Date

                        driver.WriteTextbox(By.Id("Input_328"), listing.ExpiredDateOption); // 	Option Expire Date

                        break;

                    // Cancelled
                    case "CAN":
                        driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_290")));
                        driver.WriteTextbox(By.Id("Input_290"), DateTime.Now.ToShortDateString());
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for DFW Listing with Id '" + listing.ResidentialListingID + "'");
                }
            }

            return UploadResult.Success;
        }

        private void EditLotGeneralTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.SetSelect(By.Id("Input_390"), "LNDRES"); // Property Type (HNF-176)
            driver.SetSelect(By.Id("Input_92"), "EXCAGE"); // Listing Type (HNF-176)
            driver.WriteTextbox(By.Id("Input_138"), listing.Acres); // Lot Size
            driver.SetSelect(By.Id("Input_527"), "ACRE"); // Lot Size Units
            //driver.WriteTextbox(By.Id("Input_397"), listing.); // Road Frontage 
            //driver.SetSelect(By.Id("Input_391"), ); // Subdivided
            driver.SetSelect(By.Id("Input_394"), listing.RoadAssessmentYN); // Road Asmt
            driver.WriteTextbox(By.Id("Input_73"), listing.ListPrice); // List Price
            driver.WriteTextbox(By.Id("Input_135"), listing.LotDim); // Lot Dimensions
            //driver.WriteTextbox(By.Id("Input_525"), listing.); // Feet to the Road
            driver.SetSelect(By.Id("Input_69"), "NO"); // Will Subdivide (HNF-176)
            // List Date
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
            driver.SetSelect(By.Id("Input_136"), listing.LotSize); // Lot Size/Acreage
            driver.SetMultipleCheckboxById("Input_396", listing.RoadFrontageDesc); // Road Frontage Desc
            driver.SetSelect(By.Id("Input_414"), "0"); // AG Exemption (HNF-176)
            //driver.SetSelect(By.Id("Input_413"), listing.); // Crop Retire Prog
            driver.WriteTextbox(By.Id("Input_95"), DateTime.Today.AddYears(1).ToShortDateString()); // Expire Date
            //driver.SetSelect(By.Id("Input_412"), listing.); // Land Leased
            driver.SetSelect(By.Id("Input_810"), listing.SeniorCommunity); // Senior Community
            driver.WriteTextbox(By.Id("Input_395"), listing.LotNum); // # Lots
            //driver.WriteTextbox(By.Id("Input_100"), listing.); // Year Built
            //driver.WriteTextbox(By.Id("Input_392"), listing.); // Lots Sold Sep
            driver.WriteTextbox(By.Id("Input_75"), listing.ParcelNumber); // Parcel ID
            //driver.WriteTextbox(By.Id("Input_393"), listing.); // Lots Sld Pkg
            //driver.WriteTextbox(By.Name("Input_738"), ); // Additional Parcel ID
            driver.SetSelect(By.Id("Input_72"), "0"); // Multi Parcel ID (HNF-176)
            //driver.SetMultipleCheckboxById("Input_89", listing.); // Present Use

            #region Location Information

            if (driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_58"), listing.StreetNum); // Street/Box Number
                driver.SetSelect(By.Id("Input_59"), listing.StreetDir); // Street Direction
                driver.WriteTextbox(By.Id("Input_60"), listing.StreetName.Replace('\'', ' ')); // Street Direction
                if (!string.IsNullOrWhiteSpace(listing.StreetType))
                {
                    driver.SetSelect(By.Id("Input_61"), listing.StreetType); // Street Type
                }
                driver.SetSelect(By.Id("Input_76"), listing.County); // County
            }
            
            driver.WriteTextbox(By.Id("Input_62"), listing.StreetDir); // Street Directional Suffix 
            driver.WriteTextbox(By.Id("Input_63"), listing.UnitNum); // Unit #
            driver.SetSelect(By.Id("Input_64"), listing.CityCode); // City
            driver.WriteTextbox(By.Id("Input_66"), listing.Zip); // Zip
            driver.WriteTextbox(By.Id("Input_67"), string.Empty); // Zip + 4

            driver.WriteTextbox(By.Id("Input_134"), !String.IsNullOrEmpty(listing.LotNum) ? listing.LotNum : ""); // Lot
            driver.WriteTextbox(By.Id("Input_56"), !String.IsNullOrEmpty(listing.Block) ? listing.Block : ""); // Block

            driver.SetSelect(By.Id("Input_77"), listing.MLSArea); // Area
            driver.SetSelect(By.Id("Input_78"), listing.MLSSubArea); // Sub Area

            driver.WriteTextbox(By.Id("Input_54"), listing.Subdivision); // Subdivision
            driver.WriteTextbox(By.Id("Input_67"), listing.NumLakes); //Lake Name
            driver.WriteTextbox(By.Id("Input_55"), listing.PlannedDevelopment); // Planned Development
            driver.WriteTextbox(By.Id("Input_91"), listing.Legal); // Additional Legal

            SetLongitudeAndLatitudeValues(driver, listing);
            #endregion

            #region School Information
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_99"), listing.SchoolDistrict); // School District
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_96"), listing.SchoolName1); // Elementary School
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_97"), listing.SchoolName2); // Middle School
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_98"), listing.SchoolName3); // High School 
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_458"), listing.SchoolName7); // Intermediate School 
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_459"), listing.SchoolName5); // Junior School 
            Thread.Sleep(500);
            driver.SetSelect(By.Id("Input_592"), listing.SchoolName6); // Senior High School 
            #endregion
        }

        private void EditLotLotUtilityEnvironmentTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            #region Lot Information

            //driver.WriteTextbox(By.Id("Input_401"), listing.); // Pasture Acres
            driver.WriteTextbox(By.Id("Input_405"), listing.NumberOfLakes); // # Lakes
            driver.WriteTextbox(By.Id("Input_411"), listing.NumberOfWaterMeters); // # Wtr Meters
            //driver.SetMultipleCheckboxById("Input_409", listing.); // Barn Information
            //driver.WriteTextbox(By.Id("Input_402"), listing.); // Cultivated Acres
            driver.WriteTextbox(By.Id("Input_688"), listing.NumberOfWaterMeters); // # of Stock Tanks/Ponds
            //driver.WriteTextbox(By.Id("Input_410"), listing.); // Surface Rights
            //driver.WriteTextbox(By.Id("Input_403"), listing.); // Bottom Land Acres
            driver.WriteTextbox(By.Id("Input_408"), listing.NumberOfWaterMeters); // # Wells
            //driver.WriteTextbox(By.Id("Input_404"), listing.); // Irrigated Acres
            driver.SetSelect(By.Id("Input_731"), (listing.WaterfrontYN != null && (bool)listing.WaterfrontYN) ? "1" : "0"); // Waterfront
            //driver.SetSelect(By.Id("Input_732"), listing.); // Lake Pump
            //driver.WriteTextbox(By.Id("Input_405"), listing.); // Platted Waterfront Boundary
            //driver.SetSelect(By.Id("Input_734"), listing.); // Dock Permitted
            driver.SetMultipleCheckboxById("Input_132", listing.LotFeatures); // Lot Description (HNF-176)
            driver.SetMultipleCheckboxById("Input_131", listing.ExteriorDesc); // Exterior Features
            driver.SetMultipleCheckboxById("Input_133", listing.Restrictions); // Restrictions
            driver.SetMultipleCheckboxById("Input_137", listing.Easements); // Easements
            driver.ScrollDown(250);
            driver.SetMultipleCheckboxById("Input_129", listing.FenceDesc); // Type of Fence
            driver.SetMultipleCheckboxById("Input_128", listing.SoilType); // Soil
            //driver.SetMultipleCheckboxById("Input_398", listing.); // Exterior Buildings
            driver.SetMultipleCheckboxById("Input_399", listing.ZoningLot); // Zoning Info
            driver.SetMultipleCheckboxById("Input_245", "RESSIN"); // Proposed Use
            driver.SetMultipleCheckboxById("Input_542", listing.Development); // Development
            driver.SetMultipleCheckboxById("Input_543", listing.Topography); // Topography
            //driver.SetMultipleCheckboxById("Input_544", listing.); // Crops / Grasses
            driver.SetMultipleCheckboxById("Input_545", listing.Documents); // Documents 
            driver.SetMultipleCheckboxById("Input_735", listing.WaterfrontFeatures); // Waterfront Features

            #endregion

            #region Utility Information
            
            driver.ScrollDown(250);
            driver.SetMultipleCheckboxById("Input_141", listing.Utilities); // Street/Utilities
            driver.SetMultipleCheckboxById("Input_400", listing.UtilitiesOther); // Other Utilities
            driver.SetSelect(By.Id("Input_127"), listing.MUDDistrict); // MUD District

            #endregion
        }

        private void EditLotFinancialInformationTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl04_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            //driver.SetMultipleCheckboxById("Input_167", listing.); // Proposed Financing
            // driver.SetMultipleCheckboxById("Input_161", "NEGOTI"); // Possession (HNF-176)
            driver.ExecuteScript("javascript:jQuery('#Input_161_NEGOTI').prop('checked',true);");
            driver.SetSelect(By.Id("Input_158"), "TRASCL"); // Loan Type (HNF-176)
            //driver.WriteTextbox(By.Id("Input_168"), listing.); // Mortgage Interest Rate
            //driver.SetSelect(By.Id("Input_163"), listing.); // Payment Type
            //driver.WriteTextbox(By.Id("Input_162"), listing.); // Payment 
            //driver.WriteTextbox(By.Id("Input_153"), listing.); // Balance
            //driver.WriteTextbox(By.Id("Input_169"), listing.); // Orig Mtg Date
            driver.SetSelect(By.Id("Input_264"), "0");  // 2nd Mortgage
            driver.SetSelect(By.Id("Input_207"), listing.HOA); // HOA
            //driver.SetSelect(By.Id("Input_160"), listing.); // Possible Short Sale
            //driver.WriteTextbox(By.Id("Input_159"), listing.); // Preferred Title Company
            //driver.WriteTextbox(By.Id("Input_157"), listing.); // Lender
            driver.SetSelect(By.Id("Input_155"), listing.AssociationFeeFrequency); // HOA Billing Freq
            //driver.WriteTextbox(By.Id("Input_166"), listing.); // Unexempt Taxes
            driver.WriteTextbox(By.Id("Input_165"), listing.TitleCoPhone); // Title Co Phone
            //driver.WriteTextbox(By.Id("Input_152"), listing.);  // Appraiser's Name
            driver.WriteTextbox(By.Id("Input_170"), listing.AssocFee); // HOA Dues
            driver.WriteTextbox(By.Id("Input_675"), listing.AssocPhone); // HOA Managemt Co Phone
            driver.WriteTextbox(By.Id("Input_164"), listing.TitleCoLocation); // Title Company Location
            driver.WriteTextbox(By.Id("Input_674"), listing.AssocName); // HOA Management Co
            driver.SetMultipleCheckboxById("Input_156", listing.AssocFeeIncludes); // HOA Includes
        }

        private void EditLotShowingTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl06_lbPageLink"));
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
        }

        private void EditLotRemarksTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.Id("m_rpPageList_ctl08_lbPageLink"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));

            driver.SetSelect(By.Id("Input_85"), "1"); // Allow Address Display
            driver.SetSelect(By.Id("Input_86"), "1"); // Allow AVM
            driver.SetSelect(By.Id("Input_87"), "1"); // Allow Internet Display
            driver.SetSelect(By.Id("Input_88"), "1"); // Allow Comments/Reviews

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
            if (!String.IsNullOrEmpty(listing.ContactEmailFromCompany))
                realtorContactEmail = listing.ContactEmailFromCompany;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmail))
                realtorContactEmail = listing.RealtorContactEmail;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmailFromCommunityProfile))
                realtorContactEmail = listing.RealtorContactEmailFromCommunityProfile;

            //UP-78
            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks(false)).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";

            driver.WriteTextbox(By.Id("Input_80"), "");
            driver.WriteTextbox(By.Id("Input_80"), bonusMessage + listing.GetPrivateRemarks(false) + realtorContactEmail);

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

            driver.WriteTextbox(By.Id("Input_81"), direction); //Allow Comments/Reviews

            #endregion
            
            #region Public Remark

            UpdatePublicRemarksInRemarksTab(driver, listing);

            #endregion

            #region Intra Office

            string compSaleText = String.Empty;

            if ((listing.ListStatus == "PND" || listing.ListStatus == "SLD") && driver.UploadInformation.IsNewListing)
            {
                compSaleText = "M-";
            }

            if (driver.UploadInformation.IsNewListing)
            {
                driver.WriteTextbox(By.Id("Input_82"), (!string.IsNullOrWhiteSpace(listing.PlanProfileName) ? (compSaleText + listing.PlanProfileName) : (compSaleText + " ")).RemoveSlash());
            }

            #endregion

            #region Excludes

            driver.WriteTextbox(By.Id("Input_114"), listing.Excludes.RemoveSlash()); // Property Description

            #endregion
        }

        #endregion Lot Actions
    }
}