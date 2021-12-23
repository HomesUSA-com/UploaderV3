using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.Threading;

namespace Husa.Core.Uploaders.Austin
{
    public class AustinUploaderCTX :    IUploader, 
                                        IEditor, 
                                        IStatusUploader, 
                                        IImageUploader, 
                                        ICompletionDateUploader, 
                                        IPriceUploader, 
                                        IUpdateOpenHouseUploader,
                                        IUploadVirtualTourUploader
    {
        OpenHouseBase OH = new OpenHouseBase();

        public string RoomType { get; set; }

        public bool IsFlashRequired { get { return false; } }

        public bool CanUpload(ResidentialListingRequest listing)
        {
            //This method must return true if the listing can be uploaded with this MarketSpecific Uploader
            // UP-74
            if (listing.MarketName == "Austin CTX" && listing.ServiceSubscription > 0 && 
                !String.IsNullOrEmpty(listing.isCTX) && listing.isCTX == "1")
            {
                if (!String.IsNullOrEmpty(listing.CTXMLSNum))
                    listing.MLSNum = listing.CTXMLSNum;
                else
                    listing.MLSNum = String.Empty;
                listing.MarketName = "Austin CTX";
                if (!String.IsNullOrEmpty(listing.CTXUser) && !String.IsNullOrEmpty(listing.CTXPass))
                {
                    listing.MarketUsername = listing.CTXUser;
                    listing.MarketPassword = listing.CTXPass;
                } else
                {
                    listing.MarketUsername = "306362";
                    listing.MarketPassword = "1232";
                }

                //if(listing.CompanyName == "Highland Homes")
                //{
                //    listing.MarketUsername = "bcaballero";
                //    listing.MarketPassword = "Husa2021?";
                //}
                return true;
            }

            return false;
        }

        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            try
            {
                driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

                Login(driver, listing);

                #region navigateMenu
                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl03_m_divFooterContainer")));
                #endregion

                if (string.IsNullOrWhiteSpace(listing.MLSNum))
                    NewProperty(driver, listing);
                else
                {
                    EditProperty(driver, listing);

                    driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Residential Input Form")));
                    driver.Click(By.LinkText("Residential Input Form"));
                }

                // 1. Listing Information	
                FillListingInformation(driver, listing);

                // 2. Rooms
                FillRooms(driver, listing);

                // 3. Features
                FillFeatures(driver, listing);

                // 4. Lot/Environment/Utility Information
                FillLotEnvironmentUtilityInformation(driver, listing);

                // 5. Financial Information
                FillFinancialInformation(driver, listing);

                // 6. Showing Information
                FillShowingInformation(driver, listing);

                // 7. Remarks
                FillRemarks(driver, listing);

                //try { driver.Click(By.LinkText("Status")); } catch { }

                return UploadResult.Success;
            }
            catch (Exception e)
            {
                return UploadResult.Failure;
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
                NewProperty(driver, listing);
            else
            {
                EditProperty(driver, listing);

                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Residential Input Form")));
                driver.Click(By.LinkText("Residential Input Form"));
            }

            return UploadResult.Success;
        }

        public UploadResult UpdateCompletionDate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            Login(driver, listing);

            #region navigateMenu
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl03_m_divFooterContainer")));
            #endregion
            EditProperty(driver, listing);
            
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Residential Input Form")));
            driver.Click(By.LinkText("Residential Input Form"));

            UpdateYearBuiltDescriptionInGeneralTab(driver, listing);

            driver.Click(By.LinkText("Remarks"));

            UpdatePublicRemarksInRemarksTab(driver, listing);

            return UploadResult.Success;
        }

        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);

            #region navigateMenu
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl03_m_divFooterContainer")));
            #endregion

            EditProperty(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Residential Input Form")));
            driver.Click(By.LinkText("Residential Input Form"));

            //Login(driver, listing);
            //QuickEdit(driver, listing);
            ////  var expirationDate = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1);
            //Thread.Sleep(1500);
            //switch (listing.ListStatus)
            //{
            //    #region Sold
            //    case "S":

            //        driver.Click(By.PartialLinkText("Change to Sold"));
            //        Thread.Sleep(500);
            //        driver.WriteTextbox(By.Id("Input_1029"), listing.PendingDate);
            //        driver.WriteTextbox(By.Id("Input_1023"), listing.ClosedDate);
            //        driver.WriteTextbox(By.Id("Input_1034"), listing.SalesPrice);
            //        /*QLIST-85*/
            //        //driver.WriteTextbox(By.Id("Input_1032"), listing.RepairsAmount);
            //        driver.WriteTextbox(By.Id("Input_1032"), "0"); //RepairsAmount

            //        //driver.WriteTextbox(By.Id("Input_1314"), listing.Loan1Amount);
            //        driver.WriteTextbox(By.Id("Input_1315"), listing.BuyersClsgCostPdbySell);
            //        driver.WriteTextbox(By.Id("Input_1033"), listing.SellerPoints);
            //        driver.WriteTextbox(By.Id("Input_1026"), listing.BuyerPoints);
            //        driver.WriteTextbox(By.Id("Input_1037"), "0");
            //        /*QLIST-85*/
            //        //driver.SetSelect(By.Id("Input_1142"), listing.PropConditionSale);
            //        driver.SetSelect(By.Id("Input_1142"), "EXCL"); //Condition Sale
            //        //driver.WriteTextbox(By.Id("Input_1036"), listing.SoldComments);

            //        driver.WriteTextbox(By.Id("Input_1043"), listing.SellingAgentLicenseNum ?? "NONMBR");
            //        driver.SetMultipleCheckboxById("Input_1035", listing.SoldTerms);

            //        break;

            //    #endregion

            //    #region Pending
            //    case "P":
            //        driver.Click(By.PartialLinkText("Change to Pending"));
            //        Thread.Sleep(500);
            //        driver.WriteTextbox(By.Id("Input_1056"), listing.PendingDate);
            //        driver.WriteTextbox(By.Id("Input_1063"), listing.EstClosedDate);
            //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

            //        break;
            //    #endregion

            //    #region Pending Taking Backups

            //    case "PB":
            //        driver.Click(By.LinkText("Change to Pending - Taking Backups"));

            //        driver.WriteTextbox(By.Id("Input_1067"), listing.PendingDate);
            //        driver.WriteTextbox(By.Id("Input_1352"), listing.EstClosedDate);
            //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);
            //        break;

            //    #endregion

            //    #region Active

            //    case "A":
            //        driver.Click(By.LinkText("Change to Active"));
            //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

            //        break;
            //    #endregion

            //    #region Active Contingent

            //    case "AC":
            //        driver.Click(By.LinkText("Change to Active Contingent"));
            //        Thread.Sleep(500);
            //        driver.WriteTextbox(By.Id("Input_1006"), listing.ContingencyDate);
            //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

            //        break;
            //    #endregion

            //    #region Temporarily Off Market

            //    case "T":
            //        driver.Click(By.LinkText("Change to Temporarily Off Market"));
            //        Thread.Sleep(500);
            //        driver.WriteTextbox(By.Id("Input_1018"), DateTime.Now.ToShortDateString());
            //        driver.WriteTextbox(By.Id("Input_1355"), listing.ExpiredDate);

            //        break;
            //    #endregion

            //    #region Withdrawn

            //    case "W":
            //        driver.Click(By.PartialLinkText("Change to Withdrawn"));
            //        Thread.Sleep(500);
            //        driver.WriteTextbox(By.Id("Input_1020"), DateTime.Now.ToShortDateString());

            //        break;
            //    #endregion

            //    default:
            //        throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for Austin Listing with Id '" + listing.ResidentialListingID + "'");
            //}

            return UploadResult.Success;
        }

        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            QuickEdit(driver, listing);

            // Enter Manage Photos
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Manage Photos")));
            driver.Click(By.LinkText("Manage Photos"));

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
        #endregion

        public UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);
            QuickEdit(driver, listing);

            Thread.Sleep(1000);

            driver.Click(By.PartialLinkText("Open Houses"));

            Thread.Sleep(1000);

            DeleteOpenHouses(driver, listing);
            AddOpenHouses(driver, listing);

            return UploadResult.Success;
        }
        
        #region Open House

        public void DeleteOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript("jQuery('html,body').animate({ scrollTop: 0, scrollLeft: 500 });");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Add/Edit")));
            driver.Click(By.LinkText("Add/Edit"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")));
            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.Click(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")); // Link "Edit"
            driver.Click(By.Id("m_dlInputList_ctl01_m_btnSelect"));

            while (driver.FindElements(By.LinkText("Delete")).Count() > 1)
            {
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
            }
            driver.ScrollDown();
            driver.ExecuteScript("jQuery('html,body').animate({ scrollLeft: 0 });");
            Thread.Sleep(1000);
            driver.Click(By.Id("m_lbSubmit"));
            Thread.Sleep(500);
            driver.ExecuteScript("jQuery('html,body').animate({ scrollTop: 0, scrollLeft: 500 });");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Add/Edit")));
            driver.Click(By.LinkText("Add/Edit"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")));
            driver.WriteTextbox(By.Id("m_lvInputUISections_ctrl0_tbQuickEditCommonID_m_txbInternalTextBox"), listing.MLSNum);
            driver.Click(By.Id("m_lvInputUISections_ctrl0_lbQuickEdit")); // Link "Edit"
            driver.Click(By.Id("m_dlInputList_ctl01_m_btnSelect"));
            Thread.Sleep(500);
        }

        public void AddOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(200);
            driver.ScrollDown();
            int max = 10;
            List<DateTime> date = OH.getNextDate(listing, max);

            int i = 0;
            foreach (var local in date)
            {
                string[] openhousestart;
                string[] openhouseend;

                string day = local.DayOfWeek.ToString().Substring(0, 3);
                if (listing.GetType().GetProperty("OHStartTime" + day + "OH").GetValue(listing, null) != null && listing.GetType().GetProperty("OHEndTime" + day + "OH").GetValue(listing, null) != null && listing.GetType().GetProperty("OHType" + day + "OH").GetValue(listing, null) != null)
                {
                    openhousestart = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHStartTime" + day + "OH").GetValue(listing, null).ToString());
                    openhouseend = OH.GetOpenHouseTime(listing.GetType().GetProperty("OHEndTime" + day + "OH").GetValue(listing, null).ToString());

                    // 	Open House Date
                    driver.WriteTextbox(By.Id("_Input_349__REPEAT" + i + "_342"), local.ToShortDateString());

                    // From Time
                    driver.WriteTextbox(By.Id("_Input_349__REPEAT" + i + "_TextBox_344"), openhousestart[0]);
                    if (openhousestart[1] == "AM")
                        driver.SetRadioButton(By.Id("_Input_349__REPEAT" + i + "_RadioButtonList_344_0"), openhousestart[1]);
                    else
                        driver.SetRadioButton(By.Id("_Input_349__REPEAT" + i + "_RadioButtonList_344_1"), openhousestart[1]);

                    // 	To Time
                    driver.WriteTextbox(By.Id("_Input_349__REPEAT" + i + "_TextBox_345"), openhouseend[0]);
                    if (openhouseend[1] == "AM")
                        driver.SetRadioButton(By.Id("_Input_349__REPEAT" + i + "_RadioButtonList_345_0"), openhouseend[1]);
                    else
                        driver.SetRadioButton(By.Id("_Input_349__REPEAT" + i + "_RadioButtonList_345_1"), openhouseend[1]);

                    // 	Active Status?
                    //if (listing.GetType().GetProperty("OHType" + day).GetValue(listing, null) != null)
                    //    driver.SetSelect(By.Id("_Input_349__REPEAT" + i + "_346"), listing.GetType().GetProperty("OHType" + day).GetValue(listing, null).ToString());


                    //if (listing.GetType().GetProperty("OHRefreshments" + day).GetValue(listing, null) != null)
                    //    driver.SetMultipleCheckboxById("_Input_349__REPEAT" + i + "_1227", listing.GetType().GetProperty("OHRefreshments" + day).GetValue(listing, null).ToString());

                    // Comments 
                    if (listing.GetType().GetProperty("OHComments" + day + "OH").GetValue(listing, null) != null)
                        driver.WriteTextbox(By.Id("_Input_349__REPEAT" + i + "_348"), listing.GetType().GetProperty("OHComments" + day + "OH").GetValue(listing, null));

                    i++;

                    if (max >= (i + 1))
                    {
                        driver.ScrollDown();
                        driver.ScrollDown();
                        driver.ExecuteScript("jQuery('html,body').animate({ scrollLeft: 1000 });");
                        Thread.Sleep(1000);
                        driver.Click(By.LinkText("More"));
                    }
                }
            }
            driver.ScrollDown();
            driver.ExecuteScript("jQuery('html,body').animate({ scrollLeft: 0 });");
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

            driver.Click(By.LinkText("Remarks"));
            Thread.Sleep(200);
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl02_m_divFooterContainer")));

            var virtualTour = media.OfType<ResidentialListingVirtualTour>().FirstOrDefault();
            if (virtualTour != null)
                driver.WriteTextbox(By.Id("Input_1189"), virtualTour.VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded

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
            driver.Navigate("http://matrix.ctxmls.com/Matrix/Logout.aspx");
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
            driver.Navigate("http://matrix.ctxmls.com/Matrix/login.aspx");

            driver.ExecuteScript("jQuery('#clareity').focus(); jQuery('#clareity').text('" + listing.MarketUsername + "');");
            driver.ExecuteScript("jQuery('#form-clareity').val('" + listing.MarketUsername + "');");

            driver.ExecuteScript("jQuery('#security').text('" + listing.MarketPassword + "');");
            driver.ExecuteScript("jQuery('#form-security').val('" + listing.MarketPassword + "');");
            
            driver.ExecuteScript(" inputCheck(null); ");

            #endregion

            Thread.Sleep(2000);

            #region loadingMLS
            // Wait until login page has been loaded.

            //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("footer")));

            // Use the same browser page NOT _blank
            driver.Navigate("https://matrix.ctxmls.com/Matrix/Default.aspx?c=AAEAAAD*****AQAAAAAAAAARAQAAAEMAAAAGAgAAAAQxMzM3DT8GAwAAAAjClsK8w5vCqg0CCw))&f=");
            #endregion

            Thread.Sleep(2000);

            #region ReadyToWork
            if (ExpectedConditions.ElementIsVisible(By.Id("NewsDetailDismiss")) == null)
            {
                driver.Click(By.Id("NewsDetailDismiss"));
            }

            try { driver.Click(By.LinkText("Skip"), true); } catch { }

            try { driver.ExecuteScript("jQuery('#NewsDetailDismissNew').click();"); } catch { }
            
            Thread.Sleep(2000);

            #endregion

            return LoginResult.Logged;
        }

        private void NewProperty(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            #region newProperty

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Input")));
            driver.Click(By.LinkText("Input"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Add new")));
            driver.Click(By.LinkText("Add new"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Residential Input Form")));
            driver.Click(By.LinkText("Residential Input Form"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText("Start with a blank Listing")));
            driver.Click(By.PartialLinkText("Start with a blank Listing"));

            Thread.Sleep(1000);
            #endregion
        }

        private void EditProperty(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            #region editProperty
            QuickEdit(driver, listing);
            #endregion
        }

        private void QuickEdit(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Input")));
            driver.Click(By.LinkText("Input"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Edit existing")));
            driver.Click(By.LinkText("Edit existing"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_txtSourceCommonID")));
            driver.WriteTextbox(By.Id("m_txtSourceCommonID"), listing.MLSNum);
            driver.Click(By.Id("m_lbEdit")); // "Modify button"
        }

        ///// <summary>
        ///// Fills the information for the Listing Information tab.
        ///// </summary>
        ///// <param name="driver">Webdriver Element for the current upload</param>
        ///// <param name="listing">Current listing being processed</param>
        //private void FillListingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        //{
        //    driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
        //    #region listingInformation

        //    driver.Click(By.LinkText("General"));
        //    Thread.Sleep(400);

        //    // Listing Information
        //    driver.SetSelect(By.Id("Input_140"), "EA"); // List Agreement Type
        //    driver.SetSelect(By.Id("Input_141"), "0"); // Foreclosure/REO
        //    driver.WriteTextbox(By.Id("Input_146"), listing.ListPrice); // List Price
        //    driver.SetSelect(By.Id("Input_142"), "A"); // List Agreement Document
        //    driver.SetSelect(By.Id("Input_150"), "None"); // Sales Restrictions (1)
        //    if (driver.UploadInformation.IsNewListing)
        //    {
        //        //driver.WriteTextbox(By.Id("Input_113"), DateTime.Now.ToShortDateString()); // List Date
        //        driver.WriteTextbox(By.Id("Input_114"), DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
        //    }
        //    #endregion
        //}

        /// <summary>
        /// Fills the information for the Listing Information tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillListingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.Click(By.LinkText("Listing Information")); // click in tab Listing Information
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_554"))); // the last field on the form

            #region Location Information
            driver.WriteTextbox(By.Id("Input_107"), listing.StreetNum); // Street Number
            driver.SetSelect(By.Id("Input_108"), listing.StreetDir); // St Direction
            driver.WriteTextbox(By.Id("Input_110"), listing.StreetName); // Street Name
            driver.SetSelect(By.Id("Input_109"), listing.StreetType); // Street Type
            driver.WriteTextbox(By.Id("Input_111"), listing.UnitNum); // Unit #
            driver.SetSelectByText(By.Id("Input_112"), listing.City); // City
            driver.SetSelect(By.Id("Input_113"), listing.StateCode); // State  -- TEST: verify if must to use State or StateCode property
            driver.WriteTextbox(By.Id("Input_114"), listing.Zip); // Zip Code
            driver.SetSelectByText(By.Id("Input_115"), listing.County); // County
            driver.WriteTextbox(By.Id("Input_396"), listing.Subdivision); // Subdivision
            driver.WriteTextbox(By.Id("Input_528"), listing.Legal); // Legal Description
            driver.WriteTextbox(By.Id("Input_529"), listing.TaxID); // Property ID
            driver.WriteTextbox(By.Id("Input_766"), listing.CTXGeoID); // Geo ID
                                                                       // driver.WriteTextbox(By.Id("Input_120"), ??? ); // Lot
                                                                       // driver.WriteTextbox(By.Id("Input_121"), ??? ); // Block

            //School District 
            driver.WriteTextbox(By.Id("Input_535_TB"), listing.SchoolDistrict); 
            driver.SetSelect(By.Id("Input_123"), "YES" ); // In City Limits

            string CTXETJ = "0";
            if (listing.CTXETJ == true)
                CTXETJ = "1";
            driver.SetSelect(By.Id("Input_124"), CTXETJ); // ETJ
            driver.SetSelect(By.Id("Input_406"), listing.SchoolName1.ToUpper()); // Elementary School
            driver.SetSelect(By.Id("Input_407"), listing.SchoolName3.ToUpper()); // Middle School
            driver.SetSelect(By.Id("Input_405"), listing.SchoolName6.ToUpper()); // High School
            //driver.WriteTextbox(By.Id("Input_125"), listing.MapscoMapCoord); // Map Grid
            //driver.WriteTextbox(By.Id("Input_126"), listing.MapscoMapPage); // Map Source
            SetLongitudeAndLatitudeValues(driver, listing);
            #endregion Location Information

            #region Listing Information
            driver.WriteTextbox(By.Id("Input_127"), listing.ListPrice); // List Price

            // 'Garden_Patio_Home', 'Garden\/Patio Home',
            // 'MNFHO', 'Manufactured Home',
            // 'Modular', 'Modular',
            // 'SFM', 'Single Family',
            String PropSubType = "";
            switch (listing.PropSubType)
            {
                case "HALFDUPLX":
                    PropSubType = "Modular";
                    break;
                case "CONAT":
                    PropSubType = "CNDMI";
                    break;
                case "2PLEX":
                    PropSubType = "Modular";
                    break;
                case "4PLEX":
                    PropSubType = "Modular";
                    break;
                case "HOUSE":
                    PropSubType = "SFM";
                    break;
                case "OTHER":
                    PropSubType = "OTH";
                    break;
                case "TOWNHOUS":
                    PropSubType = "TNX";
                    break;
                case "3PLEX":
                    PropSubType = "Modular";
                    break;
                default:
                    PropSubType = "SFM";
                    break;
            }
            driver.SetSelect(By.Id("Input_539"), PropSubType); // Property Type

            if (driver.UploadInformation.IsNewListing)
            {
                DateTime listDate = DateTime.Now;
                if (listing.ListStatus == "P" || listing.ListStatus == "PO")
                {
                    listDate = DateTime.Now.AddDays(-2);
                }
                else if (listing.ListStatus == "S")
                {
                    listDate = DateTime.Now.AddDays(-4);
                }

                driver.WriteTextbox(By.Id("Input_129"), listDate.ToShortDateString()); // List Date TODO: verify this logic
            }

            if(listing.ListDate != null)
                driver.WriteTextbox(By.Id("Input_130"), DateTime.Now.AddYears(1).ToShortDateString()); // Expiration Date
            else
                driver.WriteTextbox(By.Id("Input_130"), (listing.ExpiredDate != null ? ((DateTime)listing.ExpiredDate).ToShortDateString() : "")); // Expiration Date
            driver.SetSelect(By.Id("Input_544"), "NA"); // First Right Refusal Option (default hardcode "N/A")
            //driver.SetSelect(By.Id("Input_132"), "1"); // Owner LREA (default hardcode "Yes")
            driver.WriteTextbox(By.Id("Input_133"), listing.OwnerName); // Owner Legal Name
            //driver.WriteTextbox(By.Id("Input_134"), listing.OwnerPhone.Replace("(","").Replace(")", "").Replace("-", "").Replace(" ","")); // Owner Phone
            //driver.WriteTextbox(By.Id("Input_134"), listing.OwnerPhone.Remove('(').Remove(')').Remove('-').Trim()); // Owner Phone
            driver.SetSelect(By.Id("Input_135"), "NO"); // Short Sale
            driver.SetSelect(By.Id("Input_136"), "0"); // Foreclosure
            driver.SetSelect(By.Id("Input_137"), "0" ); // Also For Rent
            // driver.WriteTextbox(By.Id("Input_138"), ??? ); // Additional MLS #
            //driver.WriteTextbox(By.Id("Input_139"), listing.TaxRate); // Total Tax Rate
            #endregion Listing Information

            #region General Listing Information
            //driver.SetSelect(By.Id("Input_183"), "0"); // Res Flooded

            String constructionStatus = "";
            switch (listing.YearBuiltDesc)
            {
                case "NWCONSTR":
                case "NW":
                    constructionStatus = "COMPL";
                    break;
                case "TB":
                    constructionStatus = "TOBEB";
                    break;
                case "UC":
                    constructionStatus = "UNDER";
                    break;
            }
            
            driver.SetSelect(By.Id("Input_547"), constructionStatus); // Construction Status
            driver.WriteTextbox(By.Id("Input_553"), listing.YearBuilt); // Year Built
            driver.SetSelect(By.Id("Input_552"), "OWNSE"); // Year Built Source (default hardcode "Owner/Seller")
            driver.WriteTextbox(By.Id("Input_548"), listing.OwnerName); // Builder Name
            driver.WriteTextbox(By.Id("Input_550"), listing.SqFtTotal); // Total SqFt
            driver.SetSelect(By.Id("Input_551"), "BUILD"); // Source SqFt
            //driver.SetMultipleCheckboxById("Input_554", listing.RestrictionsDesc); // Documents on File
            #endregion 	General Listing Information
        }

        /// <summary>
        /// Fills the information for the Rooms tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillRooms(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript(" jQuery(document).scrollTop(0);");

            driver.Click(By.LinkText("Rooms")); // click in tab Listing Information
            //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_189t"))); // the last field on the form

            //driver.Click(By.Id("m_rpPageList_ctl04_lbPageLink")); // Tab: Input | Subtab: Rooms
            //driver.ExecuteScript(" __doPostBack('m_rpPageList$ctl02$lbPageLink', '');");

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer"))); // Look if the footer elements has been loaded

            driver.WriteTextbox(By.Id("Input_193"), (listing.NumBedsMainLevel != null ? listing.NumBedsMainLevel : 0 ) + (listing.NumBedsOtherLevels != null ? listing.NumBedsOtherLevels : 0)); // Bedrooms
            //driver.WriteTextbox(By.Id("Input_193"), listing.Beds); // Bedrooms
            driver.WriteTextbox(By.Id("Input_194"), listing.BathsFull); // Full Baths
            driver.WriteTextbox(By.Id("Input_195"), listing.BathsHalf); // Half Baths

            //driver.WriteTextbox(By.Id("Input_260"), listing.NumStories); // # Stories

            driver.WriteTextbox(By.Id("Input_196"), listing.NumLivingAreas); // # Living Areas

            driver.WriteTextbox(By.Id("Input_197"), listing.NumDiningAreas); // # Dining Areas
            
            List<String> putGarageDesc = new List<String>();
            List<String> optionsGarageDesc = String.IsNullOrWhiteSpace(listing.GarageDesc) ? new List<String>() : listing.GarageDesc.Split(',').ToList();
            foreach (string option in optionsGarageDesc)
            {
                switch (option)
                {
                    case "NONE":
                        putGarageDesc.Add("NONE");
                        break;
                }
            }
            // Market options that doesn't match
            //'OTHSE','Other-See Remarks',
            //'NONE','None'

            if (putGarageDesc.Count == 0 && listing.GarageCapacity > 0)
            {
                driver.SetSelect(By.Id("Input_198"), "1"); // Garage/Carport
            }
            else
            {
                driver.SetSelect(By.Id("Input_198"), "0"); // Garage/Carport
            }

            string guestHouse = "0";
            if (listing.CTXGuestHouse == true)
                guestHouse = "1";

            driver.SetSelect(By.Id("Input_199"), guestHouse); // Garage/Carport

            if (!driver.UploadInformation.IsNewListing)
            {
                var elems = driver.FindElements(By.CssSelector("table[id^=_Input_556__del_REPEAT] a"));

                foreach (var elem in elems.Where(c => c.Displayed))
                    elem.Click();
            }

            var roomTypes = ReadRoomAndFeatures(listing);

            //driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink"));
            Thread.Sleep(400);

            var i = 0;

            //foreach (var roomType in roomTypes.Where(c => c.IsValid()))
            foreach (var roomType in roomTypes)
            {
                if (i > 0)
                {
                    driver.Click(By.Id("_Input_556_more"));
                    Thread.Sleep(400);
                }

                //'Atrium', 'Atrium',
                //'Basement', 'Basement',
                //'BEDRO', 'Bedroom',
                //'Bedroom_II', 'Bedroom II',
                //'Bedroom_III', 'Bedroom III',
                //'Bedroom_IV', 'Bedroom IV',
                //'BonusRoom', 'Bonus Room',
                //'BKFRO', 'Breakfast Room',
                //'Converted_Garage', 'Converted Garage',
                //'DININ', 'Dining Room',
                //'Entry_Forer', 'Entry\/Foyer',
                //'FAMIL', 'Family Room',
                //'GAMER', 'Game Room',
                //'GreatRoom', 'Great Room',
                //'Gym', 'Gym',
                //'KITCH', 'Kitchen',
                //'Library', 'Library',
                //'LIVIN', 'Living Room',
                //'Living_Room_II', 'Living Room II',
                //'Loft', 'Loft',
                //'Master_Bath', 'Master Bath',
                //'Master_Bath_II', 'Master Bath II',
                //'MSTRB', 'Master Bedroom',
                //'Master_Bedroom_II', 'Master Bedroom II',
                //'MediaRoom', 'Media Room',
                //'OFFIC', 'Office',
                //'OTHER', 'Other',
                //'Other_Room', 'Other Room',
                //'Other_Room_II', 'Other Room II',
                //'Other_Room_III', 'Other Room III',
                //'SaunaRoom', 'Sauna Room',
                //'UTILI', 'Utility\/Laundry ',
                //'Wine', 'Wine',
                //'WORKS', 'Workshop        ',
                //'GuestHse', 'Guest House'
                
                driver.SetSelect(By.Id("_Input_556__REPEAT" + i + "_190"), roomType, true); // FieldName
                Thread.Sleep(400);
                //driver.ScrollDown();
                //driver.SetSelect(By.Id("_Input_556__REPEAT" + i + "_491"), roomType.Level, true);
                //Thread.Sleep(400);
                //driver.ScrollDown();
                //driver.WriteTextbox(By.Id("_Input_556__REPEAT" + i + "_191"), roomType.Length, true);
                //Thread.Sleep(400);
                //driver.ScrollDown();

                i++;
            }
        }

        /// <summary>
        /// Fills the information for the Features tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillFeatures(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript(" jQuery(document).scrollTop(0);");

            driver.Click(By.LinkText("Features")); // click in tab Features
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_657"))); // the last field on the form
            
            driver.SetMultipleCheckboxById("Input_557", "TRADI"); // Style (default hardcode "Traditional")
            //driver.SetMultipleCheckboxById("Input_157", listing.ConstructionDesc); // Construction/Exterior
            List<String> putOptionsConstructionDesc = new List<String>();
            List<String> optionsConstructionDesc = String.IsNullOrWhiteSpace(listing.ConstructionDesc) ? new List<String>() : listing.ConstructionDesc.Split(',').ToList();
            foreach (string option in optionsConstructionDesc)
            {
                switch (option)
                {
                    case "ADB":
                        putOptionsConstructionDesc.Add("ADOBE");
                        break;
                    case "ALTBSYS":
                        putOptionsConstructionDesc.Add("Alt_Bldg_Sys");
                        break;
                    case "ALUMSID":
                        putOptionsConstructionDesc.Add("Alluminum_Siding");
                        break;
                    case "ASIDEMAS":
                        //putOptionsConstructionDesc.Add("");
                        break;
                    case "BRICK":
                        putOptionsConstructionDesc.Add("BRICK");
                        break;
                    case "CLAPBRD":
                        putOptionsConstructionDesc.Add("Clapboard");
                        break;
                    case "COMPSHIN":
                       //putOptionsConstructionDesc.Add("");
                        break;
                    case "CONCBLOC":
                        putOptionsConstructionDesc.Add("Concrete_Block");
                        break;
                    case "DBLWIDE":
                        //putOptionsConstructionDesc.Add("");
                        break;
                    case "FRAME":
                        putOptionsConstructionDesc.Add("Frame");
                        break;
                    case "FRMBRICK":
                        //putOptionsConstructionDesc.Add("");
                        break;
                    case "FRMSTONE":
                        putOptionsConstructionDesc.Add("Frame_Stone");
                        break;
                    case "HARDI":
                        putOptionsConstructionDesc.Add("HARDI");
                        break;
                    case "INDUST":
                        //putOptionsConstructionDesc.Add("");
                        break;
                    case "LOG":
                        putOptionsConstructionDesc.Add("LOG");
                        break;
                    case "METALSID":
                        putOptionsConstructionDesc.Add("METAL");
                        break;
                    case "MODULAR":
                        //putOptionsConstructionDesc.Add("");
                        break;
                    case "NATBLDG":
                        putOptionsConstructionDesc.Add("Natural_Bldg");
                        break;
                    case "SNGLWIDE":
                        //putOptionsConstructionDesc.Add("");
                        break;
                    case "STEEL":
                        putOptionsConstructionDesc.Add("STEEL");
                        break;
                    case "STONE":
                        putOptionsConstructionDesc.Add("ROCKS");
                        break;
                    case "STUCCO":
                        putOptionsConstructionDesc.Add("STUCC");
                        break;
                    case "TRPLWIDE":
                        //putOptionsConstructionDesc.Add("");
                        break;
                    case "VERTICAL":
                        putOptionsConstructionDesc.Add("Verticle_Siding");
                        break;
                    case "VINYL":
                        putOptionsConstructionDesc.Add("VINYL");
                        break;
                    case "WOODSHIN":
                        putOptionsConstructionDesc.Add("WOOD");
                        break;
                }
                // Market options that doesn't match
                //'4SIDE','4-Side Masonry',
                //'ASBES','Asbestos Siding',
                //'BRIVE','Brick_Veneer',
                //'CONCR','Concrete&Fiber Plank',
                //'MASON','Masonry&Steel',
                //'Metal_Siding','Metal Siding',
                //'Metal_Structure','Metal Structure',
                //'METAL','Metal\/Aluminum',
                //'NONE','None',
                //'OTHSE','Other-See Remarks',
                //'Wood_Shingle','Wood Shingle'
            }

            // Construction Materials (Max 18)
            if (putOptionsConstructionDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_561", string.Join(",", putOptionsConstructionDesc.ToArray()));
            }

            //driver.SetMultipleCheckboxById("Input_158", listing.RoofDesc); // Roof / Attic
            List<String> putOptionsRoofDesc = new List<String>();
            List<String> optionsRoofDesc = String.IsNullOrWhiteSpace(listing.RoofDesc) ? new List<String>() : listing.RoofDesc.Split(',').ToList();
            foreach (string option in optionsRoofDesc)
            {
                switch (option)
                {
                    case "COMPSHIN":
                        putOptionsRoofDesc.Add("SHNGC");
                        break;
                    case "CONCR":
                        putOptionsRoofDesc.Add("CONCR");
                        break;
                    case "FBRCMNT":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "FLAT":
                        putOptionsRoofDesc.Add("FLAT");
                        break;
                    case "GGARDN":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "MANS":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "METAL":
                        putOptionsRoofDesc.Add("METAL");
                        break;
                    case "MIXD":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "OVRLAY":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "PITCHED":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "RUBBER":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "SLATE-IM":
                        putOptionsRoofDesc.Add("SLATE");
                        break;
                    case "TARGRVL":
                        //putOptionsRoofDesc.Add("");
                        break;
                    case "TILE":
                        putOptionsRoofDesc.Add("TILE");
                        break;
                    case "WOODSHIN":
                        putOptionsRoofDesc.Add("SHNGW");
                        break;
                }
            }
            // Market options that doesn't match
            //'1SIDE','1-Side Masonry',
            //'3SIDE','3-Side Masonry',
            //'ACCES','Access Only',
            //'DECKI','Decking',
            //'EXPAN','Expandable',
            //'FINIS','Finished',
            //'FLOOR','Floored',
            //'GRAVE','Gravel',
            //'LOG','Log',
            //'NONEA','None-Attic',
            //'NONER','None-Roof',
            //'OTHAT','Other Attic-See Remarks',
            //'OTHRO','Other Roof-See Remarks',
            //'PRTFI','Partially Finished',
            //'PRTFL','Partially Floored',
            //'PERMA','Permanent Stairs',
            //'PULLD','Pull Down Stairs',
            //'RADIA','Radiant Barrier Decking',
            //'SHNGC','Shingle-Composition',
            //'SHNGW','Shingle-Wood',
            //'STORA','Storage Only',

            if (putOptionsRoofDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_559", string.Join(",", putOptionsRoofDesc.ToArray()));
            }

            //driver.SetMultipleCheckboxById("Input_159", listing.FoundationDesc); // Foundation
            List<String> putFoundation = new List<String>();
            List<String> optionsFoundation = String.IsNullOrWhiteSpace(listing.FoundationDesc) ? new List<String>() : listing.FoundationDesc.Split(',').ToList();
            foreach (string option in optionsFoundation)
            {
                switch (option)
                {
                    case "ONSTILTS":
                        //putFoundation.Add("");
                        break;
                    case "PIER":
                        putFoundation.Add("PILLAR");
                        break;
                    case "PILINGS":
                        //putFoundation.Add("");
                        break;
                    case "SLAB":
                        putFoundation.Add("SLAB");
                        break;
                   
                }
            }
            // Market options that doesn't match
            //'BASEM','Basement',
            //'CEDAR','Cedar Post',
            //'NONE','None',
            //'OTHSE','Other-See Remarks',
            //'PIER','Pier',

            if (putFoundation.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_558", string.Join(",", putFoundation.ToArray())); // Foundation
            }

            if(listing.NumStories != null)
                driver.SetMultipleCheckboxById("Input_563", listing.NumStories.ToString()); // # Stories

            //driver.SetMultipleCheckboxById("Input_163", listing.FireplaceDesc); // Fireplace
            List<String> putOptionsFireplaceDesc = new List<String>();
            List<String> optionsFireplaceDesc = String.IsNullOrWhiteSpace(listing.FireplaceDesc) ? new List<String>() : listing.FireplaceDesc.Split(',').ToList();
            foreach (string option in optionsFireplaceDesc)
            {
                switch (option)
                {
                    case "FBDRM":
                        putOptionsFireplaceDesc.Add("BEDRO");
                        break;
                    case "FBTHR":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "FDNRM":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "FFLRM":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "FFMRM":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "FGMRM":
                        putOptionsFireplaceDesc.Add("GAMER");
                        break;
                    case "FGTRM":
                        putOptionsFireplaceDesc.Add("GREAT");
                        break;
                    case "FGUST":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "FHEAT":
                        putOptionsFireplaceDesc.Add("HEATI");
                        break;
                    case "FKTCH":
                        putOptionsFireplaceDesc.Add("KITCH");
                        break;
                    case "FLBRM":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "FLVRM":
                        putOptionsFireplaceDesc.Add("LIVIN");
                        break;
                    case "FNONF":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "FRWDS":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "GLDOR":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "GLLHT":
                        putOptionsFireplaceDesc.Add("GASLO");
                        break;
                    case "LGLHT":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "OUT":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "STHRU":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                    case "WDBRN":
                        putOptionsFireplaceDesc.Add("WOODB");
                        break;
                    case "WDSTI":
                        //putOptionsFireplaceDesc.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'ELECT','Electric',
            //'GASST','Gas Starter',
            //'GLASS','Glass\/Enclosed Screen',
            //'NONE','None',
            //'ONE','One',
            //'OTHSE','Other-See Remarks',
            //'PREFA','Prefab',
            //'STONE','Stone\/Rock\/Brick',
            //'STOVE','Stove Insert',
            //'THREE','Three+',
            //'TWO','Two',

            if (putOptionsFireplaceDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_562", string.Join(",", putOptionsFireplaceDesc.ToArray()));
            }

            //driver.SetMultipleCheckboxById("Input_160", listing.FloorsDesc); // Flooring
            List<String> putOptionsFloorsDesc = new List<String>();
            List<String> optionsFloorsDesc = String.IsNullOrWhiteSpace(listing.FloorsDesc) ? new List<String>() : listing.FloorsDesc.Split(',').ToList();
            foreach (string option in optionsFloorsDesc)
            {
                switch (option)
                {
                    case "BAMBOO":
                        //putOptionsFloorsDesc.Add("");
                        break;
                    case "BRICK":
                        putOptionsFloorsDesc.Add("BRICK");
                        break;
                    case "CARPET":
                        putOptionsFloorsDesc.Add("CARPE");
                        break;
                    case "CONCRETE":
                        putOptionsFloorsDesc.Add("CONCR");
                        break;
                    case "CORK":
                        //putOptionsFloorsDesc.Add("");
                        break;
                    case "LAMINATE":
                        putOptionsFloorsDesc.Add("LAMIN");
                        break;
                    case "LINOLEUM":
                        //putOptionsFloorsDesc.Add("");
                        break;
                    case "MARBLE":
                        putOptionsFloorsDesc.Add("MARBL");
                        break;
                    case "NOCARPET":
                        //putOptionsFloorsDesc.Add("");
                        break;
                    case "PARQUET":
                        putOptionsFloorsDesc.Add("PARQU");
                        break;
                    case "SLATE":
                        putOptionsFloorsDesc.Add("SLATE");
                        break;
                    case "STONE":
                        putOptionsFloorsDesc.Add("STONE");
                        break;
                    case "SVINYL":
                        ///putOptionsFloorsDesc.Add("");
                        break;
                    case "TERRAZZO":
                        putOptionsFloorsDesc.Add("TERRA");
                        break;
                    case "TILE":
                        putOptionsFloorsDesc.Add("TILE");
                        break;
                    case "VINYTILE":
                        putOptionsFloorsDesc.Add("VINYL");
                        break;
                    case "WOOD":
                        putOptionsFloorsDesc.Add("WOOD");
                        break;
                    case "WOODCRPT":
                        //putOptionsFloorsDesc.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'NONE','None',
            //'OTHSE','Other-See Remarks',

            if (putOptionsFloorsDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_565", string.Join(",", putOptionsFloorsDesc.ToArray()));
            }

            //driver.SetMultipleCheckboxById("Input_161", listing.NumStories); // # Stories
            List<String> putOptionsNumStories = new List<String>();
            //List<String> optionsNumStories = String.IsNullOrWhiteSpace(listing.NumStories) ? new List<String>() : listing.NumStories.Split(',').ToList();
            //foreach (string option in optionsNumStories)
            //{
            //    switch (option)
            //    {
            //        case "1":
            //            putOptionsNumStories.Add("ONE");
            //            break;
            //        case "2":
            //            putOptionsNumStories.Add("TWO");
            //            break;
            //        case "3":
            //            putOptionsNumStories.Add("THREE");
            //            break;
            //        case "M":
            //            putOptionsNumStories.Add("SPLIT");
            //            break;
            //    }
            //}
            // Market options that doesn't match
            //'OTHSE','Other-See Remarks',
            //'NONE','None'

            //if (putOptionsNumStories.Count > 0)
            //{
            //    driver.SetMultipleCheckboxById("Input_161", string.Join(",", putOptionsNumStories.ToArray()));
            //}

            //driver.SetMultipleCheckboxById("Input_164", listing.KitchenDesc); // Kitchen Features
            HashSet<String> putOptionsKitchenDesc = new HashSet<String>();
            List<String> optionsKitchenDesc = String.IsNullOrWhiteSpace(listing.KitchenDesc) ? new List<String>() : listing.KitchenDesc.Split(',').ToList();

            List<String> optionsOtherRoomDesc = String.IsNullOrWhiteSpace(listing.OtherRoomDesc) ? new List<String>() : listing.OtherRoomDesc.Split(',').ToList();
            if (optionsOtherRoomDesc.Count > 0)
                optionsKitchenDesc.AddRange(optionsOtherRoomDesc);

            List<String> optionsAppliancesDesc2 = String.IsNullOrWhiteSpace(listing.AppliancesDesc) ? new List<String>() : listing.AppliancesDesc.Split(',').ToList();
            if (optionsAppliancesDesc2.Count > 0)
                optionsKitchenDesc.AddRange(optionsAppliancesDesc2);

            foreach (string option in optionsKitchenDesc)
            {
                switch (option)
                {
                    case "2KTCH":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "BRAREA":
                        putOptionsKitchenDesc.Add("BKFAR");
                        break;
                    case "BRFAR":
                        putOptionsKitchenDesc.Add("BKFBA");
                        break;
                    case "CNISL":
                        putOptionsKitchenDesc.Add("Center_Island");
                        break;
                    case "CORTP":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "GLYTP":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "KTNET":
                        putOptionsKitchenDesc.Add("Kitchenette");
                        break;
                    case "NSTCN":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "OFMRM":
                        putOptionsKitchenDesc.Add("Open_to_Fam_Rm");
                        break;
                    case "SLSTNC":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "TILEC":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "WPANT":
                        putOptionsKitchenDesc.Add("Walk_in_Pantry");
                        break;
                    case "PANTRY":
                    case "RMPNT":
                        putOptionsKitchenDesc.Add("PANTR");
                        break;
                    case "MCWOV":
                        putOptionsKitchenDesc.Add("BLINM");
                        break;
                    case "CKTPG":
                        putOptionsKitchenDesc.Add("COOKT");
                        break;
                    case "DSWSR":
                        putOptionsKitchenDesc.Add("DISHW");
                        break;
                    case "DSPSL":
                        putOptionsKitchenDesc.Add("DISPO");
                        break;
                    case "GMCNT":
                        putOptionsKitchenDesc.Add("GRANITECT");
                        break;
                    case "RFGTR":
                        putOptionsKitchenDesc.Add("REFRI");
                        break;
                    case "":
                        putOptionsKitchenDesc.Add("");
                        break;
                }
            }
            List<String> optionsAppliancesDesc = String.IsNullOrWhiteSpace(listing.AppliancesDesc) ? new List<String>() : listing.AppliancesDesc.Split(',').ToList();
            foreach (string option in optionsAppliancesDesc)
            {
                switch (option)
                {
                    case "AENON":
                        putOptionsKitchenDesc.Add("NONE");
                        break;
                    case "BICMK":
                       // putOptionsKitchenDesc.Add("");
                        break;
                    case "BLTOV":
                        putOptionsKitchenDesc.Add("BLINO");
                        break;
                    case "CKTPE":
                        putOptionsKitchenDesc.Add("ELECT");
                        putOptionsKitchenDesc.Add("COOKT"); 
                        break;
                    case "CKTPG":
                        putOptionsKitchenDesc.Add("GAS");
                        putOptionsKitchenDesc.Add("COOKT");
                        break;
                    case "CNVCM":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "COFF":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "CONVE":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "DBLOV":
                        putOptionsKitchenDesc.Add("DOUBL");
                        break;
                    case "DRYER":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "DSPSL":
                        putOptionsKitchenDesc.Add("DISPO");
                        break;
                    case "DSWSR":
                        putOptionsKitchenDesc.Add("DISHW");
                        break;
                    case "DWNDF":
                        putOptionsKitchenDesc.Add("DOWND");
                        break;
                    case "ELCWT":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "ENGAP":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "EXFVT":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "GASWH":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "GEOTH":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "HASYS":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "IHWTR":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "MCWOV":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "RCXHF":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "RECWT":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "RFGTR":
                        putOptionsKitchenDesc.Add("REFRI");
                        break;
                    case "RFSZT":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "RNGFS":
                        putOptionsKitchenDesc.Add("RANGE");
                        break;
                    case "SCLOV":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "SNGOV":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "SOLWH":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "STKDW":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "STVCO":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "TNKWH":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "TSCMP":
                        putOptionsKitchenDesc.Add("TRASH");
                        break;
                    case "WINRE":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "WSHR":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "WTFLL":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "WTFLO":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "WTSFL":
                        //putOptionsKitchenDesc.Add("");
                        break;
                    case "WTSFO":
                        //putOptionsKitchenDesc.Add("");
                        break;
                }
            }

            // Market options that doesn't match
            //'BLINM','Built-In Microwave',
            //'CUSTO','Custom Cabinets',
            //'ICEM','Ice Maker',
            //'ICEMC','Ice Maker Connection',
            //'ISLAN','Island',
            //'OTHSE','Other-See Remarks',
            //'CNTOP','Solid Counter Tops',
            //'VENTH','Vent Hood',
            //'Eat_in_Kitchen','Eat in Kitchen',

            if (optionsKitchenDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_566", string.Join(",", putOptionsKitchenDesc.ToArray()));
            }

            //driver.SetMultipleCheckboxById("Input_165", listing.LaundryFacilityDesc); // Laundry
            List<String> putOptionsLaundryFacilityDesc = new List<String>();
            List<String> optionsLaundryFacilityDesc = String.IsNullOrWhiteSpace(listing.LaundryFacilityDesc) ? new List<String>() : listing.LaundryFacilityDesc.Split(',').ToList();
            foreach (string option in optionsLaundryFacilityDesc)
            {
                switch (option)
                {
                    case "BATH":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "CHTE":
                        putOptionsLaundryFacilityDesc.Add("Chute");
                        break;
                    case "COFF":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "CRPT":
                        putOptionsLaundryFacilityDesc.Add("Carport");
                        break;
                    case "CSTL":
                        putOptionsLaundryFacilityDesc.Add("CLOSE");
                        break;
                    case "ELCCN":
                        putOptionsLaundryFacilityDesc.Add("DRYER");
                        putOptionsLaundryFacilityDesc.Add("ELECT");
                        break;
                    case "GASCN":
                        putOptionsLaundryFacilityDesc.Add("DRYER");
                        putOptionsLaundryFacilityDesc.Add("GAS");
                        break;
                    case "GRGE":
                        putOptionsLaundryFacilityDesc.Add("INGAR");
                        break;
                    case "HALL":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "INUNT":
                        putOptionsLaundryFacilityDesc.Add("INEAC");
                        break;
                    case "KTCH":
                        putOptionsLaundryFacilityDesc.Add("INKIT");
                        break;
                    case "LFAGN":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "LFCOM":
                        putOptionsLaundryFacilityDesc.Add("COMMO");
                        break;
                    case "LFNON":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "LFSEP":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "LUPL":
                        putOptionsLaundryFacilityDesc.Add("UPPER");
                        break;
                    case "LURM":
                        putOptionsLaundryFacilityDesc.Add("Utility_Laundry");
                        break;
                    case "MNLV":
                        putOptionsLaundryFacilityDesc.Add("MAINL");
                        break;
                    case "MULMA":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "NOCNS":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "SINK":
                        putOptionsLaundryFacilityDesc.Add("Sink");
                        break;
                    case "STCON":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "UTRM":
                        //putOptionsLaundryFacilityDesc.Add("");
                        break;
                    case "WSHCN":
                        putOptionsLaundryFacilityDesc.Add("WASHE");
                        break;
                }
            }
            // Market options that doesn't match
            //'BASEM','Basement',
            //'INSID','Inside',
            //'LAUND','Laundry Room',
            //'LOWER','Lower Level',
            //'NONE','None',
            //'OTHSE','Other-See Remarks',

            if (putOptionsLaundryFacilityDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_567", string.Join(",", putOptionsLaundryFacilityDesc.ToArray()));
            }

            //driver.SetMultipleCheckboxById("Input_166", listing.InteriorDesc); // Interior Features
            List<String> putOptionsInteriorDesc = new List<String>();
            List<String> optionsInteriorDesc = String.IsNullOrWhiteSpace(listing.InteriorDesc) ? new List<String>() : listing.InteriorDesc.Split(',').ToList();

            List<String> optionsOtherRoomDesc2 = String.IsNullOrWhiteSpace(listing.OtherRoomDesc) ? new List<String>() : listing.OtherRoomDesc.Split(',').ToList();
            if (optionsOtherRoomDesc2.Count > 0)
                optionsInteriorDesc.AddRange(optionsOtherRoomDesc2);

            List<String> optionsBed1Desc = String.IsNullOrWhiteSpace(listing.Bed1Desc) ? new List<String>() : listing.Bed1Desc.Split(',').ToList();
            if (optionsBed1Desc.Count > 0)
                optionsInteriorDesc.AddRange(optionsBed1Desc);

            foreach (string option in optionsInteriorDesc)
            {
                switch (option)
                {
                    case "BBKCA":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "BENTC":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "BPANT":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "BSAFE":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "CDCLS":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "CLBM":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "CLCFF":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "CLCTH":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "CLHIH":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "CLVLT":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "CRMLD":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "DBMWT":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "ELVTR":
                        putOptionsInteriorDesc.Add("ELEVA");
                        break;
                    case "FRAST":
                        putOptionsInteriorDesc.Add("CARBO");
                        break;
                    case "FRNDR":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "IILPL":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "INTCM":
                        putOptionsInteriorDesc.Add("INTER");
                        break;
                    case "INUTL":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "IWICL":
                        putOptionsInteriorDesc.Add("WALKI");
                        break;
                    case "LNVOC":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "LRECE":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "MURBD":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsInteriorDesc.Add("NONE");
                        break;
                    case "POCKD":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "PSHUT":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "SCSYL":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "SCSYO":
                        putOptionsInteriorDesc.Add("SECSY");
                        break;
                    case "SHTRS":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "SKYLG":
                        putOptionsInteriorDesc.Add("SKYLI");
                        break;
                    case "SMKDT":
                        putOptionsInteriorDesc.Add("SMOKE");
                        break;
                    case "TKLGH":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "WETBR":
                        putOptionsInteriorDesc.Add("WETBA");
                        break;
                    case "WNTRM":
                        putOptionsInteriorDesc.Add("WINDO");
                        break;
                    case "WRSCT":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "WRSSD":
                        //putOptionsInteriorDesc.Add("");
                        break;
                    case "WRSTR":
                        putOptionsInteriorDesc.Add("WSPEAK");
                        break;
                    case "RMGAM":
                        putOptionsInteriorDesc.Add("GAMER");
                        break;
                    case "RMOFS":
                        putOptionsInteriorDesc.Add("OFFIC");
                        break;
                    case "FRGDN":
                        putOptionsInteriorDesc.Add("GARDE");
                        break;
                    case "RSSWR":
                        putOptionsInteriorDesc.Add("SEPAR");
                        break;

                }
            }
            // Market options that doesn't match
            //'ATTIC','Attic Fan',
            //'CEILI','Ceiling Fans ',
            //'CNTRD','Central Distribution Plumbing System',
            //'CNTRV','Central Vacuum',
            //'FINIS','Finished Basment',
            //'GAMER','Game Room',
            //'GARDE','Garden Tub',
            //'HANDI','Handicap Design',
            //'HIGHC','High Ceilings',
            //'INLAW','In-Law Suites',
            //'MSTRD','Master Down',
            //'MSTRU','Master Up',
            //'MLTDI','Multiple Dining Areas',
            //'MLTLI','Multiple Living Areas',
            //'OFFIC','Office',
            //'OTHSE','Other-See Remarks',
            //'SAUNA','Sauna',
            //'SHOWO','Shower Only',
            //'SHOWT','Shower\/Tub Combo',
            //'SPLIT','Split Bedroom',
            //'UNFIN','Unfinished Basement',
            //'WTRSO','Water Softener-Lease',
            //'WASOO','Water Softner-Owned',
            //'WHEEL','Wheelchair Access',
            //'WHIRL','Whirlpool',
            if (putOptionsInteriorDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_571", string.Join(",", putOptionsInteriorDesc.ToArray()));
            }

            //driver.SetMultipleCheckboxById("Input_167", listing.GarageDesc); // Garage/Carport
            List<String> putOptionsGarageDesc = new List<String>();
            List<String> optionsGarageDesc = String.IsNullOrWhiteSpace(listing.GarageDesc) ? new List<String>() : listing.GarageDesc.Split(',').ToList();
            foreach (string option in optionsGarageDesc)
            {
                switch (option)
                {
                    case "GATCH":
                        putOptionsGarageDesc.Add("ATTAC");
                        break;
                    case "GDMLP":
                        //putOptionsGarageDesc.Add("");
                        break;
                    case "GDROP":
                        putOptionsGarageDesc.Add("GARAG");
                        break;
                    case "GDRSG":
                        //putOptionsGarageDesc.Add("");
                        break;
                    case "GDTCH":
                        putOptionsGarageDesc.Add("DETGA");
                        break;
                    case "GENFR":
                        //putOptionsGarageDesc.Add("");
                        break;
                    case "GENRR":
                        putOptionsGarageDesc.Add("REENG");
                        break;
                    case "GENRV":
                        //putOptionsGarageDesc.Add("");
                        break;
                    case "GENSD":
                        putOptionsGarageDesc.Add("SIENG");
                        break;
                    case "GENSI":
                        //putOptionsGarageDesc.Add("");
                        break;
                    case "GOLFC":
                        //putOptionsGarageDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsGarageDesc.Add("NONEG");
                        break;
                    case "PARGA":
                        //putOptionsGarageDesc.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'1CARC','1 Car Carport',
            //'1CARG','1 Car Garage',
            //'2CARC','2 Car Carport',
            //'2CARG','2 Car Garage',
            //'3PLCA','3+ Car Carport',
            //'3PLGA','3+ Car Garage',
            //'CIRCU','Circular Drive',
            //'CONVE','Converted to Living Space',
            //'DETCA','Detached Carport',
            //'NONEC','None-Carport',
            //'OTHCA','Other Carport-See Remarks',
            //'OTHGA','Other Garage-See Remarks',
            //'OVRCA','Oversized Carport',
            //'OVRGA','Oversized Garage',
            //'REENC','Rear Entry Carport',
            //'RVPAR','RV Parking Available',
            //'SIENC','Side Entry Carport',

            if (putOptionsGarageDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_570", string.Join(",", putOptionsGarageDesc.ToArray()));
            }

            driver.SetSelect(By.Id("Input_657"), listing.CTXGuestHouse); // Guest House
        }

        /// <summary>
        /// Fills the information for the [Lot/Environment/Utility Information] tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillLotEnvironmentUtilityInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript(" jQuery(document).scrollTop(0);");

            //driver.ExecuteScript(" __doPostBack('m_rpPageList$ctl08$lbPageLink', '');");
        
            driver.Click(By.LinkText("Lot/Environment/Utility")); // Lot/Environment/Utility Information
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_613"))); // the last field on the form
            
            #region Lot Information
            driver.WriteTextbox(By.Id("Input_576"), listing.LotSize); // Lot Dimensions
            driver.WriteTextbox(By.Id("Input_577"), listing.Acres); // Apx Acreage
            driver.SetSelect(By.Id("Input_578"), "0"); // Manufactured Allowed (default hardcode "No")
            driver.SetMultipleCheckboxById("Input_596", listing.ExteriorDesc); // Exterior Features
            //driver.SetMultipleCheckboxById("Input_172", listing.FenceDesc); // Fencing
            List<String> putOptionsFenceDesc = new List<String>();
            List<String> optionsFenceDesc = String.IsNullOrWhiteSpace(listing.FenceDesc) ? new List<String>() : listing.FenceDesc.Split(',').ToList();
            foreach (string option in optionsFenceDesc)
            {
                switch (option)
                {
                    case "BARBED":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "CEDAR":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "CHAINLNK":
                        putOptionsFenceDesc.Add("CHAIN");
                        break;
                    case "ELECT":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "GAME":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "GOAT":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "INVIS":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "LIVESTK":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "MASONRY":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "MESH":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsFenceDesc.Add("NONE");
                        break;
                    case "NONPRVCY":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "PARTIAL":
                        putOptionsFenceDesc.Add("PARTI");
                        break;
                    case "PICKET":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "PIPE":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "POST":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "PRIVACY":
                        putOptionsFenceDesc.Add("PRIVA");
                        break;
                    case "SPLIT":
                        putOptionsFenceDesc.Add("BARBW");
                        break;
                    case "VINYL":
                        //putOptionsFenceDesc.Add("");
                        break;
                    case "WIRE":
                        putOptionsFenceDesc.Add("Wire");
                        break;
                    case "WIRON":
                        putOptionsFenceDesc.Add("WROUG");
                        break;
                    case "WOOD":
                        putOptionsFenceDesc.Add("WOODF");
                        break;
                }
            }
            // Market options that doesn't match
            //'CROSS','Cross Fence',
            //'HIGHF','High Fence',
            //'OTHSE','Other-See Remarks',
            //'RANCH','Ranch Fence',

            if (putOptionsFenceDesc.Count > 0)
            {
                //driver.SetMultipleCheckboxById("Input_172", string.Join(",", putOptionsFenceDesc.ToArray()));
            }
            driver.SetSelect(By.Id("Input_173"), listing.IsWaterFront); // Waterfront
            //driver.SetMultipleCheckboxById("Input_174", listing.WaterfrontDesc); // Water Features
            List<String> putOptionsWaterfrontDesc = new List<String>();
            List<String> optionsWaterfrontDesc = String.IsNullOrWhiteSpace(listing.WaterfrontDesc) ? new List<String>() : listing.WaterfrontDesc.Split(',').ToList();

            List<String> optionsViewDesc = String.IsNullOrWhiteSpace(listing.ViewDesc) ? new List<String>() : listing.ViewDesc.Split(',').ToList();

            if (optionsViewDesc != null & optionsViewDesc.Count > 0)
                optionsWaterfrontDesc.AddRange(optionsViewDesc);

            foreach (string option in optionsWaterfrontDesc)
            {
                switch (option)
                {
                    case "CANALFRT":
                        putOptionsWaterfrontDesc.Add("CANAL");
                        break;
                    case "CREEKFRT":
                        putOptionsWaterfrontDesc.Add("CREK");
                        break;
                    case "LAKEFRT":
                        putOptionsWaterfrontDesc.Add("LAKE");
                        break;
                    case "PONDFRT":
                        putOptionsWaterfrontDesc.Add("PONDS");
                        break;
                    case "RIVERFRT":
                        putOptionsWaterfrontDesc.Add("RIVER");
                        break;
                    case "HILCNTRY":
                        putOptionsWaterfrontDesc.Add("HILLC");
                        break;
                    case "NONE":
                        putOptionsWaterfrontDesc.Add("NONE");
                        break;
                }
            }
            // Market options that doesn't match
            //'BLTLK','Belton Lake',
            //'BLANC','Blanco River',
            //'BRAZ','Brazos River',
            //'CANYO','Canyon Lake\/U.S. Corp of Engineers',
            //'CITYS','City Skyline View',
            //'COMAL','Comal River',
            //'COLOR','Colorado River',
            //'COUNT','Countryside View',
            //'CREKS','Creek-Seasonal',
            //'GUADA','Guadalupe River',
            //'HILLC','Hill Country View',
            //'LKAUS','Lake Austin',
            //'BAST','Lake Bastrop',
            //'BUCKN','Lake Buchanan',
            //'LAKED','Lake Dunlap',
            //'LAKEL','Lake LBJ',
            //'LAKEM','Lake McQueeney',
            //'LAKEP','Lake Placid',
            //'LAKES','Lake Seguin',
            //'TRAV','Lake Travis',
            //'MEADO','Meadow Lake',
            //'NONE','None',
            //'OTHSE','Other-See Remarks',
            //'PEDER','Pedernales River',
            //'SANMA','San Marcos River',
            //'STHLK','Stillhouse Hollow Lake',
            //'WTRAC','Water Access',
            //'WTRVI','Water View',
            //'WTRFR','Waterfront'

            if (putOptionsWaterfrontDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_585", string.Join(",", putOptionsWaterfrontDesc.ToArray())); // Water Features
            }
            //driver.SetMultipleCheckboxById("Input_175", listing.LotDesc); // Topo/Land Description
            List<String> putOptionsLotDesc = new List<String>();
            List<String> optionsLotDesc = String.IsNullOrWhiteSpace(listing.LotDesc) ? new List<String>() : listing.LotDesc.Split(',').ToList();
            foreach (string option in optionsLotDesc)
            {
                switch (option)
                {
                    case "ALLEYACC":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "BACKGOLF":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "BACKPARK":
                        putOptionsLotDesc.Add("GREEN");
                        break;
                    case "CANAL":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "CORNER":
                        putOptionsLotDesc.Add("CORNE");
                        break;
                    case "CULDESAC":
                        putOptionsLotDesc.Add("CULDE");
                        break;
                    case "CULTVTD":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "CURBS":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "FLAGLOT":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "GOLFCOMM":
                        putOptionsLotDesc.Add("ONGOL");
                        break;
                    case "INTERIOR":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "IRREG":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "LAKE":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "LAKEFRT":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "LEVEL":
                        putOptionsLotDesc.Add("LEV");
                        break;
                    case "NOBKYDGR":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "OPEN":
                        putOptionsLotDesc.Add("OPEN");
                        break;
                    case "POND":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "PRIVROAD":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "PUBLROAD":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "ROLLING":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "SLOPE":
                        putOptionsLotDesc.Add("SLOPI");
                        break;
                    case "STREAM":
                        //putOptionsLotDesc.Add("");
                        break;
                    case "WOODED":
                        putOptionsLotDesc.Add("WOODE");
                        break;
                    case "XERISCAP":
                        //putOptionsLotDesc.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'AGEXE','Ag Exempt',
            //'BORDE','Borders State Park\/Game Ranch',
            //'FLODP','Flood Plain',
            //'GENTL','Gentle Rolling',
            //'HORSE','Horse Property',
            //'HUNTI','Hunting Permitted',
            //'MATUR','Mature Trees',
            //'MOBIL','Mobile Home Park',
            //'NONE','None',
            //'OTHSE','Other-See Remarks',
            //'PRTWO','Partially Wooded',
            //'RANCH','Ranch',
            //'SECLU','Secluded',

            if (putOptionsLotDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_597", string.Join(",", putOptionsLotDesc.ToArray())); // Topo/Land Description
            }
            //driver.SetMultipleCheckboxById("Input_176", listing.CommonFeatures); // Neighborhood Amenities
            List<String> putOptionsCommonFeatures = new List<String>();
            List<String> optionsLotCommonFeatures = String.IsNullOrWhiteSpace(listing.CommonFeatures) ? new List<String>() : listing.CommonFeatures.Split(',').ToList();
            foreach (string option in optionsLotCommonFeatures)
            {
                switch (option)
                {
                    case "CLSTRMBX":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "CLUBHSE":
                        putOptionsCommonFeatures.Add("CLUBH");
                        break;
                    case "COMMONS":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "DOGPRK":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "ELEVATOR":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "EQUEST":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "FITCTR":
                        putOptionsCommonFeatures.Add("FITCNT");
                        break;
                    case "GAMERM":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "GOLFPRIV":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "GOLFPUB":
                        putOptionsCommonFeatures.Add("GOLFC");
                        break;
                    case "GRILL":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "HTTUBCOM":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "JOGBIKE":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "KITCHFAC":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "LAKEPRIV":
                        putOptionsCommonFeatures.Add("LAKER");
                        break;
                    case "PARK":
                        putOptionsCommonFeatures.Add("PARKA");
                        break;
                    case "PETAM":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "PLAYGRND":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "POOLCOMM":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "SAUNA":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "SMAIRPRT":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "SPRTCTS":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "SPRTFAC":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "STORAGE":
                        //putOptionsCommonFeatures.Add("");
                        break;
                    case "TENNISCT":
                        putOptionsCommonFeatures.Add("TENNI");
                        break;
                    case "UNGRUTIL":
                        //putOptionsCommonFeatures.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'BASKE','Basketball Court',
            //'BBQGR','BBQ\/Grill',
            //'BIKET','Bike Trails',
            //'BOATD','Boat Dock',
            //'BOATR','Boat Ramp',
            //'BRIDL','Bridle Path',
            //'CONTR','Controlled Access',
            //'FISHI','Fishing Pier',
            //'NONE','None',
            //'OTHSE','Other-See Remarks',
            //'SWIMM','Swimming Pool',
            //'VOLLE','Volleyball Court',
            //'WALKI','Walking\/Jogging Trail'

            if (putOptionsCommonFeatures.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_599", string.Join(",", putOptionsCommonFeatures.ToArray()));  // Neighborhood Amenities
            }

            //driver.SetMultipleCheckboxById("Input_177", listing.LotDesc); // Access/Road Surface
            List<String> putOptionsLotDescForAccessRoadSurface = new List<String>();
            foreach (string option in optionsLotDesc)
            {
                switch (option)
                {
                    case "ALLEYACC":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "BACKGOLF":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "CANAL":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "CULTVTD":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "CURBS":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "FLAGLOT":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "INTERIOR":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "IRREG":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "LAKE":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "LAKEFRT":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "NOBKYDGR":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "POND":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "PRIVROAD":
                        putOptionsLotDescForAccessRoadSurface.Add("PRIVA");
                        break;
                    case "PUBLROAD":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "ROLLING":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "STREAM":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                    case "XERISCAP":
                        //putOptionsLotDescForAccessRoadSurface.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'ASPHA','Asphalt',
            //'CALIC','Caliche',
            //'CITYS','City Street',
            //'CONCR','Concrete',
            //'COUNT','County Road',
            //'CURBS','Curbs',
            //'DIRT','Dirt',
            //'EASEM','Easement Road',
            //'GRAVE','Gravel',
            //'INTER','Interstate Hwy-1 mi or Less',
            //'NONE','None',
            //'OTHSE','Other-See Remarks',
            //'PAVED','Paved',
            //'SEALE','Sealed & Chipped',
            //'SIDEW','Sidewalks',
            //'STATE','State Highway',
            //'USHIG','US Highway'

            //if (putOptionsLotDescForAccessRoadSurface.Count > 0)
            //{
            //    driver.SetMultipleCheckboxById("Input_177", string.Join(",", putOptionsLotDescForAccessRoadSurface.ToArray())); // Access/Road Surface
            //}
            driver.SetMultipleCheckboxById("Input_600", "CITYS"); // Access/Road Surface
            #endregion Lot Information

            #region Utility Information
            //driver.SetMultipleCheckboxById("Input_178", listing.HeatSystemDesc); // Heat
            List<String> putOptionsHeat = new List<String>();
            List<String> optionsLotHeat = String.IsNullOrWhiteSpace(listing.HeatSystemDesc) ? new List<String>() : listing.HeatSystemDesc.Split(',').ToList();
            foreach (string option in optionsLotHeat)
            {
                switch (option)
                {
                    case "BASEBRD":
                        putOptionsHeat.Add("BASEB");
                        break;
                    case "BOILER":
                        //putOptionsHeat.Add("");
                        break;
                    case "CENTRAL":
                        putOptionsHeat.Add("CENTR");
                        break;
                    case "ELECTRIC":
                        putOptionsHeat.Add("ELECT");
                        break;
                    case "FLRFURN":
                        putOptionsHeat.Add("FLOOR");
                        break;
                    case "GAS":
                        putOptionsHeat.Add("GAS");
                        break;
                    case "GEOTH":
                        //putOptionsHeat.Add("");
                        break;
                    case "HEATPUMP":
                        putOptionsHeat.Add("HEATP");
                        break;
                    case "NONE":
                        putOptionsHeat.Add("NONE");
                        break;
                    case "RADIANT":
                        //putOptionsHeat.Add("");
                        break;
                    case "RADIATOR":
                        //putOptionsHeat.Add("");
                        break;
                    case "SOLAR":
                        //putOptionsHeat.Add("");
                        break;
                    case "SPACEHTR":
                        putOptionsHeat.Add("SPACE");
                        break;
                    case "WALLUNIT":
                        putOptionsHeat.Add("WINDO");
                        break;
                }
            }
            // Market options that doesn't match
            //'1UNIT','1 Unit',
            //'2UNIT','2 Units',
            //'3PLUN','3+ Units',
            //'Fireplace','Fireplace',
            //'GASJE','Gas Jets',
            //'OTHSE','Other-See Remarks',
            //'PROPA','Propane\/Butane',
            //'WOODS','Wood Stove',
            //'ZONED','Zoned'

            if (putOptionsHeat.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_610", string.Join(",", putOptionsHeat.ToArray())); // Heat
            }

            //driver.SetMultipleCheckboxById("Input_179", listing.CoolSystemDesc); // A/C 
            List<String> putOptionsCoolSystemDesc = new List<String>();
            List<String> optionsLotCoolSystemDesc = String.IsNullOrWhiteSpace(listing.CoolSystemDesc) ? new List<String>() : listing.CoolSystemDesc.Split(',').ToList();
            foreach (string option in optionsLotCoolSystemDesc)
            {
                switch (option)
                {
                    case "CENTD":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "CENTRAL":
                        putOptionsCoolSystemDesc.Add("CENTR");
                        break;
                    case "CHLLDWTR":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "FAV":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "GEOTH":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "HEPA":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "MSSY":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsCoolSystemDesc.Add("NONE");
                        break;
                    case "S1315":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "S16PL":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "WALLUNIT":
                        //putOptionsCoolSystemDesc.Add("");
                        break;
                    case "WNDWUNIT":
                        putOptionsCoolSystemDesc.Add("WINDO");
                        break;
                }
            }
            // Market options that doesn't match
            //'1UNIT','1 Unit',
            //'2UNIT','2 Units',
            //'3PLUN','3+ Units',
            //'ATTIC','Attic Fan',
            //'ELECT','Electric',
            //'HEATP','Heat Pump',
            //'OTHSE','Other-See Remarks',
            //'ZONED','Zoned'

            if (putOptionsCoolSystemDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_611", string.Join(",", putOptionsCoolSystemDesc.ToArray())); // A/C 
            }

            //driver.SetMultipleCheckboxById("Input_180", listing.SewerDesc); // Water/Sewer
            List<String> putOptionsSewerDesc = new List<String>();
            List<String> optionsSewerDesc = String.IsNullOrWhiteSpace(listing.SewerDesc) ? new List<String>() : listing.SewerDesc.Split(',').ToList();
            foreach (string option in optionsSewerDesc)
            {
                switch (option)
                {
                    case "CITY":
                        putOptionsSewerDesc.Add("CITYW");
                        break;
                    case "CITYPRO":
                        //putOptionsSewerDesc.Add("");
                        break;
                    case "MUD":
                        putOptionsSewerDesc.Add("MUD");
                        break;
                    case "NONE":
                        putOptionsSewerDesc.Add("NONES");
                        putOptionsSewerDesc.Add("NONEW");
                        break;
                    case "PRIVATE":
                        putOptionsSewerDesc.Add("PRVTW");
                        break;
                    case "SEPTICO":
                        putOptionsSewerDesc.Add("SPTC");
                        break;
                    case "SPTND":
                        putOptionsSewerDesc.Add("SPTCR");
                        break;
                    case "SPTSHRD":
                        //putOptionsSewerDesc.Add("");
                        break;
                    case "WTRDIST":
                        //putOptionsSewerDesc.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'AEROB','Aerobic Septic',
            //'COOPW','Co-Op Water',
            //'ONSSE','On-Site Sewer',
            //'ONSWT','On-Site Water',
            //'OTHSE','Other Sewer-See Remarks',
            //'OTHWT','Other Water-See Remarks',
            //'PUBLI','Public Sewer',
            //'SEPRS','Separate Sewer Meters',
            //'SEPRW','Separate Water Meters',
            //'WELL','Well',
            //'WELLR','Well Required'

            if (putOptionsSewerDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_612", string.Join(",", putOptionsSewerDesc.ToArray())); // Water/Sewer
            }

            if(!String.IsNullOrWhiteSpace(listing.CTXRWaterHeater))
                driver.SetMultipleCheckboxById("Input_590", listing.CTXRWaterHeater.Trim()); // Water/Heater
            //driver.SetMultipleCheckboxById("Input_182", listing.UtilitiesDesc); // Other Utilities
            List<String> putOptionsUtilitiesDesc = new List<String>();
            List<String> optionsUtilitiesDesc = String.IsNullOrWhiteSpace(listing.UtilitiesDesc) ? new List<String>() : listing.UtilitiesDesc.Split(',').ToList();
            foreach (string option in optionsUtilitiesDesc)
            {
                switch (option)
                {
                    case "ABOVGRND":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "ELEAVAIL":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "ELECTRIC":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "ELENOAVL":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "FUELTANK":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "GAS":
                        putOptionsUtilitiesDesc.Add("GASIN");
                        putOptionsUtilitiesDesc.Add("NATUR");
                        break;
                    case "GASAVAIL":
                        putOptionsUtilitiesDesc.Add("GASIN");
                        putOptionsUtilitiesDesc.Add("GASAV"); 
                        break;
                    case "GASNOAVL":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "NONE":
                        putOptionsUtilitiesDesc.Add("NONE");
                        break;
                    case "PHONAVAL":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PHONNOT":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PHONPROP":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PROPANE":
                        putOptionsUtilitiesDesc.Add("PBTOW");
                        break;
                    case "PROPAVAL":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "PROPNEED":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "SOLAR":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "UNDRGRND":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                    case "WIND":
                        //putOptionsUtilitiesDesc.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'AVAIL','Available',
            //'CABLE','Cable TV',
            //'CITYE','City Electric',
            //'CITYG','City Garbage',
            //'COOPE','Co-Op Electric',
            //'HIGHS','High Speed Internet ',
            //'ONSEL','On-Site Electric',
            //'OTHSE','Other-See Remarks',
            //'PRIVA','Private Garbage Service',
            //'PBTLE','Propane\/Butane Tank-Leased',
            //'SATEL','Satellite Dish',
            //'SEPER','Separate Meters',
            //'TELEP','Telephone'
            if (putOptionsUtilitiesDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_613", string.Join(",", putOptionsUtilitiesDesc.ToArray())); // Other Utilities
            }
            #endregion Utility Information

            List<String> putOptionsRestrictionsDesc = new List<String>();
            List<String> optionsRestrictionsDesc = String.IsNullOrWhiteSpace(listing.RestrictionsDesc) ? new List<String>() : listing.RestrictionsDesc.Split(',').ToList();
            foreach (string option in optionsRestrictionsDesc)
            {
                switch (option)
                {
                    case "ADLT55":
                        putOptionsRestrictionsDesc.Add("ADULT55");
                        break;
                    case "ADLT62":
                        //putOptionsRestrictionsDesc.Add("");
                        break;
                    case "BLDGST":
                        putOptionsRestrictionsDesc.Add("BLDGSTYLE");
                        break;
                    case "BLDGSZ":
                        putOptionsRestrictionsDesc.Add("BLDGSIZE");
                        break;
                    case "CITRS":
                        //putOptionsRestrictionsDesc.Add("CITYRESTRIC");
                        break;
                    case "CVNANT":
                        //putOptionsRestrictionsDesc.Add("CONVENANT");
                        break;
                    case "DEEDRS":
                        putOptionsRestrictionsDesc.Add("DEEDR");
                        break;
                    case "DVLMPT":
                        //putOptionsRestrictionsDesc.Add("");
                        break;
                    case "ENVRO":
                        //putOptionsRestrictionsDesc.Add("ENVIRONMTL");
                        break;
                    case "ESMNT":
                        //putOptionsRestrictionsDesc.Add("EASEMENT");
                        break;
                    case "LEASE":
                        //putOptionsRestrictionsDesc.Add("");
                        break;
                    case "LMTDVH":
                        //putOptionsRestrictionsDesc.Add("");
                        break;
                    case "LVSTCK":
                        //putOptionsRestrictionsDesc.Add("LIVESTOCK");
                        break;
                    case "RSLIM":
                        //putOptionsRestrictionsDesc.Add("");
                        break;
                    case "RUNKN":
                        //putOptionsRestrictionsDesc.Add("");
                        break;
                    case "ZONE":
                        //putOptionsRestrictionsDesc.Add("");
                        break;
                }
            }
            // Market options that doesn't match
            //'AERIA','Aerial Photos',
            //'APPRA','Appraisal',
            //'BOUND','Boundary Survey',
            //'ENGIN','Engineer\'s Report',
            //'FIELD','Field Notes',
            //'FLODC','Flood Certification',
            //'FLOOR','Floor Plans',
            //'INHOM','Inspection-Home',
            //'INPES','Inspection-Pest',
            //'INSPT','Inspection-Septic',
            //'LEADB','Lead Based Paint Addendum',
            //'NONE','None',
            //'ONSSE','On-Site Sewer Disclosure',
            //'OTHSE','Other-See Remarks',
            //'PLAT','Plat',
            //'SELLE','Seller\'s Disclosure',
            //'SPECI','Special Contract\/Addendum Required',
            //'SBDRE','Subdivision Restrictions',
            //'SURVE','Survey',
            //'TOPOM','Topo Map'

            if (putOptionsRestrictionsDesc.Count > 0)
            {
                driver.SetMultipleCheckboxById("Input_595", string.Join(",", putOptionsRestrictionsDesc.ToArray())); // Restrictions Type
            }
        }

        /// <summary>
        /// Fills the information for the [Financial Information] tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillFinancialInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript(" jQuery(document).scrollTop(0);");

            driver.Click(By.LinkText("Financial")); // Financial Information
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_630"))); // the last field on the form

            //driver.SetMultipleCheckboxById("Input_149", "EAEXC,NEWCO"); // Special

            driver.SetMultipleCheckboxById("Input_744", listing.HowToSellDesc); // Acceptable Financing
            
            driver.SetMultipleCheckboxById("Input_616", listing.ExemptionsDesc); // Exemptions

            driver.WriteTextbox(By.Id("Input_618"), listing.TaxYear); //Tax Year
            driver.WriteTextbox(By.Id("Input_619"), listing.TaxRate); //Tax Rate

            // HOA
            if (!String.IsNullOrEmpty(listing.HOA) && (listing.HOA == "M"))
                driver.SetSelect(By.Id("Input_622"), "MAN"); // HOA Mandatory
            else if (!String.IsNullOrEmpty(listing.HOA) && (listing.HOA == "V"))
                driver.SetSelect(By.Id("Input_622"), "VOL"); // HOA Voluntary
            else
                driver.SetSelect(By.Id("Input_622"), "NONE"); // HOA None

            driver.WriteTextbox(By.Id("Input_623"), listing.AssocName); // HOA Name
            driver.WriteTextbox(By.Id("Input_624"), listing.AssocFee); // HOA Amount
            driver.SetMultipleCheckboxById("Input_630", listing.AssocFeeIncludes); // Exemptions
            if (!String.IsNullOrEmpty(listing.AssocFeePaid))
            {
                switch (listing.AssocFeePaid)
                {
                    case "A":
                        driver.SetSelect(By.Id("Input_625"), "ANNL"); // HOA Term
                        break;
                    case "M":
                        driver.SetSelect(By.Id("Input_625"), "MNTH"); // HOA Term
                        break;
                    case "Q":
                        driver.SetSelect(By.Id("Input_625"), "QTR"); // HOA Term
                        break;
                    case "S":
                        driver.SetSelect(By.Id("Input_625"), "SMIAN"); // HOA Term
                        break;
                }
            }

        }

        /// <summary>
        /// Fills the information for the [Showing Information] tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillShowingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript(" jQuery(document).scrollTop(0);");

            driver.Click(By.LinkText("Brokerage/Showing Information")); // Financial Information
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_656"))); // the last field on the form

            #region Brokerage/Showing Information


            #endregion Brokerage/Showing Information

            #region Brokerage

            if (listing.CompBuy != null)
            {
                if (listing.CompBuy.EndsWith("%"))
                {
                    driver.SetSelect(By.Id("Input_632"), "%", true);
                }
                else if (listing.CompBuy.EndsWith("$"))
                {
                    driver.SetSelect(By.Id("Input_632"), "$", true);
                }
                driver.WriteTextbox(By.Id("Input_633"), listing.CompBuy.Replace("%", "").Replace("$", "")); // Buyer Agency Compensation
            }

            if (listing.AgentBonusAmount != null)
            {
                if (listing.AgentBonusAmount.EndsWith("%"))
                {
                    driver.SetSelect(By.Id("Input_636"), "%", true);
                }
                else if (listing.AgentBonusAmount.EndsWith("$"))
                {
                    driver.SetSelect(By.Id("Input_636"), "$", true);
                }
                driver.WriteTextbox(By.Id("Input_635"), listing.AgentBonusAmount.Replace("%", "").Replace("$", "")); // Sub Agency Compensation
            }
            driver.SetSelect(By.Id("Input_637"), "0"); // Prospects Exempt (default hardcode "No")
            driver.WriteTextbox(By.Id("Input_639"), listing.CTXEarnestMoney); // Earnest Money
            #endregion Brokerage

            #region Showing
            driver.SetSelect(By.Id("Input_642"), listing.LockboxTypeDesc); // Lockbox Type
            driver.WriteTextbox(By.Id("Input_648"), listing.OwnerPhone); // Showing Phone
            driver.WriteTextbox(By.Id("Input_649"), listing.OtherPhone); // Showing Phone # 2
            driver.SetMultipleCheckboxById("Input_645", listing.ShowingInstructions);  // Showing Instructions (Max 3)
            # endregion Showing

            #region Compensation/Showing Information

            //
            
            #endregion Compensation/Showing Information
        }

        /// <summary>
        /// Fills the information for the [Remarks] tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillRemarks(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.ExecuteScript(" jQuery(document).scrollTop(0);");

            driver.Click(By.LinkText("Remarks")); // Financial Information
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_142"))); // the last field on the form

            #region Documents & Internet Display
            //driver.SetSelect(By.Id("Input_143"), "1"); // IDX Opt In (default hardcode "Yes")
            //driver.SetSelect(By.Id("Input_144"), "1"); // Display on Internet (default hardcode "Yes")
            //driver.SetSelect(By.Id("Input_145"), "1"); // Display Address (default hardcode "Yes")
            //driver.SetSelect(By.Id("Input_146"), "1"); // Allow AVM (default hardcode "Yes")
           // driver.SetSelect(By.Id("Input_147"), "1"); // Allow Comment (default hardcode "Yes")
            //driver.SetMultipleCheckboxById("Input_219", listing.RestrictionsDesc); // 	Documents On File

            //driver.SetMultipleCheckboxById("Input_219", "NONE"); // Documents On File
            #endregion Documents & Internet Display

            #region Comments
            UpdatePublicRemarksInRemarksTab(driver, listing); // Public Remarks
            UpdatePrivateRemarksInRemarksTab(driver, listing); // Agent Remarks

            // Directions
            #endregion Comments
        }

        private void UpdateYearBuiltDescriptionInGeneralTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Id("Input_553")));
            driver.ScrollDown();
            driver.SetSelect(By.Id("Input_547"), listing.YearBuiltDesc); // Construction Status
            driver.WriteTextbox(By.Id("Input_553"), listing.YearBuilt); // Year Built
        }

        private void UpdatePublicRemarksInRemarksTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            BuiltStatus status = BuiltStatus.WithCompletion;

            switch (listing.YearBuiltDesc)
            {
                case "TB":
                    status = BuiltStatus.ToBeBuilt;
                    break;
                case "NWCONSTR":
                case "NW":
                    status = BuiltStatus.ReadyNow;
                    break;
                case "UC":
                    status = BuiltStatus.WithCompletion;
                    break;
            }

            // driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Id("Input_917")));
            Thread.Sleep(400);
            driver.WriteTextbox(By.Id("Input_140"), listing.GetPublicRemarks(status)); // Internet / Remarks / Desc. of Property
            
            driver.WriteTextbox(By.Id("Input_142"), listing.Directions); // 	Directions
        }

        private void UpdatePrivateRemarksInRemarksTab(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            String bonusMessage = "";
            if (listing.BonusCheckBox.Equals(true) && listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Bonus & Buyer Incentives; ask Builder for details. ";
            else if (listing.BonusCheckBox.Equals(true))
                bonusMessage = "Possible Bonus; ask Builder for details. ";
            else if (listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Buyer Incentives; ask Builder for details. ";
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

            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";

            driver.WriteTextbox(By.Id("Input_141"), bonusMessage + listing.GetPrivateRemarks() + realtorContactEmail); // Agent Remarks
        }

        /// <summary>
        /// This method makes set of values to the Longitude and Latitud fields 
        /// </summary>
        /// <param name="driver"></param>
        private void SetLongitudeAndLatitudeValues(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            if (String.IsNullOrEmpty(listing.MLSNum))
            {
                driver.WriteTextbox(By.Id("INPUT__93"), listing.Latitude); // Latitude
                driver.WriteTextbox(By.Id("INPUT__94"), listing.Longitude); // Longitude
            }
        }

        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);
            Login(driver, listing);

            #region navigateMenu
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("ctl03_m_divFooterContainer")));
            #endregion

            EditProperty(driver, listing);

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Residential Input Form")));
            driver.Click(By.LinkText("Residential Input Form"));

            driver.ScrollDown();

            driver.WriteTextbox(By.Id("Input_127"), listing.ListPrice); // List Price

            return UploadResult.Success;

        }

        private void unSelectAllOptions(CoreWebDriver driver, String elementName)
        {
            //try
            //{
            ((IJavaScriptExecutor)driver).ExecuteScript("var elements = document.getElementsByName('" + elementName + "')[0].options; for (var i = 0; i < elements.length; i++) { elements[i].selected = false; } ");
            //}
            //catch { }
        }

        private List<String> ReadRoomAndFeatures(ResidentialListingRequest listing)
        {
            List<String> listRoomTypes = new List<String>();

            // Atrium
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMATR"))
                listRoomTypes.Add("Atrium");

            // Basement
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("BSMNT"))
                listRoomTypes.Add("Basement");

            // Rooms
            int numBedrooms = listing.NumBedsMainLevel != null ? int.Parse(listing.NumBedsMainLevel.ToString()) : 0 + listing.NumBedsOtherLevels != null ? int.Parse(listing.NumBedsOtherLevels.ToString()) : 0;

            if (numBedrooms >= 1)
                listRoomTypes.Add("BEDRO");

            if (numBedrooms >= 2)
                listRoomTypes.Add("Bedroom_II");

            if (numBedrooms >= 3)
                listRoomTypes.Add("Bedroom_III");

            if (numBedrooms >= 4)
                listRoomTypes.Add("Bedroom_IV");

            // Bonus Room

            // Breakfast Room
            if (!String.IsNullOrEmpty(listing.DiningRoomDesc) && listing.DiningRoomDesc.Contains("DBRKA"))
                listRoomTypes.Add("BKFRO");

            // Converted Garage
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMCGR"))
                listRoomTypes.Add("Converted_Garage");

            // Dining Room
            if (!String.IsNullOrEmpty(listing.DiningRoomDesc) && (listing.DiningRoomDesc.Contains("DINL") || listing.DiningRoomDesc.Contains("DFMRM")))
                listRoomTypes.Add("DININ");

            // Entry Foyer
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMFYR"))
                listRoomTypes.Add("Entry_Forer");

            // Family Room
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMFAM"))
                listRoomTypes.Add("FAMIL");

            // Game Room
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMGAM"))
                listRoomTypes.Add("GAMER");

            // Great Room
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMGRT"))
                listRoomTypes.Add("GreatRoom");

            // Gym - Gym

            // Kitchen
            listRoomTypes.Add("KITCH");

            // Library 
            if (!String.IsNullOrEmpty(listing.OtherRoomDesc) && listing.OtherRoomDesc.Contains("RMLIB"))
                listRoomTypes.Add("Library");

            // Living Room 
            listRoomTypes.Add("LIVIN");

            //'Living_Room_II', 'Living Room II',
            //'Loft', 'Loft',
            //'Master_Bath', 'Master Bath',
            //'Master_Bath_II', 'Master Bath II',
            //'MSTRB', 'Master Bedroom',
            //'Master_Bedroom_II', 'Master Bedroom II',
            //'MediaRoom', 'Media Room',
            //'OFFIC', 'Office',
            //'OTHER', 'Other',
            //'Other_Room', 'Other Room',
            //'Other_Room_II', 'Other Room II',
            //'Other_Room_III', 'Other Room III',
            //'SaunaRoom', 'Sauna Room',
            //'UTILI', 'Utility\/Laundry ',
            //'Wine', 'Wine',
            //'WORKS', 'Workshop        ',
            //'GuestHse', 'Guest House'


            return listRoomTypes;
        }

        public UploadResult UploadLeasing(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            throw new NotImplementedException();
        }
    }
}