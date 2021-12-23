using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace Husa.Core.Uploaders.Austin
{
    public class AustinUploader :   IUploader, 
                                    IEditor, 
                                    IStatusUploader, 
                                    IImageUploader, 
                                    ICompletionDateUploader, 
                                    IPriceUploader, 
                                    IUpdateOpenHouseUploader,
                                    IUploadVirtualTourUploader
    {
        OpenHouseBase OH = new OpenHouseBase();

        public bool IsFlashRequired { get { return false; } }

        public bool CanUpload(ResidentialListingRequest listing)
        {
            //This method must return true if the listing can be uploaded with this MarketSpecific Uploader
            return listing.MarketName == "Austin";
        }

        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            #region navigateMenu
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl03_m_divFooterContainer")));
            #endregion

            if (string.IsNullOrWhiteSpace(listing.MLSNum))
                NewProperty(driver);
            else
                EditProperty(driver, listing);

            FillListingInformation(driver, listing);
            FillLocationInformation(driver, listing);
            FillGeneralListingInformation(driver, listing);
            FillAdditionalInformation(driver, listing);
            FillRoomInformation(driver, listing);
            FillDocumentsUtilityEESInformation(driver, listing);
            FillGreenEnergyInformation(driver, listing);
            FillFinancialInformation(driver, listing);
            FillShowingInformation(driver, listing);
            FillAgentOfficeInformation(driver, listing);
            //FillCompensationInformation(driver, listing);
            FillRemarks(driver, listing);

            try { driver.Click(By.LinkText("Status")); } catch { }
            
            return UploadResult.Success;
        }

        private void FillAgentOfficeInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Agent/Office"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            if(driver.UploadInformation.IsNewListing && listing.AgentList != null)
            {
                driver.WriteTextbox(By.Id("Input_629"), listing.AgentList);
            }

            driver.SetSelect(By.Id("Input_315"), "Percent"); // Sub Agency Compensation Type
            driver.WriteTextbox(By.Id("Input_314"), "0.0"); // Sub Agency Compensation

            if (!String.IsNullOrEmpty(listing.CompBuy))
            {
                if (listing.CompBuy.Contains("%"))
                {
                    driver.SetSelect(By.Id("Input_316"), "Percent", true); // Buyer Agency Compensation Type
                }
                else if (listing.CompBuy.Contains("Dollar"))
                {
                    driver.SetSelect(By.Id("Input_316"), "$", true); // Buyer Agency Compensation Type
                }
                driver.WriteTextbox(By.Id("Input_510"), listing.CompBuy.Replace("%", "").Replace("$", ""));
            }

            if (listing.BonusWAmountCheckBox)
            {
                driver.SetSelect(By.Id("Input_318"), "Dollar", true); // Bonus to BA
                driver.WriteTextbox(By.Id("Input_317"), listing.AgentBonusAmount.Replace("%", "").Replace("$", "")); // Bonus to BA Amount
            }
            else
            {
                driver.WriteTextbox(By.Id("Input_317"), ""); 
            }

            driver.SetSelect(By.Id("Input_319"), "0", true); // Dual Variable Compensation
            driver.SetSelect(By.Id("Input_353"), "0", true); // Intermediary
        }

        private void FillGreenEnergyInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Green Energy"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            driver.SetMultipleCheckboxById("Input_280", "NONE"); // Upgraded Energy Efficient

            driver.SetMultipleCheckboxById("Input_281", "None"); // Green Sustainability
        }

        private void FillRoomInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Rooms"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            var roomTypes = ReadRoomAndFeatures(listing);

            var i = 0;

            foreach (var roomType in roomTypes.Where(c => c.IsValid()))
            {
                if (i > 0)
                {
                    driver.ScrollDown(1000);
                    driver.Click(By.Id("_Input_349_more"));
                    Thread.Sleep(400);
                }

                driver.SetSelect(By.Id("_Input_349__REPEAT" + i + "_345"), roomType.Value, true); // FieldName
                Thread.Sleep(400);
                driver.ScrollDown();


                // MLS-293
                if (listing.InteriorDesc.Contains("MSTDW") && roomType.Value == "MSTRBED")
                {
                    driver.SetSelect(By.Id("_Input_349__REPEAT" + i + "_346"), "MAIN", true);
                } 
                else
                {
                    driver.SetSelect(By.Id("_Input_349__REPEAT" + i + "_346"), roomType.Level, true);
                }
                Thread.Sleep(400);
                driver.ScrollDown();
                //driver.WriteTextbox(By.Id("_Input_253__REPEAT" + i + "_250"), roomType.Length, true);
                //Thread.Sleep(400);
                //driver.ScrollDown();
                //driver.WriteTextbox(By.Id("_Input_253__REPEAT" + i + "_251"), roomType.Width, true);
                //Thread.Sleep(400);
                //driver.ScrollDown();
                driver.SetMultipleCheckboxById("_Input_349__REPEAT" + i + "_347", roomType.Features);
                Thread.Sleep(400);
                driver.ScrollDown();

                i++;
            }
        }

        private IEnumerable<RoomType> ReadRoomAndFeatures(ResidentialListingRequest listing)
        {
            return new List<RoomType>()
            {
                #region Living
                new RoomType("LIVING", listing.LivingRoom1Level, listing.LivingRoom1Length, listing.LivingRoom1Width, null),
                //new RoomType("LIVROO", listing.LivingRoom2Level, listing.LivingRoom2Length, listing.LivingRoom2Width, null),
                new RoomType("GAME", listing.LivingRoom3Level, listing.LivingRoom3Length, listing.LivingRoom3Width, null),
                #endregion

                #region Dining
                new RoomType("DINING", listing.DiningRoomLevel, listing.DiningRoomLength, listing.DiningRoomWidth, null),
                #endregion

                #region Breakfast
                new RoomType("BREROO", listing.BreakfastLevel, listing.BreakfastLength, listing.BreakfastWidth, null),
                #endregion

                #region Kitchen
                new RoomType("KITCHEN", listing.KitchenLevel, listing.KitchenLength, listing.KitchenWidth,listing.KitchenDesc),
                #endregion

                #region Master Bedroom
                new RoomType("MSTRBED", listing.Bed1Level, listing.Bed1Length, listing.Bed1Width, listing.Bed1Desc),
                #endregion

                #region Master Bathroom
                new RoomType("MSTRBATH", listing.Bed1Level, listing.Bed1Length, listing.Bed1Width, listing.WaterAccessDesc),
                #endregion

                #region Bedroom
                new RoomType("BDRM", listing.Bed2Level, null, null, "SRMRKS"),
                new RoomType("BDRM", listing.Bed3Level, null, null, "SRMRKS"),
                new RoomType("BDRM", listing.Bed4Level, null, null, "SRMRKS"),
                new RoomType("BDRM", listing.Bed5Level, null, null, "SRMRKS"),
                #endregion

                #region Study
                new RoomType("LIBRARY", listing.StudyLevel, listing.StudyLength, listing.StudyWidth, null),
                #endregion

                #region Utility
                new RoomType("LAUNDRY", listing.UtilityRoomLevel, listing.UtilityRoomLength, listing.UtilityRoomWidth,listing.LaundryFacilityDesc),
                #endregion

                #region Other
                new RoomType("MEDIA", listing.OtherRoom1Level, listing.OtherRoom1Length, listing.OtherRoom1Width, null),
                new RoomType("DEN", listing.OtherRoom2Level, listing.OtherRoom2Length, listing.OtherRoom2Width, null),
                #endregion

                #region Living Room 2
                new RoomType("FAMILY", listing.LivingRoom2Level,  null, null, null),
                #endregion

                #region Bonus
                new RoomType("BONUS", listing.Bed5Location, null, null, null),
                #endregion

                #region Loft
                new RoomType("LOFT", listing.Bed1Location, null, null, null),
                #endregion

                #region Office
                new RoomType("OFFICE", listing.Bed3Location, null, null, null),
                #endregion

                #region Library
                new RoomType("LIBRARY", listing.Bed4Location, null, null, null),
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
                return !string.IsNullOrWhiteSpace(Level);
            }
        }

        public UploadResult Edit(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            #region navigateMenu
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl03_m_divFooterContainer")));
            #endregion

            if (string.IsNullOrWhiteSpace(listing.MLSNum))
                NewProperty(driver);
            else
                EditProperty(driver, listing);

            return UploadResult.Success;
        }

        public UploadResult UpdateCompletionDate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);
            Login(driver, listing);

            EditProperty(driver, listing);

            driver.ScrollToTop();
            driver.Click(By.LinkText("General"));

            driver.WriteTextbox(By.Id("Input_218"), listing.YearBuilt); // Year Built

            if(!String.IsNullOrEmpty(listing.YearBuiltDesc) && listing.YearBuiltDesc == "TB")
            {
                listing.YearBuiltDesc = "TBD," + listing.YearBuiltDesc;
            }

            driver.SetMultipleCheckboxById("Input_225", listing.YearBuiltDesc); // Year Built Description
            //driver.SetSelect(By.Id("Input_219"), listing.YearBuiltDesc); // Year Built Description

            driver.Click(By.LinkText("Remarks/Tours/Internet"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            UpdatePublicRemarksInRemarksTab(driver, listing);

            return UploadResult.Success;
        }

        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            QuickEdit(driver, listing);
            //  var expirationDate = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1);
            Thread.Sleep(2000);
            switch (listing.ListStatus)
            {
                #region Sold
                case "S":
                    
                    driver.Click(By.PartialLinkText("Change to Closed"));
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Id("Input_94"), listing.PendingDate != null ? listing.PendingDate.Value.ToShortDateString() : ""); // Pending Date
                    driver.WriteTextbox(By.Id("Input_85"), listing.ClosedDate != null ? listing.ClosedDate.Value.ToShortDateString(): ""); // Close Date
                    driver.SetSelect(By.Id("Input_524"), "EXCL"); // Property Condition at Closing
                    driver.WriteTextbox(By.Id("Input_84"), listing.SalesPrice); // Close Price

                    driver.WriteTextbox(By.Id("Input_526"), "None"); // 	Closed Comments
                    //driver.SetSelect(By.Id("Input_655"), "0"); // Property Sale Contingency

                    driver.WriteTextbox(By.Id("Input_517"), listing.BuyersClsgCostPdbySell); // Buyer Clsg Cost Pd By Sell($)
                    //driver.WriteTextbox(By.Id("Input_522"), listing.BuyerPoints); // Buyer Points
                    driver.WriteTextbox(By.Id("Input_521"), listing.SellerPoints); // Seller Points
                    //driver.WriteTextbox(By.Id("Input_523"), ); // Total Points
                    driver.SetMultipleCheckboxById("Input_525", listing.SoldTerms); // Buyer Financing

                    //driver.WriteTextbox(By.Id("Input_518"), ); // Appraiser Amount
                    driver.WriteTextbox(By.Id("Input_519"), "0"); //RepairsAmount
                    //driver.WriteTextbox(By.Id("Input_520"), listing.Loan1Amount); // New Loan Amount ($)

                    driver.WriteTextbox(By.Id("Input_726"), listing.SellingAgentLicenseNum ?? "NONMBR");

                    break;

                #endregion

                #region Pending
                case "P":
                    driver.Click(By.PartialLinkText("Change to Pending"));
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Id("Input_94"), listing.PendingDate);
                    driver.WriteTextbox(By.Id("Input_515"), listing.EstClosedDate);
                    driver.WriteTextbox(By.Id("Input_81"), listing.ExpiredDate);
                    driver.SetSelect(By.Id("Input_655"), listing.ContingencyInfo == "1" ? listing.ContingencyInfo : "0"); // Property Sell Contingency

                    break;
                #endregion

                #region Pending Taking Backups

                case "PB":
                    driver.Click(By.LinkText("Change to Pending - Taking Backups"));

                    driver.WriteTextbox(By.Id("Input_1067"), listing.PendingDate);
                    driver.WriteTextbox(By.Id("Input_1352"), listing.EstClosedDate);
                    driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);
                    break;

                #endregion

                #region Active

                case "A":
                    driver.Click(By.LinkText("Change to Active"));
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Id("Input_81"), DateTime.Today.AddDays(1).AddYears(1).ToShortDateString());

                    break;
                #endregion

                #region Active Contingent

                case "AC":
                    driver.Click(By.LinkText("Change to Active Contingent"));
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Id("Input_1006"), listing.ContingencyDate);
                    driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

                    break;
                #endregion

                #region Temporarily Off Market

                case "T":
                    driver.Click(By.LinkText("Change to Temporarily Off Market"));
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Id("Input_1018"), DateTime.Now.ToShortDateString());
                    driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

                    break;
                #endregion

                #region Withdrawn

                case "W":
                    driver.Click(By.PartialLinkText("Change to Withdrawn"));
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Id("Input_1020"), DateTime.Now.ToShortDateString());

                    break;
                #endregion

                default:
                    throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for Austin Listing with Id '" + listing.ResidentialListingID + "'");
            }

            return UploadResult.Success;
        }

        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            QuickEdit(driver, listing);

            // Enter Image Management
            Thread.Sleep(2000);
            driver.Click(By.Id("m_lbManagePhotos"));

            //Prepare Media
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lbSave")));
            DeleteAllImages(driver);
            UploadNewImages(driver, media.OfType<ResidentialListingMedia>());

            return UploadResult.Success;
        }

        #region Media Code
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

                ((IJavaScriptExecutor)driver).ExecuteScript(" ManageMediaJS.editDescription(0);");
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_tbxDescription")));
                driver.FindElement(By.Id("m_tbxDescription")).Click();
                driver.WriteTextbox(By.Id("m_tbxDescription"), image.Caption);
                driver.Click(By.LinkText("Done"));

                driver.Click(By.Id("m_ucDetailsView_m_btnSave"));

                i++;
            }
        }
        #endregion

        public UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            QuickEdit(driver, listing);

            Thread.Sleep(1000);

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Open Houses Input Form")));
            driver.Click(By.LinkText("Open Houses Input Form"));

            Thread.Sleep(1000);

            DeleteOpenHouses(driver, listing);
            if (listing.EnableOpenHouse && listing.AgreeOpenHouseConditions)
            {
                AddOpenHouses(driver, listing);
                driver.ScrollDown();
                Thread.Sleep(2000);
            }

            return UploadResult.Success;
        }

        #region Open House

        public void DeleteOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            int countDeleteButtons = driver.FindElements(By.LinkText("Delete")).Count();
            for (int i = 0; i < countDeleteButtons; i++)
            {
                try
                {
                    driver.ScrollToTop();
                    try { driver.ExecuteScript("Subforms['s_168'].deleteRow('_Input_168__del_REPEAT" + i + "_');"); } catch { }
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

        public void AddOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            int max = 10;
            List<DateTime> date = OH.getNextDate(listing, max);


            // HCS-596
            String openHouseType = "PUBLIC";

            int i = 0;
            Thread.Sleep(1000);
            driver.ScrollDown();
            foreach (var local in date)
            {
                string[] openhousestart;
                string[] openhouseend;

                string day = local.DayOfWeek.ToString().Substring(0, 3);
                if (listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null) != null && listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null) != null)
                {
                    openhousestart = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.START, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);
                    openhouseend = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.END, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);

                    // Open House Status
                    driver.SetSelect(By.Id("_Input_168__REPEAT" + i + "_165"), "ACT");

                    // Open House Type
                    driver.SetSelect(By.Id("_Input_168__REPEAT" + i + "_161"), openHouseType);

                    // Refreshments
                    if (listing.GetType().GetProperty("OHRefreshments" + day).GetValue(listing, null) != null)
                    {
                        driver.SetMultipleCheckboxById("_Input_168__REPEAT" + i + "_652", listing.GetType().GetProperty("OHRefreshments" + day).GetValue(listing, null).ToString());
                    }

                    // Open House Date
                    driver.WriteTextbox(By.Id("_Input_168__REPEAT" + i + "_162"), local.ToString("MM/dd/yyyy"));

                    // Start Time
                    driver.WriteTextbox(By.Id("_Input_168__REPEAT" + i + "_TextBox_163"), openhousestart[0]);
                    if (openhousestart[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id("_Input_168__REPEAT" + i + "_RadioButtonList_163_0"), openhousestart[1]);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id("_Input_168__REPEAT" + i + "_RadioButtonList_163_1"), openhousestart[1]);
                    }

                    // End Time
                    driver.WriteTextbox(By.Id("_Input_168__REPEAT" + i + "_TextBox_164"), openhouseend[0]);
                    if (openhouseend[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id("_Input_168__REPEAT" + i + "_RadioButtonList_164_0"), openhouseend[1]);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id("_Input_168__REPEAT" + i + "_RadioButtonList_164_1"), openhouseend[1]);
                    }

                    // Virtual Open House URL (it's Empty)

                    // Open House Remarks
                    if (listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null) != null)
                        driver.WriteTextbox(By.Id("_Input_168__REPEAT" + i + "_167"), listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null));

                    i++;
                    int countButtons = driver.FindElements(By.LinkText("Delete")).Count();
                    if (max >= (i + 1) && (countButtons < (i + 1)))
                    {
                        try
                        {
                            driver.ScrollDown();
                            driver.Click(By.LinkText("More"));
                        }
                        catch { }
                    }

                    driver.ScrollDown();
                }
            }
            Thread.Sleep(1000);
        }

        #endregion

        #region Virtual Tour

        public UploadResult UploadVirtualTour(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            QuickEdit(driver, listing);

            Thread.Sleep(1000);

            driver.Click(By.PartialLinkText("Residential Input Form"));

            Thread.Sleep(1000);

            driver.Click(By.LinkText("Remarks/Tours/Internet"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));


            var virtualTour = media.OfType<ResidentialListingVirtualTour>().ToList();
            if (virtualTour != null && virtualTour.Count > 0)
            {
                driver.WriteTextbox(By.Id("Input_324"), virtualTour[0].VirtualTourAddress); // Virtual Tour URL Unbranded
                if (virtualTour.Count > 1)
                {
                    driver.WriteTextbox(By.Id("Input_325"), virtualTour[1].VirtualTourAddress); // Virtual Tour URL Unbranded
                }
            }
                

            return UploadResult.Success;
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

        /// <summary>
        /// Login an session in the MLS system
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <returns>The final status of the login operation and whether it succeeded or not</returns>
        public LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            // Remove all cookies
            driver.Manage().Cookies.DeleteAllCookies();

            #region login
            // Connect to the login page
            driver.Navigate("http://matrix.abor.com/");
            //driver.Navigate().GoToUrl();


            // click button "Login as a Member"
            
            // MQ-306 Commented code below
            //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("logmein")));
            //driver.Click(By.Id("logmein"));

            /*driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("login")));

            // Fill the login and password
            driver.WriteTextbox(By.Id("j_username"), listing.MarketUsername);
            driver.WriteTextbox(By.Id("password"), listing.MarketPassword);
            driver.Click(By.Id("login"));*/

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("loginbtn")));
            driver.ExecuteScript("document.getElementById('username').focus();");
            driver.WriteTextbox(By.Id("username"), listing.MarketUsername);

            driver.ExecuteScript("$('#password').css({ 'display': 'none' });");
            driver.ExecuteScript("document.getElementById('password').value='" + listing.MarketPassword + "';");
            //driver.ExecuteScript("document.getElementById('j_password').value='" + listing.MarketPassword + "';");

            driver.Click(By.Id("loginbtn"));
            #endregion

            Thread.Sleep(2000);

            #region loadingMLS
            // Wait until login page has been loaded.

            //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("footer")));

            // Use the same browser page NOT _blank
            driver.Navigate("http://matrix.abor.com/Matrix/Default.aspx?c=AAEAAAD*****AQAAAAAAAAARAQAAAFEAAAAGAgAAAAQzNTI0DRsGAwAAAAbDq8KIDVkNNAs)&f=");
            #endregion

            Thread.Sleep(2000);

            #region ReadyToWork
            if (ExpectedConditions.ElementIsVisible(By.Id("NewsDetailDismiss")) == null)
            {
                driver.Click(By.Id("NewsDetailDismiss"));
            }

            try { driver.ExecuteScript("jQuery('#NewsDetailDismissNew').click();"); } catch { }

            driver.Click(By.LinkText("Skip"), true);
            Thread.Sleep(2000);

            #endregion

            return LoginResult.Logged;
        }

        private void NewProperty(CoreWebDriver driver)
        {
            #region newProperty
            // This code is for a new property
            driver.Click(By.LinkText("Add/Edit"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            driver.Click(By.LinkText("Add new"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            driver.Click(By.LinkText("Residential Input Form"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl03_m_divFooterContainer")));
            driver.Click(By.LinkText("Start with a blank Property"));
            #endregion
        }

        private void EditProperty(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            #region editProperty
            QuickEdit(driver, listing);
            Thread.Sleep(2000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Residential Input Form")));
            driver.Click(By.LinkText("Residential Input Form"));
            #endregion
        }

        private void QuickEdit(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Add/Edit")));
            driver.Click(By.LinkText("Add/Edit"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")));
            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.Click(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")); // Link "Edit"
        }

        /// <summary>
        /// Fills the information for the Listing Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillListingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            #region listingInformation
            Thread.Sleep(400);
            
            // Listing Information
            driver.SetSelect(By.Id("Input_179"), "EA"); // List Agreement Type
            driver.SetSelect(By.Id("Input_341"), "LIMIT"); // Listing Service
            driver.SetMultipleCheckboxById("Input_180", "STANDARD"); // Special Listing Conditions
            driver.SetSelect(By.Id("Input_181"), "A"); // List Agreement Document
            driver.WriteTextbox(By.Id("Input_77"), listing.ListPrice); // List Price
            //driver.SetSelect(By.Id("Input_141"), "0"); // Foreclosure/REO
            //driver.SetSelect(By.Id("Input_150"), "None"); // Sales Restrictions (1)
            if (driver.UploadInformation.IsNewListing)
            {
                //driver.WriteTextbox(By.Id("Input_113"), DateTime.Now.ToShortDateString()); // List Date
                //driver.WriteTextbox(By.Id("Input_114"), DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
                if(listing.ListDate != null)
                    driver.WriteTextbox(By.Id("Input_83"), ((DateTime)listing.ListDate).AddYears(1).ToShortDateString()); // Expiration Date
                else
                    driver.WriteTextbox(By.Id("Input_83"), DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
            }
            #endregion
        }

        /// <summary>
        /// Fills the information for the Location Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillLocationInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            #region Location Information

            driver.WriteTextbox(By.Id("Input_183"), listing.StreetNum); // Street #
            driver.SetSelect(By.Id("Input_184"), listing.StreetDir); // Pre Direction (NM)
            driver.WriteTextbox(By.Id("Input_185"), listing.StreetName); // Street Name
            driver.SetSelect(By.Id("Input_186"), listing.StreetType); // Street Type (NM)
            driver.SetSelect(By.Id("Input_394"), listing.StreetDir); // Street Dir Suffix
            driver.WriteTextbox(By.Id("Input_190"), !String.IsNullOrEmpty(listing.UnitNum) ? listing.UnitNum : ""); // Unit # (NM)
            driver.SetSelect(By.Id("Input_191"), listing.County); // County
            driver.SetSelect(By.Id("Input_192"), listing.City); // City
            driver.SetSelect(By.Id("Input_193"), listing.State); // State
            driver.SetSelect(By.Id("Input_399"), "US"); // Country
            driver.WriteTextbox(By.Id("Input_194"), listing.Zip); // ZIP Code
            driver.WriteTextbox(By.Id("Input_196"), listing.Subdivision); // Subdivision
            driver.WriteTextbox(By.Id("Input_197"), listing.Legal); // Tax Legal Description
            driver.WriteTextbox(By.Id("Input_199"), listing.OtherFees); // Tax Lot

            driver.WriteTextbox(By.Id("Input_201"), listing.TaxID); // Parcel ID
            driver.SetSelect(By.Id("Input_202"), "0"); // Additional Parcels Y/N

            driver.ScrollDown(1000);

            if(!String.IsNullOrEmpty(listing.MLSArea))
            {
                driver.Click(By.Id("Input_204_A"));
                Thread.Sleep(1500);
                List<string> handles = driver.WindowHandles.ToList<String>();
                driver.SwitchTo().Window(handles.Last());
                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_txtSearch")));
                driver.ExecuteScript("jQuery('.hideCheckBoxes ul>li[title=" + listing.MLSArea + "]')[0].click();");
                //driver.WriteTextbox(By.Id("m_txtSearch"), listing.MLSArea);
                driver.ExecuteScript("LBI_Popup.selectItem(true);");
                driver.SwitchTo().Window(handles.FirstOrDefault());
            }

            //driver.SetSelect(By.Id("Input_204_TB"), listing.MLSArea); // MLS Area
            driver.SetMultipleCheckboxById("Input_343", "N"); // FEMA 100 Yr Flood Plain
            driver.SetSelect(By.Id("Input_206"), "N"); // ETJ

            #endregion Location Information

            #region geolocalization
            SetLongitudeAndLatitudeValues(driver, listing);
            #endregion

            #region School Information

            driver.SetSelect(By.Id("Input_207"), listing.SchoolDistrict); // County/School District
            driver.SetSelect(By.Id("Input_209"), listing.SchoolName1); // School District/Elementary A
            driver.SetSelect(By.Id("Input_210"), listing.SchoolName3); // School District/Middle / Intermediate School
            driver.SetSelect(By.Id("Input_211"), listing.SchoolName6); // School District/9 Grade / High School
            
            driver.WriteTextbox(By.Id("Input_212"), listing.SchoolName2); // Elementary Other
            driver.SetSelect(By.Id("Input_213"), listing.SchoolName4); // Middle or Junior Other
            driver.SetSelect(By.Id("Input_214"), listing.SchoolName5); // High School Other

            #endregion School Information

            driver.ScrollDown();
            //driver.WriteTextbox(By.Id("Input_889"), listing.MapscoMapPage); // MAPSCO PAGE
            //driver.WriteTextbox(By.Id("Input_890"), listing.MapscoMapCoord); // MAPSCO GRID
        }

        /// <summary>
        /// Fills the information for the General Listing Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillGeneralListingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ScrollToTop();
            driver.Click(By.LinkText("General"));

            driver.SetSelect(By.Id("Input_215"), listing.PropSubType); // Property Sub Type (1)
            driver.SetSelect(By.Id("Input_216"), "FEESIM"); // Ownership Type
            driver.SetMultipleCheckboxById("Input_650", listing.NumStories != null ? listing.NumStories.ToString() : ""); // Levels

            driver.WriteTextbox(By.Id("Input_220"), listing.NumBedsMainLevel); // Main Level Beds
            driver.WriteTextbox(By.Id("Input_221"), listing.NumBedsOtherLevels); // # Other Level Beds
            driver.WriteTextbox(By.Id("Input_218"), listing.YearBuilt); // Year Built

            driver.SetSelect(By.Id("Input_219"), "BUILDER");

            if (!String.IsNullOrEmpty(listing.YearBuiltDesc))
            {
                string propertyCondition = listing.YearBuiltDesc;
                switch (listing.YearBuiltDesc)
                {
                    case "NW":
                    case "NWCONSTR":
                        propertyCondition = "NWCONSTR";
                        break;
                    case "UC":
                        propertyCondition = "UC";
                        break;
                    case "TB":
                        propertyCondition = "TBD";
                        break;
                }

                driver.SetMultipleCheckboxById("Input_225", propertyCondition); // Year Built Description
            }

            driver.WriteTextbox(By.Id("Input_224"), listing.BathsFull); // Full Baths
            driver.WriteTextbox(By.Id("Input_223"), listing.BathsHalf); // Half Bath
            driver.WriteTextbox(By.Id("Input_659"), listing.NumLivingAreas); // Living
            driver.WriteTextbox(By.Id("Input_660"), listing.NumDiningAreas); // Dining
            driver.WriteTextbox(By.Id("Input_226"), listing.SqFtTotal); // Living Area
            driver.SetSelect(By.Id("Input_227"), "BUILDER"); // Living Area Source

            driver.WriteTextbox(By.Id("Input_717"), listing.GarageCapacity); // # Garage Spaces
            driver.WriteTextbox(By.Id("Input_229"), listing.GarageCapacity); // Parking Total

            driver.SetSelect(By.Id("Input_342"), listing.FacesDesc); // Direction Faces

            driver.WriteTextbox(By.Id("Input_242"), listing.Acres != null ? listing.Acres.ToString() : ""); // Lot Size Acres

            driver.WriteTextbox(By.Id("Input_241"), listing.LotSize); // Lot Dimensions (Frontage x Depth)
            driver.SetMultipleCheckboxById("Input_231", listing.ConstructionDesc); // Construction (5)
            driver.SetMultipleCheckboxById("Input_236", listing.GarageDesc.Replace("NONE", "N/A")); // Garage Description (4)
            driver.ScrollToTop();
            driver.ScrollDown(100);
            driver.SetMultipleCheckboxById("Input_239", listing.FoundationDesc); // Foundation (2)
            driver.WriteTextbox(By.Id("Input_678"), listing.OwnerName); // Builder Name
            driver.SetMultipleCheckboxById("Input_230", listing.UnitStyleDesc); // Unit Style (5)
            driver.SetMultipleCheckboxById("Input_233", listing.RoofDesc); // Roof (4)
            if(!String.IsNullOrEmpty(listing.ViewDesc))
            {
                driver.SetMultipleCheckboxById("Input_234", listing.ViewDesc.Replace("NONE", "None")); // View (4)
            }
            if (!String.IsNullOrEmpty(listing.HasWaterAccess) && listing.HasWaterAccess == "1")
                driver.SetSelect(By.Id("Input_235"), listing.DistanceToWaterAccess); // Distance to Water Access
            else
                driver.SetSelect(By.Id("Input_235"), ""); // Distance to Water Access
            driver.SetMultipleCheckboxById("Input_237", listing.WaterfrontDesc); // Waterfront Description
            driver.SetSelect(By.Id("Input_238"), listing.BodyofWater); // Water Body Name
            driver.SetMultipleCheckboxById("Input_244", listing.LotDesc); // Lot Description (4)
            driver.SetMultipleCheckboxById("Input_232", listing.FloorsDesc); // Flooring (4)
            driver.SetMultipleCheckboxById("Input_240", listing.RestrictionsDesc); // Restrictions Description (5)
        }

        /// <summary>
        /// Fills the information for the Additional Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillAdditionalInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Additional"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            if(listing.Bed1Level == "MAIN" && !listing.InteriorDesc.Contains("MSTDW"))
            {
                listing.InteriorDesc = "MSTDW," + listing.InteriorDesc;
            }

            driver.SetMultipleCheckboxById("Input_257", listing.InteriorDesc); // Interior Features (12)
            driver.ScrollToTop();
            driver.SetMultipleCheckboxById("Input_264", listing.ExteriorDesc); // Exterior Features (12)
            driver.ScrollToTop();
            driver.SetMultipleCheckboxById("Input_256", listing.AppliancesDesc); // Appliances / Equipment (12)
            driver.ScrollToTop();
            driver.SetMultipleCheckboxById("Input_267", listing.DiningRoomDesc); // Window Features
            driver.SetMultipleCheckboxById("Input_266", listing.SecurityDesc); // Security Features
            driver.ScrollDown(200);
            driver.SetMultipleCheckboxById("Input_258", "None"); // Accessibility Features
            driver.SetMultipleCheckboxById("Input_265", listing.OtherRoomDesc); // Patio and Porch Features
            driver.SetMultipleCheckboxById("Input_255", listing.LaundryLocDesc); // Laundry Location (3)
            driver.SetSelect(By.Id("Input_248"), listing.HorsesAllowed, true); // Horses
            driver.SetMultipleCheckboxById("Input_262", "None"); // Private Pool Features (On Property)
            driver.WriteTextbox(By.Id("Input_259"), listing.NumFireplaces); // # of Fireplaces
            driver.SetMultipleCheckboxById("Input_249", "None"); // Horse Amenities
            driver.SetMultipleCheckboxById("Input_260", listing.FireplaceDesc); // Fireplace Description (3)
            driver.SetMultipleCheckboxById("Input_261", listing.FenceDesc); // Fencing (4)
            driver.SetSelect(By.Id("Input_251"), "None"); // Guest Accommodations

            driver.ScrollToTop();
            driver.ScrollDown(200);
            driver.SetMultipleCheckboxById("Input_269", "NONE"); // Other Structures
            driver.SetMultipleCheckboxById("Input_251", "None"); // Guest Accommodations

            driver.ScrollToTop();
            driver.ScrollDown(400);
            driver.SetMultipleCheckboxById("Input_268", listing.CommonFeatures); // Community Features
        }

        /// <summary>
        /// Fills the information for the Documents Utility EES Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillDocumentsUtilityEESInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Documents & Utilities"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            if (driver.UploadInformation.IsNewListing)
            {
                driver.SetMultipleCheckboxById("Input_271", "None"); // Disclosures
                driver.ScrollToTop();
                driver.SetMultipleCheckboxById("Input_272", "NA"); // Documents Available
                driver.ScrollToTop();
            }
            driver.SetMultipleCheckboxById("Input_273", listing.HeatSystemDesc); // Heating
            driver.SetMultipleCheckboxById("Input_274", listing.CoolSystemDesc); // Cooling
            driver.SetMultipleCheckboxById("Input_278", listing.UtilitiesDesc); // Utilities
            driver.SetMultipleCheckboxById("Input_276", listing.SewerDesc);  // Sewer

            driver.SetMultipleCheckboxById("Input_275", listing.WaterDesc);  // Water Source
        }

        /// <summary>
        /// Fills the information for the Financial Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillFinancialInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Financial"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            driver.SetSelect(By.Id("Input_282"), listing.HasHOA); // Association YN

            if (listing.HasHOA == "1") // "Yes" Choosed in HOA
            {
                driver.WriteTextbox(By.Id("Input_283"), listing.AssocName, true); // HOA Name
                driver.WriteTextbox(By.Id("Input_285"), listing.AssocFee, true); // HOA Fee
                driver.SetSelect(By.Id("Input_286"), listing.HOA, true); // Association Requirement
                driver.SetSelect(By.Id("Input_287"), listing.AssocFeePaid, true); // HOA Frequency
                driver.WriteTextbox(By.Id("Input_288"), listing.AssocTransferFee, true); // HOA Transfer Fee    
                driver.SetMultipleCheckboxById("Input_290", listing.AssocFeeIncludes, true); // HOA Fees Include (5)
            }
            driver.SetMultipleCheckboxById("Input_291", /*driver.UploadInformation.IsNewListing ? "SEEAGT" :*/ listing.HowToSellDesc); // Acceptable Financing (5)
            driver.WriteTextbox(By.Id("Input_296"), "0"); // Estimated Taxes ($)
            driver.WriteTextbox(By.Id("Input_297"), listing.TaxYear); // Tax Year

            driver.WriteTextbox(By.Id("Input_294"), listing.TaxRate.ToString("0.##") , true); // Tax Rate
            driver.WriteTextbox(By.Id("Input_293"), "0", true); // Tax Assessed Value
            driver.SetSelect(By.Id("Input_554"), listing.TitleDesc ?? "NONE"); // Title
            driver.SetMultipleCheckboxById("Input_295", "None"); // Buyer Incentive
            driver.SetMultipleCheckboxById("Input_298", listing.ExemptionsDesc); // Tax Exemptions
            driver.ScrollDown(400);
            driver.SetMultipleCheckboxById("Input_299", "Funding"); // Possession

        }

        /// <summary>
        /// Fills the information for the Showing Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillShowingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Showing"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            driver.SetSelect(By.Id("Input_301"), "VCNT"); // Occupant
            driver.WriteTextbox(By.Id("Input_302"), listing.OwnerName); // Owner Name
            driver.SetMultipleCheckboxById("Input_305", listing.ShowingInstructions); // Showing Requirements
            driver.SetSelect(By.Id("Input_651"), listing.LockboxTypeDesc ?? "None"); // Lockbox Type
            driver.SetMultipleCheckboxById("Input_720", "OWN"); // Showing Contact Type
            driver.WriteTextbox(By.Id("Input_310"), listing.OwnerName);
            driver.WriteTextbox(By.Id("Input_311"), listing.OwnerPhone, true); // Showing Contact Phone
            driver.WriteTextbox(By.Id("Input_406"), listing.OtherPhone, true);  // Showing Service Phone

            driver.WriteTextbox(By.Id("Input_313"), listing.AccessInstructionsDesc); // Showing Instructions
        }

        /// <summary>
        /// Fills the information for the Compensation Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillCompensationInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            if (listing.CompBuy != null)
            {
                if (listing.CompBuyType != null)
                {
                    driver.SetSelect(By.Id("Input_1125"), listing.CompBuyType, true);
                }
                else
                {
                    if (listing.CompBuy.EndsWith("%"))
                    {
                        driver.SetSelect(By.Id("Input_1125"), "%", true);
                    }
                    else if (listing.CompBuy.EndsWith("$"))
                    {
                        driver.SetSelect(By.Id("Input_1125"), "$", true);
                    }
                }
                driver.WriteTextbox(By.Id("Input_909"), listing.CompBuy.Replace("%", "").Replace("$", ""));
            }

            #region UP-201
            if (!String.IsNullOrEmpty(listing.AgentBonusAmount))
            {                
                driver.SetSelect(By.Id("Input_123"), "$", true);
                driver.WriteTextbox(By.Id("Input_918"), listing.AgentBonusAmount.Replace("%", "").Replace("$", ""));
            } else
            {
                ((IJavaScriptExecutor)driver).ExecuteScript(" $('#Input_123').val(''); ");
                driver.WriteTextbox(By.Id("Input_918"), "");
            }
            #endregion

            driver.SetSelect(By.Id("Input_1220"), "%");
            driver.WriteTextbox(By.Id("Input_1221"), "0");
        }

        /// <summary>
        /// Fills the information for the Remarks tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillRemarks(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Remarks/Tours/Internet"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            driver.WriteTextbox(By.Id("Input_320"), listing.Directions); // Directions

            String bonusMessage = "";
            if (listing.BonusCheckBox.Equals(true) && listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Bonus & Buyer Incentives; ask Builder for details. ";
            else if (listing.BonusCheckBox.Equals(true))
                bonusMessage = "Possible Bonus; ask Builder for details. ";
            else if (listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Buyer Incentives; ask Builder for details. ";
            else
                bonusMessage = "";

            String realtorContactEmail = "";
            if (!String.IsNullOrEmpty(listing.EmailRealtorsContact))
                realtorContactEmail = listing.EmailRealtorsContact;
            else
                realtorContactEmail = listing.RealtorContactEmail;

            realtorContactEmail = 
                (!String.IsNullOrWhiteSpace(realtorContactEmail) && 
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";

            string message = bonusMessage + listing.GetPrivateRemarks() + realtorContactEmail;

            string incompletedBuiltNote = "";
            if (listing.YearBuiltDesc == "UC" &&
                !message.Contains("Home is under construction. For your safety, call appt number for showings"))
            {
                incompletedBuiltNote = "Home is under construction. For your safety, call appt number for showings. ";
            }

            driver.WriteTextbox(By.Id("Input_321"), "");
            driver.WriteTextbox(By.Id("Input_321"), incompletedBuiltNote + message + " " + listing.AgentPrivateRemarks2); 
            driver.ScrollDown(200);

            UpdatePublicRemarksInRemarksTab(driver, listing);
            
            if (driver.UploadInformation.IsNewListing)
            {
                driver.SetSelect(By.Id("Input_329"), "1"); // Internet
                driver.ScrollDown();
                driver.SetMultipleCheckboxById("Input_333", "AHS,HAR,DC,HSNAP,REALTOR,ZILG,HDC,HOMES,LISTHUB,ZILORTRU"); // Listing Will Appear On (4)

                driver.SetSelect(By.Id("Input_330"), "1"); // Internet Automated Valuation Display
                driver.SetSelect(By.Id("Input_331"), "1"); // Internet Consumer Comment
                driver.SetSelect(By.Id("Input_332"), "1"); // Internet Address Display
            }
        }

        private void UpdatePublicRemarksInRemarksTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            BuiltStatus status = BuiltStatus.WithCompletion;

            switch (listing.YearBuiltDesc)
            {
                case "TBD":
                case "TB":
                case "TBD,TB":
                    status = BuiltStatus.ToBeBuilt;
                    break;
                case "NW":
                case "NWCONSTR":
                    status = BuiltStatus.ReadyNow;
                    break;
                case "UC":
                    status = BuiltStatus.WithCompletion;
                    break;
            }

            //driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Id("Input_917")));
            Thread.Sleep(400);
            driver.WriteTextbox(By.Id("Input_322"), listing.GetPublicRemarks(status)); // Internet / Remarks / Desc. of Property
            driver.WriteTextbox(By.Id("Input_323"), listing.GetPublicRemarks(status)); // Syndication Remarks
        }

        /// <summary>
        /// This method makes set of values to the Longitude and Latitud fields 
        /// </summary>
        /// <param name="driver"></param>
        private void SetLongitudeAndLatitudeValues(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            if (String.IsNullOrEmpty(listing.MLSNum))
            {
                driver.WriteTextbox(By.Id("INPUT__146"), listing.Latitude); // Latitude
                driver.WriteTextbox(By.Id("INPUT__168"), listing.Longitude); // Longitude
            }
        }

        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);
            Login(driver, listing);
            QuickEdit(driver, listing);

            driver.Click(By.PartialLinkText("Price Change"));
            Thread.Sleep(1000);

            // sets new value for list price field 
            driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Id("Input_77")));
            Thread.Sleep(400);
            driver.WriteTextbox(By.Id("Input_77"), listing.ListPrice); // List Price

            return UploadResult.Success;

        }

        public UploadResult UploadLeasing(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            throw new NotImplementedException();
        }
    }
}