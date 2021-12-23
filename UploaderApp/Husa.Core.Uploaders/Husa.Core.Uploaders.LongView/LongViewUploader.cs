using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace Husa.Core.Uploaders.LongView
{
    public partial class LongViewUploader : IUploader,
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
        /// Returns whether this uploader needs to have Adobe Flash functionality enabled. In Longview this always returns false.
        /// </summary>
        public bool IsFlashRequired { get { return false; } }

        /// <summary>
        /// Determines if a particular listing can be uploaded with the Longview Uploader
        /// </summary>
        /// <param name="listing">The listing to test for upload</param>
        /// <returns>True if the listing can be uploaded by the Longview Uploader, false if not.</returns>
        public bool CanUpload(ResidentialListingRequest listing)
        {
            return listing.MarketName == "Longview";
        }

        /// <summary>
        /// Inserts or updates a listing into the Longview MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <param name="media">The media files (mostly images) related to the listing</param>
        /// <returns>The final status of the upload operation and whether it succeeded or not</returns>
        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            try 
            { 
                driver.Click(By.Id("Close")); 
            } catch { }

            if (driver.UploadInformation.IsNewListing)
            {
                // If operation is insert
                StartInsert(driver);

                Thread.Sleep(1000);
            }
            else
            {
                // If operation is update
                StartUpdate(driver, listing);

                driver.wait.Until(ExpectedConditions.ElementExists(By.Id(listing.MLSNum)));

                Thread.Sleep(1000);

                // mode edit listing
                driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td:nth-child(16)\").dblclick()");

                Thread.Sleep(1000);

                driver.SwitchTo().ParentFrame();
                Thread.Sleep(1000);
                driver.SwitchTo().Frame("tab1_2");
                Thread.Sleep(1000);
                driver.SwitchTo().Frame("listingFrame");
            }

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.f-form-openall')[0].click();");

            EditListingSection(driver, listing);
            EditKeywordsSection(driver, listing);
            EditGeneralSection(driver, listing);
            EditFeaturesSection(driver, listing);
            EditRemarksSection(driver, listing);

            string[] soldStatus = new string[3] { "2_3", "2_0", "2_4" };
            if (soldStatus.Contains(listing.ListStatus))
            {
                EditSoldSection(driver, listing);
            }

            return UploadResult.Success;
        }

        public UploadResult Edit(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            Thread.Sleep(2000);

            try
            {
                driver.Click(By.Id("Close"));
            }
            catch { }

            if (driver.UploadInformation.IsNewListing)
            {
                // If operation is insert
                StartInsert(driver);
            }
            else
            {
                // If operation is update
                StartUpdate(driver, listing);
                
                // Mode edit listing
                driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='"+listing.MLSNum+"'] > td:nth-child(16)\").dblclick()");
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

            driver.Navigate("https://lgvboard.paragonrels.com/ParagonLS/Default.mvc/Login");
            Thread.Sleep(1500);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("Enter")));
            
            char[] username = listing.MarketUsername.ToArray();

            foreach (var charact in username)
            {
                Thread.Sleep(200);
                driver.FindElement(By.Id("LoginName")).SendKeys(charact.ToString());
            }

            driver.FindElement(By.Id("LoginName")).SendKeys(Keys.Tab);

            char[] password = listing.MarketPassword.ToArray();

            foreach (var charact in password)
            {
                Thread.Sleep(200);
                driver.FindElement(By.Id("Password")).SendKeys(charact.ToString());
            }
            driver.Click(By.Id("Enter"));
            Thread.Sleep(2000);
            try
            {
                if (driver.FindElement(By.ClassName("sub-header2-text")).Displayed)
                {
                    driver.FindElement(By.Id("Password")).Click();
                    driver.FindElement(By.Id("Password")).SendKeys(Keys.Enter);
                }
            }
            catch { }

            Thread.Sleep(2000);

            return LoginResult.Logged;
        }

        public UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            Thread.Sleep(2000);

            try
            {
                driver.Click(By.Id("Close"));
            }
            catch { }

            // If operation is update
            StartUpdate(driver, listing);

            // Mode edit listing
            driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td > a.MenuAction\").click();");
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery(\".OpenHouse\")[0].click();");
            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame(0);
            Thread.Sleep(1000);

            DeleteOpenHouses(driver, listing);

            AddOpenHouses(driver, listing);

            return UploadResult.Success;
        }

        public void DeleteOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("tab1_2");
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("otherFrame");
            Thread.Sleep(1000);

            driver.ExecuteScript("jQuery('#cb_grid').click();");
            Thread.Sleep(400);
            driver.ExecuteScript("jQuery('.Delete').click();");
            Thread.Sleep(400);
            driver.ExecuteScript("jQuery('.ui-dialog-buttonpane').find('button')[0].click()");
            Thread.Sleep(500);
        }

        #region Open House

        #region Virtual Tour

        public UploadResult UploadVirtualTour(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            try
            {
                driver.Click(By.Id("Close"));
            }
            catch { }

            // If operation is update
            StartUpdate(driver, listing);

            Thread.Sleep(2000);

            // Mode edit listing
            driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td > a.MenuAction\").click();");
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery(\".Tour\")[0].click();");
            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("tab1_2");
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("otherFrame");
            Thread.Sleep(1000);
            driver.FindElement(By.ClassName("f-form-lookup-partial")).Click();
            Thread.Sleep(1000);
            //driver.WriteTextbox(By.Id("f_526_606"), listing.VirtualTourURL);

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
            driver.ScrollDown();
            List<DateTime> date = OH.getNextDate(listing, max);

            foreach (var local in date)
            {
                driver.ExecuteScript("jQuery('.f-form-lookup-partial').click();");
                Thread.Sleep(1000);
                driver.SwitchTo().ParentFrame();
                Thread.Sleep(1000);
                driver.SwitchTo().Frame("otherFrame");
                Thread.Sleep(1000);

                string[] openhousestart;
                string[] openhouseend;

                string day = local.DayOfWeek.ToString().Substring(0, 3);
                if (listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null) != null && listing.GetType().GetProperty("OHEndTime" + day ).GetValue(listing, null) != null)
                {
                    driver.SwitchTo().ParentFrame();
                    Thread.Sleep(400);
                    driver.SwitchTo().ParentFrame();
                    Thread.Sleep(400);
                    driver.SwitchTo().ParentFrame();
                    Thread.Sleep(400);
                    driver.SwitchTo().Frame(0);
                    Thread.Sleep(1000);

                    driver.ExecuteScript("jQuery('.f-form-date-multi').datepick('setDate', '" + local.ToShortDateString() +  "');");
                    Thread.Sleep(400);

                    openhousestart = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null).ToString());
                    openhouseend = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null).ToString());

                    #region Time Start
                    // Hour
                    driver.SetSelect(By.Id("f_523_Hour"), DateTime.Parse(openhousestart[0]).Hour);
                    // Minutes
                    driver.SetSelect(By.Id("f_523_Minute"), DateTime.Parse(openhousestart[0]).Minute);
                    // AM/PM
                    driver.SetSelect(By.Id("f_523_AMPM"), openhousestart[1]);
                    #endregion Time Start

                    #region Time End
                    // Hour
                    driver.SetSelect(By.Id("f_524_Hour"), DateTime.Parse(openhouseend[0]).Hour);
                    // Minutes
                    driver.SetSelect(By.Id("f_524_Minute"), DateTime.Parse(openhouseend[0]).Minute);
                    // AM/PM
                    driver.SetSelect(By.Id("f_524_AMPM"), openhouseend[1]);
                    #endregion Time End

                    if (listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null) != null)
                        driver.WriteTextbox(By.Name("f_526_12"), listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null).ToString());

                    Thread.Sleep(400);

                    driver.SwitchTo().ParentFrame();
                    Thread.Sleep(400);

                    driver.ExecuteScript("jQuery('button')[0].click();");
                    Thread.Sleep(400);

                    driver.SwitchTo().Frame("tab1_2");
                    Thread.Sleep(1000);

                    driver.SwitchTo().Frame("otherFrame");
                    Thread.Sleep(1000);
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
            driver.ExecuteScript("jQuery('#logoff').click();");
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
                    throw new ArgumentOutOfRangeException("status", status, @"The status '" + status + @"' is not configured for Longview");
            }
        }

        #region Upload Code

        private void StartInsert(CoreWebDriver driver)
        {
            driver.ExecuteScript("jQuery('#listings-nav').parent().find('div ul').css('display', 'block'); jQuery('#listings-nav').parent().find('div ul > li:first > ul > li:first a').click(); jQuery('#listings-nav').parent().find('div ul').css('display', 'none');");

            Thread.Sleep(1000);

            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("tab1_2");
            Thread.Sleep(1000);

            try 
            {
                driver.ExecuteScript("jQuery('.ui-button-text')[0].click();");
            } catch { }

            driver.SwitchTo().Frame("listingFrame");
            Thread.Sleep(1000);
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
           try {
                driver.Click(By.Id("Close"));
            } catch { }
            driver.ExecuteScript("jQuery('#listings-nav').parent().find('div ul').css('display', 'block'); jQuery('#listings-nav').parent().find('div ul > li:nth-child(2) > ul > li:first a').click(); jQuery('#listings-nav').parent().find('div ul').css('display', 'none');");
            Thread.Sleep(1000);

            driver.SwitchTo().Frame("tab1_1");

            driver.WriteTextbox(By.Id("ListingIDs"), listing.MLSNum);
            driver.Click(By.Id("Go"));
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

        private void fillFieldSingleOption(CoreWebDriver driver, string fieldName, string value)
        {
            if(!String.IsNullOrEmpty(value)) 
            {
                var mainWindow = driver.WindowHandles.FirstOrDefault(c => c == driver.CurrentWindowHandle);
                driver.ExecuteScript("jQuery('#" + fieldName + "').focus();");
                driver.ExecuteScript("jQuery('#" + fieldName + "').closest('div').find('a')[0].click();");
                driver.SwitchTo().Window(driver.WindowHandles.Last());
                Thread.Sleep(1000);
                driver.SwitchTo().Frame(0);
                Thread.Sleep(1000);
                driver.FindElement(By.Id("search_cd")).Click();

                char[] fieldValue = value.ToArray();

                foreach (var charact in fieldValue)
                {
                    Thread.Sleep(400);
                    driver.FindElement(By.Id("search_cd")).SendKeys(charact.ToString());
                }
                Thread.Sleep(2000);

                driver.ExecuteScript("jQuery(\"table[id=list] > tbody > tr\").find(\"td:nth-child(2)\").each(function (index, item) { if(jQuery(item).text() == '" + value + "') { jQuery(item).click(); return; }  } );");
                driver.SwitchTo().ParentFrame();
                driver.Click(By.Id("Save"));
                Thread.Sleep(1000);
                driver.SwitchTo().Window(mainWindow);
                driver.SwitchTo().ParentFrame();
                Thread.Sleep(1000);
                driver.SwitchTo().Frame("tab1_2");
                Thread.Sleep(1000);
                driver.SwitchTo().Frame("listingFrame");
                Thread.Sleep(1000);
            }
        }

        private void fillFieldMultiOptions(CoreWebDriver driver, string fieldName, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                var mainWindow = driver.WindowHandles.FirstOrDefault(c => c == driver.CurrentWindowHandle);
                driver.ExecuteScript("jQuery('#" + fieldName + "').focus();");
                driver.ExecuteScript("jQuery('#" + fieldName + "').closest('div').find('a')[0].click();");
                driver.SwitchTo().Window(driver.WindowHandles.Last());
                Thread.Sleep(1000);
                driver.SwitchTo().Frame(0);
                Thread.Sleep(1000);
                driver.Click(By.Id("uncheckall"));
                driver.ExecuteScript("$(\"table > tbody > tr\").find(\"td\").each(function (index, item) { var jsonobj = $.parseJSON($(item).find(\"input\").val()); var arrayValues = [" + value + "]; if (arrayValues.indexOf(jsonobj.ID) > -1)  { $(item).find(\"input\").prop('checked', true); } });");
                driver.SwitchTo().ParentFrame();
                driver.Click(By.Id("Save"));
                Thread.Sleep(1000);
                driver.SwitchTo().Window(mainWindow);
                driver.SwitchTo().ParentFrame();
                Thread.Sleep(1000);
                driver.SwitchTo().Frame("tab1_2");
                Thread.Sleep(1000);
                driver.SwitchTo().Frame("listingFrame");
                Thread.Sleep(1000);
            }
        }

        private void EditListingSection(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            fillFieldSingleOption(driver, "f_3", "SF"); // Type
            fillFieldSingleOption(driver, "f_4", listing.MLSArea); // Area
            // Price
            if(listing.ListPrice > 100000)
            {
                string price = listing.ListPrice.ToString();
                int priceSize = listing.ListPrice.ToString().Length;
                string price1 = price.Substring(0, priceSize - (priceSize - 3));
                string price2 = price.Substring(priceSize - 3);

                driver.ExecuteScript("jQuery('#f_5_Thousands').val('" + price1 + "');jQuery('#f_5_Thousands').text('" + price1 + "');");
                driver.ExecuteScript("jQuery('#f_5_Dollars').val('" + price2 + "');jQuery('#f_5_Dollars').text('" + price2 + "');");
            }

            // Address
            driver.WriteTextbox(By.Id("f_6_Num"), listing.StreetNum);
            driver.WriteTextbox(By.Id("f_6_St"), listing.StreetName);

            // City
            driver.WriteTextbox(By.Id("f_8"), listing.City);

            // State
            driver.SetSelect(By.Name("f_9"), listing.State);

            // Zip
            driver.WriteTextbox(By.Id("f_10_Zip"), listing.Zip);

            // Status 
            string listStatus = listing.ListStatus;
            switch (listing.ListStatus)
            {
                case "1_0":
                    listStatus = "ACT";
                    break;
                case "1_3":
                    listStatus = "OS";
                    break;
                case "1_6":
                    listStatus = "AOP";
                    break;
                case "1_4":
                    listStatus = "BA";
                    break;
                case "4_0":
                    listStatus = "EXP";
                    break;
                case "3_0":
                    listStatus = "UC";
                    break;
                case "2_3":
                    listStatus = "NBM";
                    break;
                case "2_0":
                    listStatus = "SLD";
                    break;
                case "2_4":
                    listStatus = "BFL";
                    break;
                case "5_1":
                    listStatus = "WDU";
                    break;
                case "6_0":
                    listStatus = "RNT";
                    break;
            }
            fillFieldSingleOption(driver, "f_11", listStatus);

            // Sale/Rent
            driver.SetSelect(By.Name("f_12"), "S");

            // IDX Include
            //driver.SetSelect(By.Name("f_522"), );

            // VOW Include
            //driver.SetSelect(By.Name("f_635"), );

            // VOW Address
            //driver.SetSelect(By.Name("f_636"), );

            // VOW Comment
            //driver.SetSelect(By.Name("f_637"), );

            // VOW AVM
            //driver.SetSelect(By.Name("f_638"), );
        }

        private void EditKeywordsSection(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript("jQuery('#f_13').focus()");

            // Total Bedrooms
            driver.WriteTextbox(By.Id("f_13"), listing.Beds);

            // Full Baths
            driver.WriteTextbox(By.Id("f_14"), listing.BathsFull);

            // Half Baths
            driver.WriteTextbox(By.Id("f_15"), listing.BathsHalf);

            // Garage Capacity
            driver.WriteTextbox(By.Id("f_21"), listing.GarageCapacity);

            // Garage Type ??????????????????????
            // driver.SetSelect(By.Name("f_16"), listing.GarageDesc);

            // SqFt Range
            driver.SetSelect(By.Name("f_17"), listing.RangeDesc);

            // Age Range
            driver.SetSelect(By.Name("f_18"), listing.EES);

            // Lot Size
            driver.SetSelect(By.Name("f_19"), listing.LotSize);
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

        private void EditFeaturesSection(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript("jQuery('#f_13').focus()");

            // A-WILL SELL
            fillFieldMultiOptions(driver, "f_517_1", listing.AccessibilityDesc);

            // B-SHOWING INSTRUCTIONS
            fillFieldMultiOptions(driver, "f_517_2", listing.Showing);

            // C-POSSESSION
            // fillFieldMultiOptions(driver, "f_517_18", );

            // D-SPECIAL INFORMATION
            // fillFieldMultiOptions(driver, "f_517_17", );

            // E-WARRANTY
            // fillFieldMultiOptions(driver, "f_517_134", );

            // F-DOCUMENTS ON FILE
            // fillFieldMultiOptions(driver, "f_517_135", );

            // G-STYLE
            fillFieldMultiOptions(driver, "f_517_136", listing.HousingStyleDesc);

            // H-LEVELS
            fillFieldMultiOptions(driver, "f_517_137", listing.NumStories != null ? listing.NumStories.ToString() : "");

            // I-CONSTRUCTION
            fillFieldMultiOptions(driver, "f_517_24", listing.ConstructionDesc);

            // J-ROOF
            fillFieldMultiOptions(driver, "f_517_11", listing.RoofDesc);

            // K-FOUNDATION STYLE
            fillFieldMultiOptions(driver, "f_517_3", listing.FoundationDesc);

            // L-HEATING
            fillFieldMultiOptions(driver, "f_517_4", listing.HeatSystemDesc);

            // M-COOLING
            fillFieldMultiOptions(driver, "f_517_4", listing.CoolSystemDesc);

            // N-UTILITY TYPE
            fillFieldMultiOptions(driver, "f_517_22", listing.UtilitiesDesc);

            // O-ENERGY EFFICIENT
            fillFieldMultiOptions(driver, "f_517_14", listing.EnergyDesc);

            // P-WATER HEATER
            fillFieldMultiOptions(driver, "f_517_23", listing.WaterExtras);

            // Q-WATER/SEWER
            fillFieldMultiOptions(driver, "f_517_25", listing.WaterDesc);

            // R-ROOM DESCRIPTION
            fillFieldMultiOptions(driver, "f_517_138", listing.RoomDescription);

            // S-BATH DESCRIPTION
            fillFieldMultiOptions(driver, "f_517_139", listing.BedBathDesc);

            // T-BEDROOM DESCRIPTION
            fillFieldMultiOptions(driver, "f_517_141", listing.Bed1Desc);

            // U-DINING ROOM
            fillFieldMultiOptions(driver, "f_517_21", listing.DiningRoomDesc);

            // V-UTILITY/LAUNDRY
            fillFieldMultiOptions(driver, "f_517_8", listing.UtilityRoomDesc);

            // W-INTERIOR
            fillFieldMultiOptions(driver, "f_517_9", listing.InteriorDesc);

            // X-KITCHEN EQUIPMENT
            fillFieldMultiOptions(driver, "f_517_140", listing.KitchenDesc);

            // Y-FIREPLACE
            fillFieldMultiOptions(driver, "f_517_20", listing.FireplaceDesc);

            // Z-EXTERIOR FEATURES
            fillFieldMultiOptions(driver, "f_517_13", listing.ExteriorDesc);

            // ZA-POOL/SPA
            // fillFieldMultiOptions(driver, "f_517_142", );

            // ZB-FENCING
            fillFieldMultiOptions(driver, "f_517_143", listing.FenceDesc);

            // ZC-DRIVEWAY
            fillFieldMultiOptions(driver, "f_517_144", listing.CommonFeatures);

            //ZD-PARKING TYPE
            fillFieldMultiOptions(driver, "f_517_19", listing.ParkingDesc);

            // ZE-GARAGE
            fillFieldMultiOptions(driver, "f_517_145", listing.GarageDesc);

            // ZF-LOT
            fillFieldMultiOptions(driver, "f_517_15", listing.LotDesc);

            // ZG-ROAD TYPE
            // fillFieldMultiOptions(driver, "f_517_146", );

            // ZH-ROAD SURFACE
            // fillFieldMultiOptions(driver, "f_517_147", );

            // ZI-TOPOGRAPHY
            // fillFieldMultiOptions(driver, "f_517_148", );

            // ZJ-GRASSES
            // fillFieldMultiOptions(driver, "f_517_149", );

            // ZK-SOIL TYPE
            // fillFieldMultiOptions(driver, "f_517_150", );

            // ZL-LAND FEATURES
            fillFieldMultiOptions(driver, "f_517_151", listing.Restrictions);

            // ZM-OUTBUILDINGS
            // fillFieldMultiOptions(driver, "f_517_152", );

            // ZN-EASEMENTS
            // fillFieldMultiOptions(driver, "f_517_153", );

            // ZO-TIMBER TYPE
            // fillFieldMultiOptions(driver, "f_517_154", );

            // ZP-EXEMPTIONS
            fillFieldMultiOptions(driver, "f_517_160", listing.ExemptionsDesc);
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

        private void EditGeneralSection(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript("jQuery('#f_29').focus()");

            // Comp to Buyer Agency%/$
            driver.WriteTextbox(By.Id("f_254"), listing.CompBuy);

            // Listing Agent 2
            // fillFieldSingleOption(driver, "f_13", );

            // Listing Agent 3
            // fillFieldSingleOption(driver, "f_31", );

            // Brokerage License Number
            driver.WriteTextbox(By.Id("f_119"), "437572");

            // Supervisor License Number
            driver.WriteTextbox(By.Id("f_121"), "096651");

            // Agent License Number
            driver.WriteTextbox(By.Id("f_120"), "096651");

            // Listing Agent 2 License Number
            // driver.WriteTextbox(By.Id("f_122"), listing.);

            // Comp to SubAgency%/$
            driver.WriteTextbox(By.Id("f_253"), "0%");

            // Comp to Buyer Agency%/$
            if (!String.IsNullOrWhiteSpace(listing.CompBuy))
            {
                driver.WriteTextbox(By.Id("f_254"), listing.CompBuy);
            }
            else
            {
                driver.WriteTextbox(By.Id("f_254"), "");
            }
            //driver.WriteTextbox(By.Id("f_254"), listing.CompBuy);

            // Comp to Non-MLS
            // driver.WriteTextbox(By.Id("f_265"), listing.);

            // Compensation for Lease
            // driver.WriteTextbox(By.Id("f_400"), listing.);

            // Variable Commission
            driver.WriteTextbox(By.Id("f_255"), "NO");

            // EA/ER
            // driver.WriteTextbox(By.Id("f_256"), listing.);

            // Owner Name
            driver.WriteTextbox(By.Id("f_418"), listing.OwnerName);

            // Owner Phone
            driver.WriteTextbox(By.Id("f_102"), listing.OwnerPhone);

            // Listing Date
            if (String.IsNullOrEmpty(listing.MLSNum))
            {
                driver.WriteTextbox(By.Id("f_33"), DateTime.Now.Date.ToString("MM/dd/yyyy"));
            }
            else
            {
                driver.WriteTextbox(By.Id("f_33"), listing.ListDate.Value.ToString("MM/dd/yyyy"));
            }


            // Expiration Date
            // driver.WriteTextbox(By.Id("f_34"), listing.);

            // Option Period Expire Date
            // driver.WriteTextbox(By.Id("f_207"), listing.);

            // Addition/Survey
            driver.WriteTextbox(By.Id("f_103"), listing.Category);

            // Subdivision Y/ N
            // driver.SetSelect(By.Name("f_266"), listing.RangeDesc);

            // County
            driver.WriteTextbox(By.Id("f_72"), listing.County);

            // Water System
            driver.WriteTextbox(By.Id("f_73"), listing.SupWater);

            // Legal
            driver.WriteTextbox(By.Id("f_132"), listing.Legal);

            // Parcel Number
            driver.WriteTextbox(By.Id("f_386"), listing.TaxID);

            // Homeowners Fee Y/N
            driver.SetSelect(By.Name("f_74"), listing.HOA);

            // Homeowners Fee $
            //driver.WriteTextbox(By.Id("f_454"), listing.);

            // Ho Fee Monthly$
            driver.WriteTextbox(By.Id("f_197"), listing.AssocFeePaid);

            // HO Fee Annual$
            driver.WriteTextbox(By.Id("f_198"), listing.AssocFeeOtherDesc);

            // Taxes$
            driver.WriteTextbox(By.Id("f_104"), listing.TaxRate);

            // Exemptions
            driver.WriteTextbox(By.Id("f_105"), listing.ExemptionsDesc);

            // School District
            driver.WriteTextbox(By.Id("f_106"), listing.SchoolDistrictLongName);

            // Jr College
            // driver.WriteTextbox(By.Id("f_107"), listing.);

            // Entry Latitude/ Longitude
            driver.WriteTextbox(By.Id("f_108"), listing.Latitude + ", " + listing.Longitude);

            // Flood Zone
            driver.SetSelect(By.Name("f_75"), listing.HurricanePropertyFlooded);

            // Waterfront
            // driver.SetSelect(By.Name("f_97"), listing.);

            // Approx Lot Dimensions
            driver.WriteTextbox(By.Name("f_109"), listing.LotDim);

            // Number of Acres
            driver.WriteTextbox(By.Name("f_361"), listing.Acres);

            // Approx Living Area SqFt
            driver.WriteTextbox(By.Name("f_23"), listing.SqFtTotal);

            // SqFt Source
            // driver.SetSelect(By.Name("f_76"), listing.);

            // Total Rooms
            driver.WriteTextbox(By.Name("f_157"), listing.NumLivingAreas);

            // Total Baths
            driver.WriteTextbox(By.Name("f_158"), listing.BathsFull);

            // Master Bedroom Dim
            driver.WriteTextbox(By.Name("f_101"), listing.Bed1Dim);

            // Bedroom 2 Dim
            driver.WriteTextbox(By.Name("f_312"), listing.Bed2Dim);

            // Bedroom 3 Dim
            driver.WriteTextbox(By.Name("f_313"), listing.Bed3Dim);

            // Bedroom 4 Dim
            driver.WriteTextbox(By.Name("f_314"), listing.Bed4Dim);

            // Den Dimensions
            driver.WriteTextbox(By.Name("f_315"), listing.LivingRoom2Dim);

            // Family Room Dim
            driver.WriteTextbox(By.Name("f_316"), listing.Bed5Dim);

            // Kitchen Dimensions
            driver.WriteTextbox(By.Name("f_317"), listing.KitchenDim);

            // Living Room Dim
            driver.WriteTextbox(By.Name("f_318"), listing.LivingRoom1Dim);

            // Dining Room Dim
            driver.WriteTextbox(By.Name("f_319"), listing.DiningRoomDim);

            // Fireplace #
            driver.WriteTextbox(By.Name("f_159"), listing.NumFireplaces);

            // Pool Y/N
            // driver.SetSelect(By.Name("f_77"), listing.);

            // Hot Tub/Spa Y/N
            // driver.SetSelect(By.Name("f_78"), listing.);

            // Reserved Items
            // driver.WriteTextbox(By.Name("f_133"), listing.);

            // Directions
            driver.WriteTextbox(By.Name("f_427"), listing.Directions);

            // Foreclosure/Bank Owned
            // driver.SetSelect(By.Name("f_331"), listing.);

            // Escrow To
            // driver.WriteTextbox(By.Name("f_110"), listing.InsulationDesc);

            // Escrow Amt$
            driver.WriteTextbox(By.Name("f_111"), listing.InsulationDesc);

            // Year Built
            driver.WriteTextbox(By.Name("f_160"), listing.YearBuilt);

            // Virtual Tour
            // driver.WriteTextbox(By.Name("f_514"), listing.);

            // Virtual Tour 2
            // driver.WriteTextbox(By.Name("f_630"), listing.);
        }

        private void EditSoldSection(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            
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
            if (!String.IsNullOrWhiteSpace(listing.CompBuy))
            {
                driver.WriteTextbox(By.Id("Input_191"), "%" + listing.CompBuy);
            }
            else if (!String.IsNullOrWhiteSpace(listing.CommissionLease))
            {
                driver.WriteTextbox(By.Id("Input_191"), "$" + listing.CommissionLease);
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

        private void EditRemarksSection(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript("jQuery('#f_551').focus()");

            #region Public Remark
            BuiltStatus status = BuiltStatus.WithCompletion;
            switch (listing.YearBuiltDesc)
            {
                case "NCI":
                    status = BuiltStatus.ToBeBuilt;
                    break;
                case "NCC":
                    status = BuiltStatus.ReadyNow;
                    break;
                default:
                    status = BuiltStatus.WithCompletion;
                    break;
            }
            driver.WriteTextbox(By.Id("f_551"), listing.GetPublicRemarks(status));
            #endregion

            #region Confidential Agent Rmks

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

            driver.WriteTextbox(By.Id("f_553"), "");
            driver.WriteTextbox(By.Id("f_553"), bonusMessage + listing.GetPrivateRemarks(false) + realtorContactEmail);

            #endregion

            // Additional Showing Instr
            driver.WriteTextbox(By.Id("f_552"), listing.ShowingInstructions);

            // Addendum
            // driver.WriteTextbox(By.Id("f_550"), listing.);
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
        /// Updates a listing's completion date in the Longview MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <returns>The final status of the Completion Date update operation and whether it succeeded or not</returns>
        public UploadResult UpdateCompletionDate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            try
            {
                driver.Click(By.Id("Close"));
            }
            catch { }

            // If operation is update
            StartUpdate(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id(listing.MLSNum)));

            Thread.Sleep(1000);

            // Mode edit listing
            driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td > a.MenuAction\").click();");
            Thread.Sleep(1000);
            /*driver.ExecuteScript("jQuery(\".pricechange\")[0].click();");
            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame(0);
            Thread.Sleep(1000);

            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("tab1_2");
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("listingFrame");

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.f-form-openall')[0].click();");*/

            // TODO : add coding to fill the field

            return UploadResult.Success;
        }

        /// <summary>
        /// Updates a listing's images in the Longview MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <param name="media">The media files (mostly images) related to the listing</param>
        /// <returns>The final status of the image update operation and whether it succeeded or not</returns>
        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            try
            {
                driver.Click(By.Id("Close"));
            }
            catch { }

            // If operation is update
            StartUpdate(driver, listing);

            Thread.Sleep(2000);

            // Mode edit listing
            driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td > a.MenuAction\").click();");
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery(\".f-form-lookup-photo-admin\")[0].click();");
            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame(0);
            Thread.Sleep(1000);

            // Deleting all photos
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('ul.f-panel-menu > li > a#checkAll')[0].click();");
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('ul.f-panel-menu > li > a#deletePhotos')[0].click();");
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('button')[1].click();");

            // Uploading new photos
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('ul.f-panel-menu > li > a#uploadPhotos')[0].click();");

            Thread.Sleep(1000);

            driver.SwitchTo().Frame(0);

            if (media.Any(c => c is ResidentialListingMedia))
            {
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
                driver.FindElement(By.CssSelector("input[type=file]")).SendKeys(image.PathOnDisk);
                i++;
            }

            driver.Click(By.Id("uploader_start"));
        }

        /// <summary>
        /// Updates a listing's price in the Longview MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <returns>The final status of the price update operation and whether it succeeded or not</returns>
        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            Thread.Sleep(2000);

            try
            {
                driver.Click(By.Id("Close"));
            }
            catch { }

            // If operation is update
            StartUpdate(driver, listing);

            // Mode edit listing
            driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td > a.MenuAction\").click();");
            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery(\".pricechange\")[0].click();");
            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame(0);
            Thread.Sleep(1000);

            // Price
            if (listing.ListPrice > 100000)
            {
                string price = listing.ListPrice.ToString();
                int priceSize = listing.ListPrice.ToString().Length;
                string price1 = price.Substring(0, priceSize - (priceSize - 3));
                string price2 = price.Substring(priceSize - 3);

                driver.WriteTextbox(By.Id("f_5_Thousands"), price1);
                driver.WriteTextbox(By.Id("f_5_Dollars"), price2);

                driver.ExecuteScript("jQuery('#f_5_Thousands').focus(); jQuery('#f_5_Thousands').select(); jQuery('#f_5_Thousands').val('" + price1 + "');jQuery('#f_5_Thousands').text('" + price1 + "');");
                driver.ExecuteScript("jQuery('#f_5_Dollars').focus(); jQuery('#f_5_Dollars').select(); jQuery('#f_5_Dollars').val('" + price2 + "');jQuery('#f_5_Dollars').text('" + price2 + "');");
            }

            return UploadResult.Success;
        }

        /// <summary>
        /// Updates a listing's status in the Longview MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <returns>The final status of the status update operation and whether it succeeded or not</returns>
        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            try
            {
                driver.Click(By.Id("Close"));
            }
            catch { }

            // If operation is update
            StartUpdate(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id(listing.MLSNum)));

            Thread.Sleep(1000);

            // mode edit listing
            driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td:nth-child(16)\").dblclick()");

            Thread.Sleep(1000);

            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("tab1_2");
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("listingFrame");

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.f-form-openall')[0].click();");

            string listStatus = listing.ListStatus;
            switch (listing.ListStatus)
            {
                case "1_0":
                    listStatus = "ACT";
                    break;
                case "1_3":
                    listStatus = "OS";
                    break;
                case "1_6":
                    listStatus = "AOP";
                    break;
                case "1_4":
                    listStatus = "BA";
                    break;
                case "4_0":
                    listStatus = "EXP";
                    break;
                case "3_0":
                    listStatus = "UC";
                    break;
                case "2_3":
                    listStatus = "NBM";
                    break;
                case "2_0":
                    listStatus = "SLD";
                    break;
                case "2_4":
                    listStatus = "BFL";
                    break;
                case "5_1":
                    listStatus = "WDU";
                    break;
            }
            fillFieldSingleOption(driver, "f_11", listStatus);

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
            EditKeywordsSection(driver, listing);
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
            StartUpdate(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id(listing.MLSNum)));

            Thread.Sleep(1000);

            // mode edit listing
            driver.ExecuteScript("jQuery(\"#grid > tbody > tr[id='" + listing.MLSNum + "'] > td:nth-child(16)\").dblclick()");

            Thread.Sleep(1000);

            driver.SwitchTo().ParentFrame();
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("tab1_2");
            Thread.Sleep(1000);
            driver.SwitchTo().Frame("listingFrame");

            driver.ExecuteScript("jQuery('.f-form-openall')[0].click();");

            string listStatus = listing.ListStatus;
            switch (listing.ListStatus)
            {
                case "1_0":
                    listStatus = "ACT";
                    break;
                case "5_1":
                    listStatus = "WDU";
                    break;
                case "6_0":
                    listStatus = "RNT";
                    break;
            }

            if (!string.IsNullOrWhiteSpace(listStatus))
            {
                fillFieldSingleOption(driver, "f_11", listStatus); // Status
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

                        driver.WriteTextbox(By.Id("Input_803"), listing.SellingAgent2ID); // Selling Agent 2 ID
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
                        throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for Longview Listing with Id '" + listing.ResidentialListingID + "'");
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
            //driver.SetSelect(By.Id("Input_810"), listing.); // Senior Community
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