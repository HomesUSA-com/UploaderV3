using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace Husa.Core.Uploaders.SanAntonio
{
    public class SanAntonioUploader :   IUploader, 
                                        IEditor, 
                                        IImageUploader, 
                                        IPriceUploader, 
                                        IStatusUploader, 
                                        ICompletionDateUploader, 
                                        IUpdateOpenHouseUploader,
                                        IUploadVirtualTourUploader,
                                        ILeaseUploader,
                                        ILotUploader
    {
        OpenHouseBase OH = new OpenHouseBase();

        private String siteURL;

        private string buttonToWait = string.Empty;

        public bool IsFlashRequired { get { return false; } }
        bool IUploader.CanUpload(ResidentialListingRequest listing)
        {
            return listing.MarketName == "San Antonio";
        }

        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);
            this.buttonToWait = driver.UploadInformation.IsNewListing ? "Save & Continue" : "Save Changes";
            if (driver.UploadInformation.IsNewListing)
            {
                Login(driver, listing);
                DoClick(driver, driver.FindElement(By.Id("newlistingLink")));
                NewProperty(driver, listing);
            }
            else
            {
                Login(driver, listing);
                Thread.Sleep(1000);

                EditProperty(driver, listing.MLSNum);

                driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
                Thread.Sleep(1000);
                driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
                Thread.Sleep(1000);

                try
                {
                    driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
                }
                catch { }
            }

            #region General
            FillGeneralListingInformation(driver, listing);
            #endregion

            #region Exterior
            FillExteriorInformation(driver, listing);
            #endregion

            #region Interior
            FillInteriorInformation(driver, listing);
            #endregion

            #region Utilities
            FillUtilitiesInformation(driver, listing);
            #endregion

            #region TaxHoa
            FillTaxHoaInformation(driver, listing);
            #endregion

            #region Office
            FillOfficeInformation(driver, listing);
            #endregion

            #region Remarks
            FillRemarksInformation(driver, listing);
            #endregion

            //FIXME still need to complete this method
            if (driver.UploadInformation.IsNewListing)
            {
                #region Media

                // TODO : Uncomment the below line after the fixes about the Media 
                //FillMedia(driver, listing, media.Value);
                #endregion
                // FinalizeInsert(driver, listing);
            }
            // else
            // FinalizeUpdate(driver);

            #region Sets Geolocation fields
            //SetLongitudeAndLatitudeValues(driver, listing);
            #endregion
            
            return UploadResult.Success;
        }
        public UploadResult Edit(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);
            if (driver.UploadInformation.IsNewListing)
            {
                Login(driver, listing);
                DoClick(driver, driver.FindElement(By.Id("newlistingLink")));
                NewProperty(driver, listing);
            }
            else
            {
                Login(driver, listing);
                Thread.Sleep(1000);

                EditProperty(driver, listing.MLSNum);

                Thread.Sleep(2000);
                driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
                Thread.Sleep(1000);
                driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
                Thread.Sleep(1000);

                try
                {
                    driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
                }
                catch { }
            }

            return UploadResult.Success;
        }
        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Login(driver, listing);
            Thread.Sleep(1000);

            EditProperty(driver, listing.MLSNum);

            Thread.Sleep(2000);
            driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
            driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[0].click();");
            Thread.Sleep(1000);

            try 
            {
                driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
            } 
            catch { }

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("csframe")));
            driver.SwitchTo("csframe");
            driver.SetSelect(By.Id("statuses"), listing.ListStatus);// Listing Status

            if (listing.ListStatus == "SLD")
            {
                driver.wait.Until(x => driver.FindElement(By.Name("HOWSOLD")).Displayed);
                driver.WriteTextbox(By.Name("HOWSOLD"), listing.MFinancing); // How Sold/Sale Terms
                driver.WriteTextbox(By.Name("CLOSEDATE"), listing.ClosedDate.Value.Date.ToString("MM/dd/yyyy")); // Closing Date
                driver.WriteTextbox(By.Name("SOLDPRICE"), listing.SalesPrice); // Sold Price
                driver.WriteTextbox(By.Name("SELLCONCES"), listing.SellerPaid); // Seller Concessions
                driver.WriteTextbox(By.Name("SELL_CONC_DESC"), ""); // Seller Concessions Description
                driver.WriteTextbox(By.Name("SELL_CONC_DESC"), listing.SellerPaid > 0 ? listing.BuyerIncentiveDesc : "NONE"); // Seller Concessions Description
            }

            if (listing.ListStatus == "PDB" || listing.ListStatus == "PND" || listing.ListStatus == "SLD")
            {
                driver.wait.Until(x => driver.FindElement(By.Name("CONTDATE")).Displayed);
                driver.WriteTextbox(By.Name("CONTDATE"), listing.PendingDate.Value.Date.ToString("MM/dd/yyyy"));// Contract Date
                driver.WriteTextbox(By.Name("SELLAGT1"), listing.SellingAgentLicenseNum);// Selling / Buyer's Agent ID
            }

            return UploadResult.Success;
        }

        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Login(driver, listing);
            Thread.Sleep(1000);

            EditProperty(driver, listing.MLSNum);

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
            driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[1].click();");
            Thread.Sleep(1000);
            try
            {
                driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
            }
            catch { }
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("csframe")));
            driver.SwitchTo("csframe");
            try
            {
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)driver).ExecuteScript("jQuery('#LISTPRICE').attr('onchange','');");
                driver.WriteTextbox(By.Id("LISTPRICE"), listing.ListPrice);//List Price
            }
            catch (UnhandledAlertException alertEx)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("jQuery('#LISTPRICE').attr('onchange','');");
                driver.WriteTextbox(By.Id("LISTPRICE"), listing.ListPrice);
            }

            return UploadResult.Success;
        }

        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;
            Login(driver, listing);
            Thread.Sleep(1000);

            EditProperty(driver, listing.MLSNum);

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
            driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[5].click();");
            Thread.Sleep(1000);
            try
            {
                driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
            }
            catch { }

            ProcessImages(driver, media.OfType<ResidentialListingMedia>());
            return UploadResult.Success;
        }

        public UploadResult UpdateCompletionDate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;
            Login(driver, listing);
            Thread.Sleep(1000);

            EditProperty(driver, listing.MLSNum);

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
            driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
            Thread.Sleep(1000);
            try
            {
                driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
            }
            catch { }

            // MLS-130
            driver.ExecuteScript("clearPicklist('MISCELANEStable');selectVals('MISCELANES');;closeDiv();");

            FillRemarksInformation(driver, listing, true);
            return UploadResult.Success;
        }

        public UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;
            Login(driver, listing);
            Thread.Sleep(1000);

            EditProperty(driver, listing.MLSNum);

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
            driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[7].click();");
            Thread.Sleep(1000);
            try
            {
                driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
            }
            catch { }

            DeleteOpenHouses(driver, listing);

            driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
            driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[7].click();");
            Thread.Sleep(1000);

            if (listing.EnableOpenHouse)
            {
                AddOpenHouses(driver, listing);
            }

            return UploadResult.Success;
        }

        #region Virtual Tour

        public UploadResult UploadVirtualTour(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;
            Login(driver, listing);

            EditProperty(driver, listing.MLSNum);

            Thread.Sleep(1000);
            driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
            Thread.Sleep(1000);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal-dialog")));
            driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[6].click();");
            Thread.Sleep(1000);

            try
            {
                driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
            }
            catch { }

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("main")));
            driver.SwitchTo("main");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("workspace")));
            driver.SwitchTo("workspace");
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("VIRTTOUR")));
            var virtualTour = media.OfType<ResidentialListingVirtualTour>().FirstOrDefault();
            if (virtualTour != null)
                driver.WriteTextbox(By.Id("VIRTTOUR"), virtualTour.VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded

            return UploadResult.Success;
        }

        #endregion

        private bool isElementPresent(By by, CoreWebDriver driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException e)
            {
                return false;
            }
        }

        public UploadResult Logout(CoreWebDriver driver)
        {
            driver.SwitchTo().DefaultContent();
            driver.Click(By.CssSelector("a[href='servlet/SignOut']"));
            //TODO: Finish Logout for San Antonio
            return UploadResult.Success;
        }

        /// <summary>
        /// Logs in to SABOR's website to process the current listing request
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="optLinkText">Link to be clicked on after it has logged in</param>
        public LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            #region login
            // Connect to the login page
            this.siteURL = "http://sabor.connectmls.com/slogin.jsp";
            driver.Navigate().GoToUrl(siteURL);
            try
            {
                driver.wait.Until(x => driver.FindElement(By.Name("go")).Displayed);
                driver.WriteTextbox(By.Id("j_username"), listing.MarketUsername);
                driver.WriteTextbox(By.Id("j_password"), listing.MarketPassword);
                DoClick(driver, driver.FindElement(By.Name("go")));
            }
            catch {
                driver.wait.Until(x => driver.FindElement(By.Name("login")).Displayed);
                // Fill the login and password
                driver.WriteTextbox(By.Id("userid"), listing.MarketUsername);
                driver.WriteTextbox(By.Id("password"), listing.MarketPassword);
                driver.FindElement(By.Name("login")).Submit();
            }

            #endregion

            #region UP-89
            try
            {
                if (driver.FindElement(By.Name("remindLater")).Displayed.Equals(true))
                {
                    driver.FindElement(By.Name("remindLater")).Click();
                }
            } catch { }

            #endregion

            return LoginResult.Logged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="listing"></param>
        /// <param name="fieldName"></param>
        /// <param name="containerClassName"></param>
        /// <param name="childIndex"></param>
        private void SetVaueToSelectField(CoreWebDriver driver, String fieldId, String containerClassName, int childIndex, String fieldValue)
        {
            driver.ExecuteScript("$('." + containerClassName + " select:eq(" + childIndex + ")').attr('id', '" + fieldId + "');");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id(fieldId)));
            driver.SetSelect(By.Id(fieldId), fieldValue); 
        }

        private void SetVaueToTextField(CoreWebDriver driver, String fieldId, String containerClassName, int childIndex, String fieldValue)
        {
            driver.ExecuteScript("$('." + containerClassName + " input[type=\"text\"]:eq(" + childIndex + ")').attr('id', '" + fieldId + "');");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id(fieldId)));
            driver.WriteTextbox(By.Id(fieldId), fieldValue);
        }

        /// <summary>
        /// Begins the process of loading a new listing into the system
        /// </summary>
        private void NewProperty(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("dcModal")));
            
            // TODO : Remove the below line
            listing.Category = "RE";
            
            this.SetVaueToSelectField(driver, "category", "property-type-selector", 0, listing.Category); // Class
            ((IJavaScriptExecutor)driver).ExecuteScript("$('.search-options input[type=\"checkbox\"]').attr('id', 'autoPopulateFromTax');$('#autoPopulateFromTax').click();"); // Auto-populate from Tax data
            ((IJavaScriptExecutor)driver).ExecuteScript("$('.search-options input[type=\"checkbox\"]:last').attr('id', 'manuallyEnterAllData');$('#manuallyEnterAllData').click();"); // Manually enter all listing data
            ((IJavaScriptExecutor)driver).ExecuteScript("$('.modal-header button:eq(1)').click();"); // Next>> 

            Thread.Sleep(1000);
            
            //Second Page of new listing
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("save-page")));
            driver.WriteTextbox(By.Id("AREAID"), "3100" /*listing.MLSArea*/); // Area
            driver.WriteTextbox(By.Id("FERGUSON"), listing.MapscoMapCoord); // Mapsco Grid
            driver.WriteTextbox(By.Id("LISTPRICE"), listing.ListPrice.ToString()); // List Price
            driver.WriteTextbox(By.Id("ADDRNUMBER"), listing.StreetNum.ToString()); // Street Number
            driver.WriteTextbox(By.Id("ADDRSTREET"), listing.StreetName); // Street Name
            driver.WriteTextbox(By.Id("CITYID"), listing.City); // City

            // TODO : Remove the below line
            listing.State = "TX";
            driver.WriteTextbox(By.Id("STATEID"), listing.State); // State
            driver.WriteTextbox(By.Id("zip5"), listing.Zip); // Zip
            driver.WriteTextbox(By.Id("COUNTYID"), listing.County); // County
            if (listing.TaxID != null)
                driver.WriteTextbox(By.Id("COUNTACTNO"), listing.TaxID);

            driver.WriteTextbox(By.Id("MULTIPLE_CANSID"), "NO"); // Multiple County AcctNos


            if (driver.FindElement(By.Id("AREAID")) != null)
            {
                driver.FindElement(By.Id("save-page")).Click();

                try
                {
                    if (driver.FindElement(By.ClassName("address-suggestion")) != null && 
                        driver.FindElement(By.ClassName("address-suggestion")).Displayed)
                    {

                            ((IJavaScriptExecutor)driver).ExecuteScript("ignoreSuggestions();");
                            driver.FindElement(By.Id("save-page")).Click();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Begins the process of loading a new lesing into the system
        /// </summary>
        private void NewLeasingProperty(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("dcModal")));
            this.SetVaueToSelectField(driver, "category", "property-type-selector", 0, "RR"); // Class
            ((IJavaScriptExecutor)driver).ExecuteScript("$('.search-options input[type=\"checkbox\"]').attr('id', 'autoPopulateFromTax');$('#autoPopulateFromTax').click();"); // Auto-populate from Tax data
            ((IJavaScriptExecutor)driver).ExecuteScript("$('.search-options input[type=\"checkbox\"]:last').attr('id', 'manuallyEnterAllData');$('#manuallyEnterAllData').click();"); // Manually enter all listing data
            ((IJavaScriptExecutor)driver).ExecuteScript("$('.modal-header button:eq(1)').click();"); // Next>> 

            Thread.Sleep(1000);

            //Second Page of new listing
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("save-page")));

            driver.WriteTextbox(By.Id("AREAID"), listing.MLSArea); // Area
            driver.WriteTextbox(By.Id("FERGUSON"), listing.MapscoMapCoord); // Mapsco Grid
            driver.WriteTextbox(By.Id("LISTPRICE"), listing.ListPrice.ToString()); // List Price
            driver.WriteTextbox(By.Id("ADDRNUMBER"), listing.StreetNum != null ? listing.StreetNum.ToString() : ""); // Street Number
            driver.WriteTextbox(By.Id("ADDRSTREET"), listing.StreetName); // Street Name
            driver.WriteTextbox(By.Id("CITYID"), listing.CityCode); // City
            driver.WriteTextbox(By.Id("STATEID"), listing.State); // State
            driver.WriteTextbox(By.Id("zip5"), listing.Zip); // Zip
            driver.WriteTextbox(By.Id("COUNTYID"), listing.County); // County
            if (listing.TaxID != null)
                driver.WriteTextbox(By.Id("COUNTACTNO"), listing.TaxID);

            driver.WriteTextbox(By.Id("MULTIPLE_CANSID"), "NO"); // Multiple County AcctNos


            if (driver.FindElement(By.Id("AREAID")) != null)
            {
                driver.FindElement(By.Id("Save & Continue")).Click();

                try
                {
                    if (driver.FindElement(By.ClassName("address-suggestion")) != null &&
                        driver.FindElement(By.ClassName("address-suggestion")).Displayed)
                    {

                        ((IJavaScriptExecutor)driver).ExecuteScript("ignoreSuggestions();");
                        driver.FindElement(By.Id("Save & Continue")).Click();
                    }
                }
                catch { }
            }
            FillBasicInformation(driver, listing);
        }

        /// <summary>
        /// Begins the process of loading an existing listing into the system
        /// </summary>
        private void EditProperty(CoreWebDriver driver, string mlsnum)
        {
            driver.ScrollDown(500);
            driver.FindElement(By.Id("listTxnsLink")).Click();
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("mylistcontainer")));

            driver.ExecuteScript("jQuery('.select-list-styled:eq(1) > select').attr('id','selectlist');");
            driver.SetSelect(By.Id("selectlist"), "ALL");
            Thread.Sleep(2000);

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("mylistings")));

            Boolean mlsFound = false;
            while (!mlsFound)
            {
                try
                {
                    String result = driver.ExecuteScript("return jQuery('.dctable-cell > a:contains(\"" + mlsnum + "\")').length;").ToString();
                    if (result != "0")
                    {
                        mlsFound = true;
                    }
                    else
                    {
                        String nextButtonVisible = driver.ExecuteScript("return jQuery('.main-content > .mylistcontainer > .affix-top > ul.pagination > li').length;").ToString();
                        if (!String.IsNullOrEmpty(nextButtonVisible) && nextButtonVisible != "0")
                        {
                            int nextButtonIndex = (int.Parse(nextButtonVisible) > 0) ? (int.Parse(nextButtonVisible) - 1) : 0;
                            String nextButtonDisabled = driver.ExecuteScript("return jQuery('ul.pagination > li:eq(" + nextButtonIndex + ")').is(':disabled');").ToString();
                            if (Boolean.Parse(nextButtonDisabled) == false)
                            {
                                driver.ExecuteScript("return jQuery('ul.pagination > li:eq(" + (nextButtonIndex - 1) + ") > a').click();");
                                mlsFound = false;
                            }
                        }
                    }
                    Thread.Sleep(3000);
                }
                catch { Thread.Sleep(3000); }
            }
        }

        /// <summary>
        /// Fills the information for the General Tab
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillGeneralListingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            String propertyType = listing.Category == "RE" ? "SFD" : "TWNHM";
            driver.SetAttribute(By.Name("TYPE"), propertyType, "value");//Type

            driver.WriteTextbox(By.Name("BLOCK"), listing.Block);//Block
            driver.WriteTextbox(By.Name("LGLDSCLOT"), listing.LotNum);//Legal Desc-Lot
            //driver.WriteTextbox(By.Name("CBORNCB"), listing.CBNCB), newListing);
            driver.SetAttribute(By.Name("SBDIVISION"), listing.Subdivision, "value");//Subdivision (Legal Name)
            driver.SetAttribute(By.Name("SUBDIVISION_CKA"), listing.Subdivision, "value");//Subdivision (Common Name)
            driver.WriteTextbox(By.Name("LEGALDESC"), listing.Legal); //Legal Description

            string direction = listing.Directions;
            if (!String.IsNullOrEmpty(direction))
            {
                direction = direction.RemoveSlash();
                int dirLen = direction.Length;
                if (direction.ElementAt(dirLen - 1) == '.')
                    direction = direction.Remove(dirLen - 1);
                else
                    direction = direction + ".";
            }
            driver.WriteTextbox(By.Name("INSTORDIR"), direction);//Inst/Dir
            driver.WriteTextbox(By.Name("HOMEFACES"), listing.FacesDesc, true);//Home Faces

            // TODO : remove the below line
            //if (listing.YearBuilt == null)
                listing.YearBuilt = 2021;
            if (String.IsNullOrEmpty(listing.YearBuiltDesc) || listing.YearBuiltDesc == "complete")
            {
                listing.YearBuiltDesc = "C";
            } 
            else
            {
                listing.YearBuiltDesc = "I";
            }


            if (listing.YearBuilt.HasValue)
                driver.WriteTextbox(By.Name("YEAR_BUILT"), listing.YearBuilt.Value); //Year Built
            else
            {
                IWebElement elem = null;
                try
                {
                    elem = driver.FindElement(By.Id("unknownYEAR_BUILTU"));
                }
                catch
                { }

                if (elem != null && !elem.Selected)
                {
                    driver.Click(By.Id("unknownYEAR_BUILTU"));
                }
            }
            driver.WriteTextbox(By.Name("SQFEET"), listing.SqFtTotal, true);//Square Feet
            driver.WriteTextbox(By.Name("SOURCESQFT"), listing.SqFtSource, true);//Source SQFT/Acre
            driver.SetAttribute(By.Name("SCHLDIST"), listing.SchoolDistrict.ToUpper(), "value");//School District
            driver.SetAttribute(By.Name("ELEMSCHL"), listing.SchoolName1, "value");//Elementary School

            driver.SetAttribute(By.Name("MIDSCHL"), listing.SchoolName2, "value");//Middle School
            driver.SetAttribute(By.Name("HIGHSCHL"), listing.SchoolName3, "value");//High School
            driver.WriteTextbox(By.Name("CONSTRCTN"), "NEW");//Construction
            driver.WriteTextbox(By.Name("BLDRNAME"), listing.OwnerName);//Builder Name

            // TODO : Remove the below line
            //if(String.IsNullOrEmpty(listing.HasHandicapAmenities))
                listing.HasHandicapAmenities = "NO";
            driver.WriteTextbox(By.Name("ACCESS_HOME"), listing.HasHandicapAmenities);//Accessible/Adaptive Home

            driver.WriteTextbox(By.Name("NGHBRHDMNT"), listing.CommonFeatures);//Neighborhood Amenities
            if (listing.HasHandicapAmenities == "YES")
            {
                //driver.WriteTextbox(By.Name("ACCESS_HOME"), "YES", true);//Accessible/Adaptive Details
                //((IJavaScriptExecutor)driver).ExecuteScript("verifyVals('ACCESS_HOME', 'Accessible/Adaptive Home', true, true, true, false, false); ; ACCESS_HOMEActions()");
                //((IJavaScriptExecutor)driver).ExecuteScript(" selectVals('ACCESS_HOME');;ACCESS_HOMEActions();closeDiv(); ");

                if(!String.IsNullOrEmpty(listing.AccessibilityDesc))
                {
                    driver.WriteTextbox(By.Name("ACESIBILTY"), listing.AccessibilityDesc, false);//Accessible/Adaptive Details
                    //((IJavaScriptExecutor)driver).ExecuteScript(" verifyVals('ACESIBILTY', 'Accessible/Adaptive Details', false, true, true, false, false); ");
                    //((IJavaScriptExecutor)driver).ExecuteScript(" selectVals('ACESIBILTY');;closeDiv(); ");
                }
            }
            if (listing.YearBuiltDesc == "I")
                driver.WriteTextbox(By.Name("MISCELANES"), "UNDCN", true);//Miscellaneous
            else
                driver.ExecuteScript("clearPicklist('MISCELANEStable');selectVals('MISCELANES');closeDiv();");

            driver.WriteTextbox(By.Name("GREEN_CERT"), listing.GreenCerts, true);//Green Certification
            driver.WriteTextbox(By.Name("GREEN_FEAT"), listing.GreenFeatures, true);//Green Features
            driver.WriteTextbox(By.Name("ENERGY_EFF"), listing.EnergyDesc, true);//Energy Efficiency
        }

        /// <summary>
        /// Fills the information for the General Tab
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillGeneralLeasingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            driver.SetAttribute(By.Name("TYPE"), "SFDET", "value");//Type
            string direction = listing.Directions;
            if (!String.IsNullOrEmpty(direction))
            {
                direction = direction.RemoveSlash();
                int dirLen = direction.Length;
                if (direction.ElementAt(dirLen - 1) == '.')
                    direction = direction.Remove(dirLen - 1);
                else
                    direction = direction + ".";
            }
            driver.WriteTextbox(By.Name("INSTORDIR"), direction);//Inst/Dir
            driver.SetAttribute(By.Name("SBDIVISION"), listing.Subdivision, "value");//Subdivision (Legal Name)
            driver.SetAttribute(By.Name("SUBDIVISION_CKA"), listing.Subdivision, "value");//Subdivision (Common Name)
                                                                                          //driver.WriteTextbox(By.Name("CONDONAME"), listing.); // Condo Name
            driver.WriteTextbox(By.Name("LEGALDESC"), listing.Legal); //Legal Description
            if (listing.YearBuilt.HasValue)
                driver.WriteTextbox(By.Name("YEAR_BUILT"), listing.YearBuilt.Value); //Year Built
            else
            {
                IWebElement elem = null;
                try
                {
                    elem = driver.FindElement(By.Id("unknownYEAR_BUILTU"));
                }
                catch
                { }

                if (elem != null && !elem.Selected)
                {
                    driver.Click(By.Id("unknownYEAR_BUILTU"));
                }
            }
            driver.WriteTextbox(By.Name("SQFEET"), listing.SqFtTotal, true);//Square Feet
            driver.WriteTextbox(By.Name("SOURCESQFT"), listing.SqFtSource, true);//Source SQFT/Acre
            driver.SetAttribute(By.Name("SCHLDIST"), listing.SchoolDistrict.ToUpper(), "value");//School District
            driver.SetAttribute(By.Name("ELEMSCHL"), listing.SchoolName1, "value");//Elementary School
            driver.SetAttribute(By.Name("MIDSCHL"), listing.SchoolName2, "value");//Middle School
            driver.SetAttribute(By.Name("HIGHSCHL"), listing.SchoolName3, "value");//High School
            driver.WriteTextbox(By.Name("CONSTRCTN"), "NEW");//Construction
            driver.WriteTextbox(By.Name("BLDRNAME"), listing.OwnerName);//Builder Name
            driver.WriteTextbox(By.Name("RENTINCLDS"), listing.RentIncludes, true);// Rent Includes
            driver.WriteTextbox(By.Name("TENANTPAYS"), listing.TenantPays, true); // Tenant Pays
            driver.WriteTextbox(By.Name("RESTRICTNS"), listing.Restrictions, true); // Restrictions
            driver.WriteTextbox(By.Name("COMONRMNTS"), listing.CommonFeatures, true); // Common Area Amenities
            driver.WriteTextbox(By.Name("ACCESS_HOME"), listing.HasHandicapAmenities, true); // Accessible/Adaptive Home
            driver.WriteTextbox(By.Name("SECTION8"), "NO", true); // Section 8 Qualified
            driver.WriteTextbox(By.Name("GREEN_FEAT"), listing.GreenFeatures, true);//Green Features
            driver.WriteTextbox(By.Name("ENERGY_EFF"), listing.EnergyDesc, true);//Energy Efficiency
        }

        /// <summary>
        /// Fills the information for the Exterior Tab
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillExteriorInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('1') ");
            Thread.Sleep(2000);

            // TODO : Remove the below lines
            //if (String.IsNullOrEmpty(listing.HousingTypeDesc))
                listing.HousingTypeDesc = "1STRY,TRDNL";
            driver.WriteTextbox(By.Name("STYLE"), listing.HousingTypeDesc); //Style
            driver.FindElement(By.Name("STYLE")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            // TODO : Remove the below lines
            //if(String.IsNullOrEmpty(listing.NumStories))
                listing.NumStories = "2";
            driver.WriteTextbox(By.Name("NOSTRY"), listing.NumStories); //# of Stories

            driver.FindElement(By.Name("NOSTRY")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            // TODO : Remove the below lines
            //if (String.IsNullOrEmpty(listing.ExteriorFeatures))
                listing.ExteriorFeatures = "BRICK,WOOD,VINYL";
            driver.WriteTextbox(By.Name("EXTERIOR"), listing.ExteriorFeatures); //Exterior
            driver.FindElement(By.Name("EXTERIOR")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.SetAttribute(By.Name("ROOF"), listing.RoofDesc, "value");//Roof
            driver.FindElement(By.Name("ROOF")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.WriteTextbox(By.Name("FOUNDATION"), listing.FoundationDesc); //Foundation
            driver.FindElement(By.Name("FOUNDATION")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            try
            {
                if(!String.IsNullOrWhiteSpace(listing.ParkingDesc))
                {
                    driver.WriteTextbox(By.Name("PARKING"), listing.ParkingDesc); //Parking
                    driver.FindElement(By.Name("PARKING")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                ShowMarkToFieldByNameElement(driver, "PARKING");
            }
            try { driver.SwitchTo().Alert().Accept(); } catch {  }
            ((IJavaScriptExecutor)driver).ExecuteScript(" closeDiv(); ");

            driver.WriteTextbox(By.Name("ADDL_PARKING"), listing.OtherParking); //Additional/Other Parking
            driver.FindElement(By.Name("ADDL_PARKING")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            //HY-277
            //driver.WriteTextbox(By.Name("NOGARSPC"), listing.GarageCapacity); //# Parking Spaces

            // TODO : Remove the below lines
            if(listing.HasPool != null && listing.HasPool == true)
            {
                driver.WriteTextbox(By.Name("POOL"), "Y"); //Pool
            }
            else
            {
                driver.WriteTextbox(By.Name("POOL"), "N"); //Pool
            }
            
            if (!string.IsNullOrWhiteSpace(listing.PoolDesc))//Pool/Spa
                driver.WriteTextbox(By.Name("POOLSPA"), listing.PoolDesc);
            else
                driver.WriteTextbox(By.Name("POOLSPA"), "NONE");

            driver.WriteTextbox(By.Name("EXTERRFTRS"), listing.ExteriorDesc, true);//Exterior Features
            driver.FindElement(By.Name("EXTERRFTRS")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.WriteTextbox(By.Name("LOTSIZE"), listing.LotSize); //Lot Size (Acres)
            driver.FindElement(By.Name("LOTSIZE")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.WriteTextbox(By.Name("LOTDSCRPTN"), listing.LotDesc, true); //Lot Description
            driver.FindElement(By.Name("LOTDSCRPTN")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.WriteTextbox(By.Name("LOTDIMENSIONS"), listing.LotDim, true); //Lot Dimensions
            driver.FindElement(By.Name("LOTDIMENSIONS")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            driver.WriteTextbox(By.Name("LTMPRVMNTS"), listing.UtilitiesDesc, true); //Lot Improvements

            //moved to the end because the app is not filling it
            
        }

        /// <summary>
        /// Fills the information for the Exterior Tab
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillExteriorLeasingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('1') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("STYLE"), listing.HousingStyleDesc); //Style
            driver.FindElement(By.Name("STYLE")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("STORIES"), listing.NumStories); //# of Stories
            driver.FindElement(By.Name("STORIES")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("EXTERIOR"), listing.ConstructionDesc); //Exterior
            driver.FindElement(By.Name("EXTERIOR")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("ROOF"), listing.RoofDesc);//Roof
            driver.FindElement(By.Name("ROOF")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("FOUNDATION"), listing.FoundationDesc); //Foundation
            driver.FindElement(By.Name("FOUNDATION")).SendKeys(Keys.Tab);
            try
            {
                if (!String.IsNullOrWhiteSpace(listing.ParkingDesc))
                {
                    driver.WriteTextbox(By.Name("PARKING"), listing.ParkingDesc); //Parking
                    driver.FindElement(By.Name("PARKING")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                //ShowMarkToFieldByNameElement(driver, "PARKING");
            }
            try { driver.SwitchTo().Alert().Accept(); } catch { }
            ((IJavaScriptExecutor)driver).ExecuteScript(" closeDiv(); ");

            driver.WriteTextbox(By.Name("ADDL_PARKING"), listing.OtherParking); //Additional/Other Parking
            driver.FindElement(By.Name("ADDL_PARKING")).SendKeys(Keys.Tab);

            //HY-277
            //driver.WriteTextbox(By.Name("NOGARSPC"), listing.GarageCapacity); //# Parking Spaces

            //driver.WriteTextbox(By.Name("POOL"), listing.HasPool); //Pool
            if (!string.IsNullOrWhiteSpace(listing.PoolDesc))//Pool/Spa 
            {
                driver.WriteTextbox(By.Name("POOLSPA"), listing.PoolDesc);
            }
            else
            {
                driver.WriteTextbox(By.Name("POOLSPA"), "NONE");
            }
            driver.FindElement(By.Name("POOLSPA")).SendKeys(Keys.Tab);


            driver.WriteTextbox(By.Name("EXTERRFTRS"), listing.ExteriorDesc, true);//Exterior Features
            driver.FindElement(By.Name("EXTERRFTRS")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("LOTSIZE"), listing.LotSize); //Lot Size (Acres)
            
            driver.WriteTextbox(By.Name("LOTDSCRPTN"), listing.LotDesc, true); //Lot Description
            driver.FindElement(By.Name("LOTDSCRPTN")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("LOTDIMENSIONS"), listing.LotDim, true); //Lot Dimensions

            //driver.WriteTextbox(By.Name("LTMPRVMNTS"), listing.UtilitiesDesc, true); //Lot Improvements
        }

        /// <summary>
        /// Fills the information for the Interior Tab
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillInteriorInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('2') ");
            Thread.Sleep(2000);

            // 1.
            // TODO: Remove the below line
            listing.InteriorDesc = "1LVAR";
            driver.WriteTextbox(By.Name("INTERIOR"), listing.InteriorDesc); //Interior

            // 2. 
            driver.WriteTextbox(By.Name("INCLUSIONS"), listing.InclusionsDesc); //Inclusions

            // 3.
            try
            {
                if(!String.IsNullOrWhiteSpace(listing.FloorsDesc))
                {
                    driver.WriteTextbox(By.Name("FLOOR"), listing.FloorsDesc); //Floor
                    driver.FindElement(By.Name("FLOOR")).SendKeys(Keys.Tab);
                }    
            }
            catch
            {
                ShowMarkToFieldByNameElement(driver, "FLOOR");
            }
            try { driver.SwitchTo().Alert().Accept(); } catch { }

            // 4.
            try
            {
                if (!String.IsNullOrWhiteSpace(listing.NumberFireplaces))
                {
                    driver.WriteTextbox(By.Name("FIREPLACE1"), listing.NumberFireplaces); //# Fireplaces
                    driver.FindElement(By.Name("FIREPLACE1")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                ShowMarkToFieldByNameElement(driver, "FIREPLACE1");
            }
            try { driver.SwitchTo().Alert().Accept(); } catch { }
            
            ((IJavaScriptExecutor)driver).ExecuteScript(" closeDiv(); ");

            // 5.

            //driver.SetAttribute(By.Name("FIREPLACE1"), listing.NumberFireplaces, "value");//# Fireplaces
            //FIXME is not filling in this value.
            try
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Name("FIREPLACE")));
                if (!String.IsNullOrWhiteSpace(listing.FireplaceDesc) && !listing.FireplaceDesc.Equals("NA"))
                {
                    driver.WriteTextbox(By.Name("FIREPLACE"), listing.FireplaceDesc); //Fireplace
                    driver.FindElement(By.Name("FIREPLACE")).SendKeys(Keys.Tab);
                } 
            }
            catch
            {
                ShowMarkToFieldByNameElement(driver, "FIREPLACE");
            }
            try { driver.SwitchTo().Alert().Accept(); } catch { }
            ((IJavaScriptExecutor)driver).ExecuteScript(" closeDiv(); ");
            
            // 6.
            driver.WriteTextbox(By.Name("WNDWCVRNGS"), listing.WindowCoverings); //Window Coverings

            // 7.
            driver.WriteTextbox(By.Name("BEDROOMS"), listing.Beds); //Bedrooms
            ((IJavaScriptExecutor)driver).ExecuteScript(" BEDROOMSActions() ");

            // 8.
            // TODO : Remove the below line
            //if (String.IsNullOrEmpty(listing.Bed1Desc))
                listing.Bed1Desc = "DWNST,WLKIN,CLFAN,FLBT";
            driver.WriteTextbox(By.Name("MASTERBDRM"), listing.Bed1Desc); //Master Bedroom

            // 9.
            if(listing.ClosetLength > 0 && listing.ClosetWidth > 0)
            {
                driver.WriteTextbox(By.Name("leftMBRCLOSET_SIZE"), listing.ClosetLength);
                driver.WriteTextbox(By.Name("rightMBRCLOSET_SIZE"), listing.ClosetWidth);
                driver.FindElement(By.Name("rightMBRCLOSET_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("MBRCLOSET_LEVEL"), listing.Bed1Level, true); // Master Bedroom Level
            }

            // 9.
            driver.WriteTextbox(By.Name("leftADDEDIT_BATHS"), listing.BathsFull); //Bathrooms Full
            driver.WriteTextbox(By.Name("rightADDEDIT_BATHS"), listing.BathsHalf); //Bathrooms Half
            driver.FindElement(By.Name("rightADDEDIT_BATHS")).SendKeys(Keys.Tab);
            Thread.Sleep(500);

            // 10.
            // TODO : Remove the below lines
            //if (String.IsNullOrEmpty(listing.BedBathDesc))
                listing.BedBathDesc = "TSCMB,HS/HR,DBLVN";
            driver.WriteTextbox(By.Name("MASTERBATH"), listing.BedBathDesc); //Master Bath
            ((IJavaScriptExecutor)driver).ExecuteScript(" MASTERBATHActions(); closeDiv(); ");
            //selectVals('MASTERBATH'); ; MASTERBATHActions(); closeDiv();
            // 11.
            if (listing.LivingRoom3Length != null && listing.LivingRoom3Width != null)//Entry Room Size
            {
                driver.WriteTextbox(By.Name("leftENTRM_SIZE"), listing.LivingRoom3Length, true);  //Length
                driver.WriteTextbox(By.Name("rightENTRM_SIZE"), listing.LivingRoom3Width, true); //Width
                driver.FindElement(By.Name("rightENTRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("ENTRM_LEVEL"), listing.LivingRoom3Level, true); // Entry Room level
            }

            // 12.
            if (listing.LivingRoom1Length != null && listing.LivingRoom1Width != null) //Living Room Size
            {
                driver.WriteTextbox(By.Name("leftLVRM_SIZE"), listing.LivingRoom1Length, true); //Lenght
                driver.WriteTextbox(By.Name("rightLVRM_SIZE"), listing.LivingRoom1Width, true); //Width
                driver.FindElement(By.Name("rightLVRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("LVRM_LEVEL"), listing.LivingRoom1Level, true); // Living Room level
            }

            // 13.
            if (listing.LivingRoom2Length != null && listing.LivingRoom2Width != null)//Family Room Size
            {
                driver.WriteTextbox(By.Name("leftFAMRM_SIZE"), listing.LivingRoom2Length, true);//Lenght
                driver.WriteTextbox(By.Name("rightFAMRM_SIZE"), listing.LivingRoom2Width, true);//Width
                driver.FindElement(By.Name("rightFAMRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("FAMRM_LEVEL"), listing.LivingRoom2Level, true); // Family Room level
            }

            // 14.
            if (listing.StudyLength != null && listing.StudyWidth != null)//Study/Office Size
            {
                driver.WriteTextbox(By.Name("leftSTYOROF_SIZE"), listing.StudyLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightSTYOROF_SIZE"), listing.StudyWidth, true);//Width
                driver.FindElement(By.Name("rightSTYOROF_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("STYOROF_LEVEL"), listing.StudyLevel, true); // Study/Office level
            }

            // 15.
            if (listing.KitchenLength != null && listing.KitchenWidth != null)//Kitchen Size
            {
                driver.WriteTextbox(By.Name("leftKIT_SIZE"), listing.KitchenLength);//Lenght
                driver.WriteTextbox(By.Name("rightKIT_SIZE"), listing.KitchenWidth);//Width
                driver.FindElement(By.Name("rightKIT_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("KIT_LEVEL"), listing.KitchenLevel, true); // Kitchen Room level
            }

            // 16.
            if (listing.BreakfastLength != null && listing.BreakfastWidth != null)//Breakfast Room Size
            {
                driver.WriteTextbox(By.Name("leftBFSTRM_SIZE"), listing.BreakfastLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightBFSTRM_SIZE"), listing.BreakfastWidth, true);//Width
                driver.FindElement(By.Name("rightBFSTRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("BFSTRM_LEVEL"), listing.BreakfastLevel, true); // Breakfast Room level
            }

            // 17.
            if (listing.DiningRoomLength != null && listing.DiningRoomWidth != null)//Dining Room Size
            {
                driver.WriteTextbox(By.Name("leftDINR_SIZE"), listing.DiningRoomLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightDINR_SIZE"), listing.DiningRoomWidth, true);//Width
                driver.FindElement(By.Name("rightDINR_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("DINR_LEVEL"), listing.DiningRoomLevel, true); // Dining Room level
            }

            // 18.
            if (listing.UtilityRoomLength != null && listing.UtilityRoomWidth != null)//Utility Room Size
            {
                driver.WriteTextbox(By.Name("leftUTLRM_SIZE"), listing.UtilityRoomLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightUTLRM_SIZE"), listing.UtilityRoomWidth, true);//Width
                driver.FindElement(By.Name("rightUTLRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("UTLRM_LEVEL"), listing.UtilityRoomLevel, true); // Utility Room level
            }

            // 19.
            if (listing.Bed1Length != null && listing.Bed1Width != null)//Master Bedroom Size
            {
                driver.WriteTextbox(By.Name("leftMBR_SIZE"), listing.Bed1Length);//Lenght
                driver.WriteTextbox(By.Name("rightMBR_SIZE"), listing.Bed1Width);//Width
                driver.FindElement(By.Name("rightMBR_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("MBR_LEVEL"), listing.Bed1Level, true); // Master Bedroom level
            }

            // 19.1
            if (listing.Bed1Desc != null && listing.Bed1Desc.Contains("DUAL") && listing.Mbr2Len != null && listing.Mbr2Wid != null)//Master Bedroom 2 Size
            {
                driver.WriteTextbox(By.Name("leftMBR2_SIZE"), listing.Mbr2Len);//Lenght
                driver.WriteTextbox(By.Name("rightMBR2_SIZE"), listing.Mbr2Wid);//Width
                driver.FindElement(By.Name("rightMBR2_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("MBR2_LEVEL"), listing.MBR2LEVEL, true); // Master Bedroom level
            }

            try
            {
                // 20.
                if (listing.Bath1Length != null && listing.Bath1Width != null)//Master Bath Size
                {
                    driver.WriteTextbox(By.Name("leftMBTH_SIZE"), listing.Bath1Length);//Lenght
                    driver.WriteTextbox(By.Name("rightMBTH_SIZE"), listing.Bath1Width);//Width
                    driver.FindElement(By.Name("rightMBTH_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    driver.WriteTextbox(By.Name("MBTH_LEVEL"), listing.Bath1Level, true); // Master Bedroom level
                }
            }
            catch { }

            try
            {
                // 21.
                if (listing.Bed2Length != null && listing.Bed2Width != null) //Bedroom 2 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM2_SIZE"), listing.Bed2Length);//Lenght
                    driver.WriteTextbox(By.Name("rightBDRM2_SIZE"), listing.Bed2Width);//Width
                    driver.FindElement(By.Name("rightBDRM2_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    driver.WriteTextbox(By.Name("BDRM2_LEVEL"), listing.Bed2Level, true); // Bedroom 2 level
                }

                // 22.
                if (listing.Bed3Length != null && listing.Bed3Width != null)//Bedroom 3 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM3_SIZE"), listing.Bed3Length);//Lenght
                    driver.WriteTextbox(By.Name("rightBDRM3_SIZE"), listing.Bed3Width);//Width
                    driver.FindElement(By.Name("rightBDRM3_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    driver.WriteTextbox(By.Name("BDRM3_LEVEL"), listing.Bed3Level, true); // Bedroom 3 level
                }

                // 23.
                if (listing.Bed4Length != null && listing.Bed4Width != null)//Bedroom 4 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM4_SIZE"), listing.Bed4Length); //Lenght
                    driver.WriteTextbox(By.Name("rightBDRM4_SIZE"), listing.Bed4Width); //Width
                    driver.FindElement(By.Name("rightBDRM4_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    driver.WriteTextbox(By.Name("BDRM4_LEVEL"), listing.Bed4Level, true); // Bedroom 4 level
                }

                // 24.
                if (listing.Bed5Length != null && listing.Bed5Width != null)//Bedroom 5 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM5_SIZE"), listing.Bed5Length, true); //Lenght
                    driver.WriteTextbox(By.Name("rightBDRM5_SIZE"), listing.Bed5Width, true); //Width
                    driver.FindElement(By.Name("rightBDRM5_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(500);
                    driver.WriteTextbox(By.Name("BDRM45_LEVEL"), listing.Bed5Level, true); //Bedroom 5 Level
                }
            }
            catch { 
                // This fields are optionals 
            }

            // 25.
            if (listing.OtherRoom1Length != null && listing.OtherRoom1Width != null)
            {
                driver.WriteTextbox(By.Name("OTHER_ROOMS"), "OTHR"); // Other Rooms
                driver.FindElement(By.Name("OTHER_ROOMS")).SendKeys(Keys.Tab);

                driver.WriteTextbox(By.Name("A1N"), "GAME");
                driver.FindElement(By.Name("A1N")).SendKeys(Keys.Tab);

                driver.WriteTextbox(By.Name("leftA1S"), listing.OtherRoom1Length); //Lenght
                driver.WriteTextbox(By.Name("rightA1S"), listing.OtherRoom1Width); //Width
                driver.FindElement(By.Name("rightA1S")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("A1L"), listing.OtherRoom1Level); //Other Room 1 Level
                driver.FindElement(By.Name("A1L")).SendKeys(Keys.Tab);
            }

            // 26.
            if (listing.OtherRoom2Length != null && listing.OtherRoom2Width != null)
            {

                driver.WriteTextbox(By.Name("A2N"), "MDIA");
                driver.FindElement(By.Name("A2N")).SendKeys(Keys.Tab);

                driver.WriteTextbox(By.Name("leftA2S"), listing.OtherRoom2Length); //Lenght
                driver.WriteTextbox(By.Name("rightA2S"), listing.OtherRoom2Width); //Width
                driver.FindElement(By.Name("rightA2S")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("A2L"), listing.OtherRoom2Level); //Other Room 1 Level
                driver.FindElement(By.Name("A2L")).SendKeys(Keys.Tab);
            }
        }

        /// <summary>
        /// Fills the information for the Interior Tab
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillInteriorLeasingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('2') ");
            Thread.Sleep(2000);

            // 1.
            driver.WriteTextbox(By.Name("INTERIOR"), listing.InteriorDesc); // Interior
            driver.FindElement(By.Name("INTERIOR")).SendKeys(Keys.Tab);

            // 2. 
            driver.WriteTextbox(By.Name("INCLUSIONS"), listing.InclusionsDesc); // Inclusions
            driver.FindElement(By.Name("INCLUSIONS")).SendKeys(Keys.Tab);

            // 3.
            driver.WriteTextbox(By.Name("SECURITY"), listing.SecurityDesc); // Security
            driver.FindElement(By.Name("INCLUSIONS")).SendKeys(Keys.Tab);

            // 4.
            driver.WriteTextbox(By.Name("FLOORING"), listing.FloorsDesc); // Flooring
            driver.FindElement(By.Name("FLOORING")).SendKeys(Keys.Tab);

            // 5.
            try
            {
                if (!String.IsNullOrWhiteSpace(listing.NumberFireplaces))
                {
                    driver.WriteTextbox(By.Name("FIREPLACE1"), listing.FireplaceDesc); //# Fireplaces
                    driver.FindElement(By.Name("FIREPLACE1")).SendKeys(Keys.Tab);
                }
            }
            catch
            {
                //ShowMarkToFieldByNameElement(driver, "FIREPLACE1");
            }
            try { driver.SwitchTo().Alert().Accept(); } catch { }

            ((IJavaScriptExecutor)driver).ExecuteScript(" closeDiv(); ");

            // 6.

            ////FIXME is not filling in this value.
            //try
            //{
            //    driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Name("FIREPLACE")));
            //    if (!String.IsNullOrWhiteSpace(listing.FireplaceDesc) && !listing.FireplaceDesc.Equals("NA"))
            //    {
            //        driver.WriteTextbox(By.Name("FIREPLACE"), listing.FireplaceDesc); //Fireplace
            //        driver.FindElement(By.Name("FIREPLACE")).SendKeys(Keys.Tab);
            //    }
            //}
            //catch
            //{
            //    //ShowMarkToFieldByNameElement(driver, "FIREPLACE");
            //}
            //try { driver.SwitchTo().Alert().Accept(); } catch { }
            //((IJavaScriptExecutor)driver).ExecuteScript(" closeDiv(); ");

            // 7.
            driver.WriteTextbox(By.Name("WNDWCVRNGS"), listing.WindowCoverings); //Window Coverings
            driver.FindElement(By.Name("WNDWCVRNGS")).SendKeys(Keys.Tab);

            // 8.
            driver.WriteTextbox(By.Name("BEDROOMS"), listing.Beds); //Bedrooms
            ((IJavaScriptExecutor)driver).ExecuteScript(" BEDROOMSActions() ");

            // 9.
            driver.WriteTextbox(By.Name("MASTERBDRM"), listing.Bed1Desc); //Master Bedroom
            driver.FindElement(By.Name("MASTERBDRM")).SendKeys(Keys.Tab);

            // 10.
            if (listing.ClosetLength > 0 && listing.ClosetWidth > 0)
            {
                driver.WriteTextbox(By.Name("leftMBRCLOSET_SIZE"), listing.ClosetLength);
                driver.WriteTextbox(By.Name("rightMBRCLOSET_SIZE"), listing.ClosetWidth);
                driver.FindElement(By.Name("rightMBRCLOSET_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("MBRCLOSET_LEVEL"), listing.Bed1Level, true); // Master Bedroom Level
            }

            // 11.
            driver.WriteTextbox(By.Name("leftADDEDIT_BATHS"), listing.BathsFull); //Bathrooms Full
            driver.WriteTextbox(By.Name("rightADDEDIT_BATHS"), listing.BathsHalf); //Bathrooms Half

            // 10.
            driver.WriteTextbox(By.Name("MASTERBATH"), listing.BedBathDesc); //Master Bath
            ((IJavaScriptExecutor)driver).ExecuteScript(" MASTERBATHActions(); closeDiv(); ");
            driver.FindElement(By.Name("MASTERBATH")).SendKeys(Keys.Tab);
            //selectVals('MASTERBATH'); ; MASTERBATHActions(); closeDiv();

            // 12.
            if (listing.LivingRoom3Length != null && listing.LivingRoom3Width != null)//Entry Room Size
            {
                driver.WriteTextbox(By.Name("leftENTRM_SIZE"), listing.LivingRoom3Length, true);  //Length
                driver.WriteTextbox(By.Name("rightENTRM_SIZE"), listing.LivingRoom3Width, true); //Width
                driver.FindElement(By.Name("rightENTRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("ENTRM_LEVEL"), listing.LivingRoom3Level, true); // Entry Room level
            }

            // 13.
            if (listing.LivingRoom1Length != null && listing.LivingRoom1Width != null) //Living Room Size
            {
                driver.WriteTextbox(By.Name("leftLVRM_SIZE"), listing.LivingRoom1Length, true); //Lenght
                driver.WriteTextbox(By.Name("rightLVRM_SIZE"), listing.LivingRoom1Width, true); //Width
                driver.FindElement(By.Name("rightLVRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("LVRM_LEVEL"), listing.LivingRoom1Level, true); // Living Room level
            }

            // 14.
            if (listing.LivingRoom2Length != null && listing.LivingRoom2Width != null)//Family Room Size
            {
                driver.WriteTextbox(By.Name("leftFAMRM_SIZE"), listing.LivingRoom2Length, true);//Lenght
                driver.WriteTextbox(By.Name("rightFAMRM_SIZE"), listing.LivingRoom2Width, true);//Width
                driver.FindElement(By.Name("rightFAMRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("FAMRM_LEVEL"), listing.LivingRoom2Level, true); // Family Room level
            }

            // 15.
            if (listing.StudyLength != null && listing.StudyWidth != null)//Study/Office Size
            {
                driver.WriteTextbox(By.Name("leftSTYOROF_SIZE"), listing.StudyLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightSTYOROF_SIZE"), listing.StudyWidth, true);//Width
                driver.FindElement(By.Name("rightSTYOROF_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("STYOROF_LEVEL"), listing.StudyLevel, true); // Study/Office level
            }

            // 16.
            if (listing.KitchenLength != null && listing.KitchenWidth != null)//Kitchen Size
            {
                driver.WriteTextbox(By.Name("leftKIT_SIZE"), listing.KitchenLength);//Lenght
                driver.WriteTextbox(By.Name("rightKIT_SIZE"), listing.KitchenWidth);//Width
                driver.FindElement(By.Name("rightKIT_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("KIT_LEVEL"), listing.KitchenLevel, true); // Kitchen Room level
            }

            // 17.
            if (listing.BreakfastLength != null && listing.BreakfastWidth != null)//Breakfast Room Size
            {
                driver.WriteTextbox(By.Name("leftBFSTRM_SIZE"), listing.BreakfastLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightBFSTRM_SIZE"), listing.BreakfastWidth, true);//Width
                driver.FindElement(By.Name("rightBFSTRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("BFSTRM_LEVEL"), listing.BreakfastLevel, true); // Breakfast Room level
            }

            // 18.
            if (listing.DiningRoomLength != null && listing.DiningRoomWidth != null)//Dining Room Size
            {
                driver.WriteTextbox(By.Name("leftDINR_SIZE"), listing.DiningRoomLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightDINR_SIZE"), listing.DiningRoomWidth, true);//Width
                driver.FindElement(By.Name("rightDINR_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("DINR_LEVEL"), listing.DiningRoomLevel, true); // Dining Room level
            }

            // 19.
            if (listing.UtilityRoomLength != null && listing.UtilityRoomWidth != null)//Utility Room Size
            {
                driver.WriteTextbox(By.Name("leftUTLRM_SIZE"), listing.UtilityRoomLength, true);//Lenght
                driver.WriteTextbox(By.Name("rightUTLRM_SIZE"), listing.UtilityRoomWidth, true);//Width
                driver.FindElement(By.Name("rightUTLRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("UTLRM_LEVEL"), listing.UtilityRoomLevel, true); // Utility Room level
            }

            // 20.
            if (listing.Bed1Length != null && listing.Bed1Width != null)//Master Bedroom Size
            {
                driver.WriteTextbox(By.Name("leftMBR_SIZE"), listing.Bed1Length);//Lenght
                driver.WriteTextbox(By.Name("rightMBR_SIZE"), listing.Bed1Width);//Width
                driver.FindElement(By.Name("rightMBR_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("MBR_LEVEL"), listing.Bed1Level, true); // Master Bedroom level
            }

            // 20.
            //if (listing.Bath1Length != null && listing.Bath1Width != null)//Master Bath Size
            //{
            //    driver.WriteTextbox(By.Name("leftMBTH_SIZE"), listing.Bath1Length);//Lenght
            //    driver.WriteTextbox(By.Name("rightMBTH_SIZE"), listing.Bath1Width);//Width
            //    driver.FindElement(By.Name("rightMBTH_SIZE")).SendKeys(Keys.Tab);
            //    Thread.Sleep(500);
            //    driver.WriteTextbox(By.Name("MBTH_LEVEL"), listing.Bath1Level, true); // Master Bedroom level
            //}

            try
            {
                // 21.
                if (listing.Bed2Length != null && listing.Bed2Width != null) //Bedroom 2 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM2_SIZE"), listing.Bed2Length);//Lenght
                    driver.WriteTextbox(By.Name("rightBDRM2_SIZE"), listing.Bed2Width);//Width
                    driver.FindElement(By.Name("rightBDRM2_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Name("BDRM2_LEVEL"), listing.Bed2Level, true); // Bedroom 2 level
                }

                // 22.
                if (listing.Bed3Length != null && listing.Bed3Width != null)//Bedroom 3 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM3_SIZE"), listing.Bed3Length);//Lenght
                    driver.WriteTextbox(By.Name("rightBDRM3_SIZE"), listing.Bed3Width);//Width
                    driver.FindElement(By.Name("rightBDRM3_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Name("BDRM3_LEVEL"), listing.Bed3Level, true); // Bedroom 3 level
                }

                // 23.
                if (listing.Bed4Length != null && listing.Bed4Width != null)//Bedroom 4 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM4_SIZE"), listing.Bed4Length); //Lenght
                    driver.WriteTextbox(By.Name("rightBDRM4_SIZE"), listing.Bed4Width); //Width
                    driver.FindElement(By.Name("rightBDRM4_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Name("BDRM4_LEVEL"), listing.Bed4Level, true); // Bedroom 4 level
                }

                // 24.
                if (listing.Bed5Length != null && listing.Bed5Width != null)//Bedroom 5 Size
                {
                    driver.WriteTextbox(By.Name("leftBDRM5_SIZE"), listing.Bed5Length, true); //Lenght
                    driver.WriteTextbox(By.Name("rightBDRM5_SIZE"), listing.Bed5Width, true); //Width
                    driver.FindElement(By.Name("rightBDRM5_SIZE")).SendKeys(Keys.Tab);
                    Thread.Sleep(1000);
                    driver.WriteTextbox(By.Name("BDRM45_LEVEL"), listing.Bed5Level, true); //Bedroom 5 Level
                }
            }
            catch
            {
                // This fields are optionals 
            }

            // 25.
            driver.WriteTextbox(By.Name("OTRMUSE"), listing.OtherRoomDesc, true);//Other Room Use

            // 26.
            if (listing.OtherRoom1Length != null && listing.OtherRoom1Width != null)//Other Room Size
            {
                driver.WriteTextbox(By.Name("leftOTHRM_SIZE"), listing.OtherRoom1Length, true);//Lenght
                driver.WriteTextbox(By.Name("rightOTHRM_SIZE"), listing.OtherRoom1Width, true);//Width
                driver.FindElement(By.Name("rightOTHRM_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(1000);
                driver.WriteTextbox(By.Name("OTHRM_LEVEL"), listing.OtherRoom1Level, true);//Other Room level
            }

            // 27.
            driver.WriteTextbox(By.Name("OTRM2USE"), listing.OtherRoom2Desc, true);//Other Room2 Use

            // 28.
            if (listing.OtherRoom2Length != null && listing.OtherRoom2Width != null)//Other Room 2 Size
            {
                driver.WriteTextbox(By.Name("leftOTHRM2_SIZE"), listing.OtherRoom2Length, true);//Lenght
                driver.WriteTextbox(By.Name("rightOTHRM2_SIZE"), listing.OtherRoom2Width, true);//Width
                driver.FindElement(By.Name("rightOTHRM2_SIZE")).SendKeys(Keys.Tab);
                Thread.Sleep(500);
                driver.WriteTextbox(By.Name("OTHRM2_LEVEL"), listing.OtherRoom2Level, true);//Other Room 2 level
            }
        }

        /// <summary>
        /// Fills the information for the Utilities tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillUtilitiesInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('3') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("AIRCNDTNNG"), listing.CoolSystemDesc);//Air Conditioning
            driver.FindElement(By.Name("AIRCNDTNNG")).SendKeys(Keys.Tab);
            
            driver.WriteTextbox(By.Name("HEATING"), listing.HeatSystemDesc);//Heating
            driver.FindElement(By.Name("HEATING")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("HEATINGFUL"), listing.HeatingFuel);//Heating Fuel
            driver.FindElement(By.Name("HEATINGFUL")).SendKeys(Keys.Tab);

            // TODO : Remove the below lines
            //if (String.IsNullOrEmpty(listing.WaterDesc))
                listing.WaterDesc = "WTRSY,SWRSY,CITY";
            driver.WriteTextbox(By.Name("WATERSEWER"), listing.WaterDesc);//Water/Sewer
            driver.FindElement(By.Name("WATERSEWER")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("UTSPELEC"), listing.SupElectricity, true);//Utility Supplier: Elec
            driver.WriteTextbox(By.Name("UTSPGAS"), listing.SupGas, true);//Utility Supplier: Gas
            driver.WriteTextbox(By.Name("UTSPWATER"), listing.SupWater, true);//Utility Supplier: Water
            driver.WriteTextbox(By.Name("UTSPSEWER"), listing.SupSewer, true);//Utility Supplier: Sewer
            driver.WriteTextbox(By.Name("UTSPGRBGE"), listing.SupGarbage, true);//Utility Supplier: Grbge
            driver.WriteTextbox(By.Name("UTSPOTHER"), listing.SupOther, true);//Utility Supplier: Other
        }

        /// <summary>
        /// Fills the information for the Tax/Hoa tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillTaxHoaInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('4') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("MTPLCNTY"), "NO"); //Taxed by Mltpl Counties

            driver.WriteTextbox(By.Name("TAX_YEAR"), listing.YearBuilt); //Certified Tax Year

            // TODO : Remove the below line
            //if(String.IsNullOrEmpty(listing.HOA))
                listing.HOA = "NONE";

            driver.WriteTextbox(By.Name("HOAMNDTRY"), listing.HOA); //HOA
            driver.FindElement(By.Name("HOAMNDTRY")).SendKeys(Keys.Tab);
            Thread.Sleep(1000);
            try
            {
                driver.SwitchTo().Alert().Accept();
            } 
            catch { }
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("openPicklist('HOAMNDTRY')");
            ((IJavaScriptExecutor)driver).ExecuteScript("selectVals('HOAMNDTRY'); ; HOAMNDTRYActions(); closeDiv();");
            Thread.Sleep(1000);
            driver.WriteTextbox(By.Name("TOTALTAX"), listing.TaxRate.ToString("0.##")); //Total Tax (Without Exemptions)
            if (listing.HOA.Trim() == "MAND" || listing.HOA.Trim() == "VOLNT")
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("openPicklist('HOAMNDTRY')");

                driver.WriteTextbox(By.Name("MLTPLHOA"), "NO"); //Multiple HOA
                driver.WriteTextbox(By.Name("HOAFEE"), listing.AssocFee); //HOA Fee
                driver.WriteTextbox(By.Name("HOANAME"), listing.AssocName); //HOA Name
                driver.WriteTextbox(By.Name("PYMNTFREQ"), listing.AssocFeePaid); //Payment Frequency
                driver.WriteTextbox(By.Name("ASNTRNFEE"), listing.AssocTransferFee); //Assoc Transfer Fee
            }
        }

        /// <summary>
        /// Fills the information for the Office tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillOfficeInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('5') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("CONTRACT"), "EA");//Contract

            try 
            {
                if (listing.ExpiredDate != null)
                    driver.WriteTextbox(By.Name("EXPDATE"), listing.ExpiredDate.Value.Date.ToString("MM/dd/yyyy"));//Expiration Date            
            } catch { }

            if (driver.UploadInformation.IsNewListing)
            {
                DateTime ListDate = DateTime.Now;
                // TODO : Remove the bnelow line
                listing.ListStatus = "ACT";

                if (listing.ListStatus.ToLower() == "pnd")
                    ListDate = ListDate.AddDays(-2);
                else if (listing.ListStatus.ToLower() == "sld")
                    ListDate = ListDate.AddDays(-HusaMarketConstants.ListDateSold);
                var now = ListDate.ToString("MM/dd/yyyy");
               // var next = ListDate.AddDays(365).ToString("MM/dd/yyyy");

                //driver.WriteTextbox(By.Name("SCHEDULED_ACTIVATION"), "N"); //Scheduled Activation
                driver.WriteTextbox(By.Name("LSTDATE"), now);//List Date
                //driver.WriteTextbox(By.Name("EXPDATE"), listing.ExpiredDate);//Expiration Date
                driver.WriteTextbox(By.Name("EXPDATE"), DateTime.Now.AddYears(1).Date.ToString("MM/dd/yyyy"));//Expiration Date
            }

            driver.WriteTextbox(By.Name("PROPSDTRMS"), listing.PROPSDTRMS); //Proposed Terms
            Thread.Sleep(400);
            driver.WriteTextbox(By.Name("POSSESSION"), "NEGO"); //Possession
            //driver.WriteTextbox(By.Name("PHTOSHOW"), listing.AgentListApptPhone); //Ph to Show
            //driver.WriteTextbox(By.Name("PHTOSHOW"), !String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany) ? listing.AgentListApptPhoneFromCompany : listing.AgentListApptPhone); //Ph to Show
            // BEGIN UP-73
            var apptPhone = String.Empty;
            if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany))
                apptPhone = listing.AgentListApptPhoneFromCompany;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCommunityProfile))
                apptPhone = listing.AgentListApptPhoneFromCommunityProfile;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhone))
                apptPhone = listing.AgentListApptPhone;
            else if (!String.IsNullOrEmpty(listing.OwnerPhone))
                apptPhone = listing.OwnerPhone;

            driver.WriteTextbox(By.Name("PHTOSHOW"), apptPhone); //Ph to Show
            // END BEGIN UP-73
            driver.WriteTextbox(By.Name("SHWCONTCT"), listing.Showing); //Showing Contact
            driver.WriteTextbox(By.Name("LOCKBOX"), "NONE"); //Lockbox Type
            //driver.WriteTextbox(By.Name("LOCKBOX"), listing.LockboxTypeDesc); //Lockbox Type
            driver.WriteTextbox(By.Name("OCCUPANCY"), "VACNT", true); //Occupancy
            driver.WriteTextbox(By.Name("CURRENTLY_LEASED"), "NO", true); //Currently Being Leased
            driver.WriteTextbox(By.Name("OWNER"), listing.OwnerName); //Owner
            driver.WriteTextbox(By.Name("SUBAGTCOM"), "0%"); //Subagent Com

            // TODO : Remove the below line
            //if (String.IsNullOrEmpty(listing.CompBuy))
                listing.CompBuy = "0";
            driver.WriteTextbox(By.Name("BUYAGTCOM"), listing.CompBuy); //Buyer Agent Com

            if(!String.IsNullOrEmpty(listing.AgentBonusAmount))
                driver.WriteTextbox(By.Name("BONUS"), "$" + listing.AgentBonusAmount); // Bonus

            //driver.WriteTextbox(By.Name("BONUS"), listing.CompBuyBonus);
            driver.WriteTextbox(By.Name("LREAORLREB"), "NO");//Owner LREA/LREB
            driver.WriteTextbox(By.Name("PRFTITLECO"), listing.TitleCo); //Preferred Title Company
            
            driver.FindElement(By.Name("PRFTITLECO")).SendKeys(Keys.Tab);
            driver.WriteTextbox(By.Name("PRFTTL_EONAME"), listing.OwnerName); //Company Name

            driver.WriteTextbox(By.Name("POT_SS_YNID"), "NO", true); //Potential Short Sale
        }

        /// <summary>
        /// Fills the information for the Office tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FillOfficeLeasingInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('4') ");
            Thread.Sleep(2000);

            if (listing.ExpiredDate != null)
                driver.WriteTextbox(By.Name("EXPDATE"), listing.ExpiredDate.Value.Date.ToString("MM/dd/yyyy"));//Expiration Date

            try
            {
                driver.WriteTextbox(By.Name("LSTDATE"), listing.isFutureUse == "1" ? listing.estimatedFutureDate.ToString() : DateTime.Now.ToShortDateString()); // List Date
                if (driver.UploadInformation.IsNewListing)
                {
                    driver.WriteTextbox(By.Name("EXPDATE"), DateTime.Now.AddYears(1).Date.ToString("MM/dd/yyyy")); // Expiration Date
                }
            }
            catch { }

            driver.WriteTextbox(By.Name("DATEAVAIL"),!String.IsNullOrEmpty(listing.Date) ? DateTime.Parse(listing.Date).Date.ToShortDateString() : "" ); // Date Available
            driver.WriteTextbox(By.Name("MINNOMNTHS"), listing.MinNumMonths); // Min # of Months
            driver.WriteTextbox(By.Name("MAXNOMNTHS"), listing.MaxNumMonths); // Max # of Months
            driver.WriteTextbox(By.Name("SECRTYDEP"), listing.DepositSecurity); // Security Deposit
            driver.WriteTextbox(By.Name("CLEANDEP"), listing.DepositClean); // Cleaning Deposit
            
            driver.WriteTextbox(By.Name("PETDEP"), listing.DepositPet); // Pet Deposit
            driver.FindElement(By.Name("PETDEP")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("PETDEPREF"), listing.PetDepositRefund); // Pet Deposit Refundable
            driver.FindElement(By.Name("PETDEPREF")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("PETRENT"), listing.PetsAllowed); // Pet Rent
            //driver.WriteTextbox(By.Name("PORT"), listing.); // P/T
            driver.WriteTextbox(By.Name("APPLYAT"), listing.ApplyAt); // Apply At
            driver.WriteTextbox(By.Name("APPLFORM"), listing.ApplForm); // Application Form
            driver.WriteTextbox(By.Name("APPLFEE"), listing.ApplicationFeePay); // Application Fee
            
            driver.WriteTextbox(By.Name("PRSNALCHK"), listing.PersonalChecksAccepted); // Personal Checks Accepted
            driver.FindElement(By.Name("PRSNALCHK")).SendKeys(Keys.Tab);
            
            driver.WriteTextbox(By.Name("CASHACCPT"), listing.CashAccepted); // Cash Accepted
            driver.FindElement(By.Name("CASHACCPT")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("PHTOSHOW"), "NO"); // Ph to Show  ???
            
            driver.WriteTextbox(By.Name("SHWCONTCT"), listing.Showing); // Showing Contact
            driver.FindElement(By.Name("SHWCONTCT")).SendKeys(Keys.Tab);
            
            driver.WriteTextbox(By.Name("LOCKBOX"), "NONE"); //Lockbox Type ???
            driver.FindElement(By.Name("LOCKBOX")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("OCCUPANCY"), listing.Occupancy); //Occupancy
            driver.FindElement(By.Name("OCCUPANCY")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("OWNER"), listing.OwnerName); //Owner
            driver.WriteTextbox(By.Name("SUBAGTCOM"), listing.CommissionLease); //Subagent Com ???
            driver.WriteTextbox(By.Name("BUYAGTCOM"), "0%"); //Buyer Agent Com ???
            //if (!String.IsNullOrEmpty(listing.AgentBonusAmount))
            //    driver.WriteTextbox(By.Name("BONUS"), "$" + listing.AgentBonusAmount); // Bonus ???
            
            driver.WriteTextbox(By.Name("LREAORLREB"), "NO");  // Owner LREA/LREB
            driver.FindElement(By.Name("LREAORLREB")).SendKeys(Keys.Tab);

            driver.WriteTextbox(By.Name("MISCELANES"), listing.MiscellaneousDesc); // Miscellaneous
            driver.FindElement(By.Name("MISCELANES")).SendKeys(Keys.Tab);
        }

        /// <summary>
        /// Fills the information for the Remarks tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        /// <param name="isCompletionUpdate">determines whether the next button needs to be clicked.</param>
        private void FillRemarksInformation(CoreWebDriver driver, ResidentialListingRequest listing, bool isCompletionUpdate = false)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('6') ");
            Thread.Sleep(2000);

            if (!isCompletionUpdate)
            {
                ClickNextButton(driver);

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

                realtorContactEmail = 
                    (!String.IsNullOrWhiteSpace(realtorContactEmail) && 
                    !(bonusMessage + listing.GetPrivateRemarks(true, false)).ToLower().Contains("email contact") &&
                    !(bonusMessage + listing.GetPrivateRemarks(true, false)).ToLower().Contains(realtorContactEmail))? "Email contact: " + realtorContactEmail + ". " : "";

                Thread.Sleep(2000);

                string agentBonusAmount = !string.IsNullOrEmpty(listing.AgentBonusAmount) ? ("$" + listing.AgentBonusAmount + " Agent Bonus, ask Builder for details. ") : "";

                string message = agentBonusAmount + bonusMessage + listing.GetPrivateRemarks(true, false) + realtorContactEmail;

                string incompletedBuiltNote = "";
                if (listing.YearBuiltDesc == "I" 
                    && !message.Contains("Home is under construction. For your safety, call appt number for showings"))
                {
                    incompletedBuiltNote = "Home is under construction. For your safety, call appt number for showings. ";
                }

                driver.WriteTextbox(By.Name("AGTRMRKS"), "");
                driver.WriteTextbox(By.Name("AGTRMRKS"), incompletedBuiltNote + message, true);//Agent Confidential Rmrks
            }

            #region public remarks
            string remark = string.Empty;
            string cName = listing.CompanyName;
            string builtNote = "MLS# " + listing.MLSNum + " - Built by " + cName + " - ";

            if (listing.YearBuiltDesc == "C")
            {
                String dateFormat = "MMM dd";
                int diffDays = DateTime.Now.Subtract((DateTime)listing.BuildCompletionDate).Days;
                if (diffDays > 365)
                    dateFormat = "MMM dd yyyy";

                if (!String.IsNullOrEmpty(listing.RemarksFormatFromCompany) && listing.RemarksFormatFromCompany == "SD")
                {
                    builtNote += "CONST. COMPLETED " + listing.BuildCompletionDate.Value.ToString(dateFormat) + " ~ ";
                }
                else
                {
                    builtNote += "Ready Now! ~ ";
                }
            }
            else
                builtNote += listing.GetCompletionText() + " completion! ~ ";

            if (listing.IncludeRemarks != null && listing.IncludeRemarks == false)
                builtNote = "";

            if (!String.IsNullOrEmpty(listing.PublicRemarks) && listing.PublicRemarks.Contains('~'))
            {
                int tempIndex = 0;
                tempIndex = listing.PublicRemarks.IndexOf('~') + 1;
                string temp = listing.PublicRemarks.Substring(tempIndex).Trim();
                remark = builtNote + temp.RemoveSlash();
            }
            else
                remark = builtNote + listing.PublicRemarks.RemoveSlash();

            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("REMARKS"), remark, true);//Public Remarks
            #endregion
        }

        /// <summary>
        /// Fills the information for the Remarks tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        /// <param name="isCompletionUpdate">determines whether the next button needs to be clicked.</param>
        private void FillRemarksLeasingInformation(CoreWebDriver driver, ResidentialListingRequest listing, bool isCompletionUpdate = false)
        {
            Thread.Sleep(1000);

            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('5') ");
            Thread.Sleep(2000);

            //ClickNextButton(driver);

            #region private remarks
            String realtorContactEmail = String.Empty;
            if (!String.IsNullOrEmpty(listing.EmailRealtorsContact))
                realtorContactEmail = listing.EmailRealtorsContact;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmail))
                realtorContactEmail = listing.RealtorContactEmail;
            else if (!String.IsNullOrEmpty(listing.RealtorContactEmailFromCommunityProfile))
                realtorContactEmail = listing.RealtorContactEmailFromCommunityProfile;
            // END UP-73
            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(listing.GetPrivateRemarks(true, false)).ToLower().Contains("email contact") &&
                !(listing.GetPrivateRemarks(true, false)).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";

            Thread.Sleep(2000);

            string agentBonusAmount = !string.IsNullOrEmpty(listing.AgentBonusAmount) ? ("$" + listing.AgentBonusAmount + " Agent Bonus, ask Builder for details. ") : "";

            driver.WriteTextbox(By.Name("AGTRMRKS"), "");

            string privateRemarks = "For more information or to schedule a showing call " +
                listing.AgentListApptPhone +
                ". LIMITED SERVICE LISTING: verify dimensions & ISD info" +
                realtorContactEmail + ".";

            driver.WriteTextbox(By.Name("AGTRMRKS"), privateRemarks, true);//Agent Confidential Rmrks
            #endregion private remarks

            #region public remarks
            string remark = string.Empty;
            string cName = listing.CompanyName;
            string builtNote = "MLS# " + listing.MLSNum + " - Built by " + cName + " - ";

            if (listing.IncludeRemarks != null && listing.IncludeRemarks == false)
                builtNote = "";

            if (listing.PublicRemarks.Contains('~'))
            {
                int tempIndex = 0;
                tempIndex = listing.PublicRemarks.IndexOf('~') + 1;
                string temp = listing.PublicRemarks.Substring(tempIndex).Trim();
                remark = builtNote + temp.RemoveSlash();
            }
            else
                remark = builtNote + listing.PublicRemarks.RemoveSlash();

            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("REMARKS"), remark, true);//Public Remarks
            #endregion
        }

        /// <summary>
        /// Fills the information for the Media tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        /// <param name="media">All media associated to the listing</param>
        private void FillMedia(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('7') ");
            Thread.Sleep(2000);

            // UP-200
            driver.WriteTextbox(By.Name("RTS"), "NO");//Are any property photos virtually staged?

            driver.FindElement(By.Id("managephotosbutton")).Click();
            Thread.Sleep(1000);
            ProcessImages(driver, media.OfType<ResidentialListingMedia>());
            Thread.Sleep(2500);
            //driver.ExecuteScript("javascript:saveImages();");

            Thread.Sleep(3000);
        }

        /// <summary>
        /// Fills the information for the Media tab.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        /// <param name="media">All media associated to the listing</param>
        private void FillMediaLeasing(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('6') ");
            Thread.Sleep(2000);

            driver.SetSelect(By.Name("INTERNET_LISTING"), "YES"); // Display Listing on Internet
            driver.SetSelect(By.Name("INTERNET_ADDRESS"), "YES"); // Display Address on Internet
            driver.SetSelect(By.Name("RTS"), "YES"); // Are any property photos virtually staged?
            var virtualTour = media.OfType<ResidentialListingVirtualTour>().FirstOrDefault();
            if (virtualTour != null)
                driver.WriteTextbox(By.Id("VIRTTOUR"), virtualTour.VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded

            driver.FindElement(By.Id("managephotosbutton")).Click();
            ProcessImages(driver, media.OfType<ResidentialListingMedia>());
            Thread.Sleep(2500);
            //driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Id("saveLink")));
            ((IJavaScriptExecutor)driver).ExecuteScript(" parent.workspace.saveImages() ");
            //driver.Click(By.Id("saveLink"));

            #region switchTo workspace
            //driver.SwitchTo("workspace", true);
            #endregion

            Thread.Sleep(3000);
        }

        private void FillMediaLot(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('6') ");
            Thread.Sleep(2000);

            driver.SetSelect(By.Name("INTERNET_LISTING"), "YES"); // Display Listing on Internet
            driver.SetSelect(By.Name("INTERNET_ADDRESS"), "YES"); // Display Address on Internet
            var virtualTour = media.OfType<ResidentialListingVirtualTour>().FirstOrDefault();
            if (virtualTour != null)
                driver.WriteTextbox(By.Id("VIRTTOUR"), virtualTour.VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded
            driver.SetSelect(By.Name("RTS"), "NO"); // Are any property photos virtually staged?

            driver.FindElement(By.Id("managephotosbutton")).Click();
            ProcessImages(driver, media.OfType<ResidentialListingMedia>());
            Thread.Sleep(2500);
            //driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Id("saveLink")));
            ((IJavaScriptExecutor)driver).ExecuteScript(" parent.workspace.saveImages() ");
            //driver.Click(By.Id("saveLink"));

            #region switchTo workspace
            //driver.SwitchTo("workspace", true);
            #endregion

            Thread.Sleep(3000);
        }

        /// <summary>
        /// Method used to move from one page to the other.
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        private void ClickNextButton(CoreWebDriver driver)
        {
            var xpath = "//input[@value=\"Next Page >>\"]";

            IWebElement elem = null;

            try
            {
                elem = driver.FindElement(By.XPath(xpath));
            }
            catch
            { }

            if (elem != null && elem.Displayed)
                driver.FindElements(By.XPath(xpath))[1].Click();

            //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id(this.buttonToWait)));
        }

        /// <summary>
        /// Finalizes the insert of the new listing and captures the MlsNum
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="listing">Current listing being processed</param>
        private void FinalizeInsert(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            string mlsNumber = string.Empty;
            ClickNextButton(driver);

            driver.FindElements(By.Name("Save & Continue"))[1].Click();

            driver.FindElements(By.Name("Assign Listing Number"))[1].Click();

            mlsNumber = driver.FindElement(By.Name("dc")).FindElement(By.XPath("//table//td//b")).Text;

            if (mlsNumber != string.Empty)
                listing.MLSNum = mlsNumber;

            driver.FindElements(By.Name("    OK    "))[1].Click();
        }

        /// <summary>
        /// Finalizes the update of a listing in order to reflect the changes on MLS
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        private void FinalizeUpdate(CoreWebDriver driver)
        {
            ClickNextButton(driver);

            driver.FindElements(By.Name("Save Changes"))[1].Click();

            driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Name("Save Listing")));

            driver.FindElements(By.Name("Save Listing"))[1].Click();
        }

        /// <summary>
        /// Uploads all images associated to the listing
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        /// <param name="media">All media associated to the listing</param>
        private void ProcessImages(CoreWebDriver driver, IEnumerable<ResidentialListingMedia> media)
        {

            driver.wait.Until(x => ExpectedConditions.ElementExists(By.Id("photo-browser")));
            
            if (!driver.UploadInformation.IsNewListing)
                DeleteResources(driver);

            Thread.Sleep(1000);

            IEnumerable<ResidentialListingMedia>  mediaFiles = media;

            foreach (var photo in mediaFiles)
            {
                Thread.Sleep(500);
                driver.FindElement(By.Name("files[]")).SendKeys(photo.PathOnDisk);
            }


            bool outLoop = false;
            while (outLoop == false)
            {
                try
                {
                    driver.wait.Until(x => !driver.FindElement(By.ClassName("fileupload-progress")).Displayed);
                    outLoop = true;
                }
                catch (Exception ex)
                {
                    outLoop = false;
                }
            }
        }

        /// <summary>
        /// Method used to remove all images associated to the listing in order to begin the image upload
        /// </summary>
        /// <param name="driver">Webdriver Element for the current upload</param>
        private void DeleteResources(CoreWebDriver driver)
        {
            driver.wait.Until(x => ((IJavaScriptExecutor)x).ExecuteScript("return document.readyState").Equals("complete"));
            //driver.SwitchTo("main");
            //driver.SwitchTo("workspace");
            var photos = driver.FindElement(By.Id("sortable-list")).FindElements(By.TagName("li"));
            if (photos != null && photos.Count > 0)
            {
                foreach (var photo in photos)
                    photo.FindElement(By.ClassName("delete-icon")).Click();
                
                //Check all images have been deleted and delete if needed.
                photos = driver.FindElement(By.Id("sortable-list")).FindElements(By.TagName("li"));
                if (photos != null && photos.Count > 0)
                    foreach (var photo in photos)
                        photo.FindElement(By.ClassName("delete-icon")).Click();

            }
            
            driver.wait.Until(x => driver.FindElement(By.Id("sortable-list")).FindElements(By.TagName("li")).Count == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="listing"></param>
        private void FillCompletionInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            string miscComplete = string.Empty;
            if (listing.YearBuiltDesc.Equals("C") && listing.MiscellaneousDesc != null)
            {
                miscComplete = listing.MiscellaneousDesc.Replace("UNDCN", "");
                var doubleComma = miscComplete.IndexOf(",,");
                if (doubleComma != -1)
                    miscComplete = miscComplete.Replace(",,", "");
            }
            else if (listing.MiscellaneousDesc != null)
            {
                var isUnderConstruction = listing.MiscellaneousDesc.IndexOf("UNDCN");
                if (isUnderConstruction != -1)
                {
                    if (!string.IsNullOrEmpty(listing.MiscellaneousDesc))
                        miscComplete = listing.MiscellaneousDesc + ",UNDCN";
                    else miscComplete = "UNDCN";
                }
                else
                    miscComplete = listing.MiscellaneousDesc;
            }
            else if (listing.YearBuiltDesc.Equals("I"))
                miscComplete = "UNDCN";
            driver.WriteTextbox(By.Name("MISCELANES"), miscComplete, true);//Miscellaneous
        }


        private void FillBasicInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            string xpath = "//input[@onclick=\"ignoreSuggestions();\"]";

            try
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                IWebElement elem = driver.FindElement(By.XPath(xpath));
                
                if (elem != null && elem.Displayed)
                {
                    driver.Click(By.XPath(xpath));
                }
                else
                {
                    Thread.Sleep(1000);
                    var checkboxes =
                        driver.FindElements(By.TagName("input")).Where(x => x.GetAttribute("type") == "checkbox" &&
                                                                            x.GetAttribute("value") == "on");
                    if (checkboxes.Count() > 0)
                    {
                        var ignoreSuggestions = checkboxes.First();
                        if (ignoreSuggestions.GetAttribute("onclick") == "ignoreSuggestions();")
                            ignoreSuggestions.Click();
                    }
                }
            }
            catch
            { }

            if (driver.UploadInformation.IsNewListing)
            {
                try
                {
                    driver.FindElement(By.Id("save-page")).Click();
                } catch
                {
                    driver.wait.Until(ExpectedConditions.ElementExists(By.Id("main")));
                    driver.SwitchTo("main");
                    driver.wait.Until(ExpectedConditions.ElementExists(By.Id("workspace")));
                    driver.SwitchTo("workspace");
                    driver.ScrollDown();
                    driver.FindElement(By.Id("save-page")).Click();
                }
            }
            else
                try {
                    driver.FindElement(By.Id("submit-data")).Click();
                } catch { }
        }

        /// <summary>
        /// Method used for the click event. Also make a wait for the pages has fully loaded.
        /// </summary>
        /// <param name="driverLocal">Webdriver Element for the current upload</param>
        /// <param name="element">Web element that makes the click event</param>
        private void DoClick(CoreWebDriver driverLocal, IWebElement element)
        {
            // 1. click event
            element.Click();

            // 2. wait until page has fully loaded (Asynchronous events are not controlled)
            //driverLocal.wait.Until(x => ((IJavaScriptExecutor)driverLocal).ExecuteScript("return document.readyState").Equals("complete"));
        }


        /// <summary>
        /// This method makes set of values to the Longitude and Latitud fields 
        /// </summary>
        /// <param name="driver"></param>
        private void SetLongitudeAndLatitudeValues(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Re-position Property on the Map")));
            driver.Click(By.LinkText("Re-position Property on the Map"));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("lng")));
            if (String.IsNullOrEmpty(listing.MLSNum))
            {
                driver.WriteTextbox(By.Id("lat"), listing.Latitude); // Latitude
                driver.WriteTextbox(By.Id("lng"), listing.Longitude); // Longitude
            }
        }

        /// <summary>
        /// Draws a mark for the field
        /// </summary>
        /// <param namef="driver"></param>
        /// <param name="elementName"></param>
        private void ShowMarkToFieldByNameElement(CoreWebDriver driver, String elementName)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(" var elem = document.createElement('img'); elem.setAttribute('tooltip', 'Requires attention'); elem.setAttribute('src', 'http://www.fancyicons.com/free-icons/221/modern-anti-malware/png/24/security_warning_24.png');elem.setAttribute('height', '24');elem.setAttribute('width', '24'); document.getElementsByName('" + elementName + "')[0].parentNode.appendChild(elem); document.getElementsByName('" + elementName + "')[0].parentNode.setAttribute('style', 'background-color: #ffe3a3;') ");
        }

        public void DeleteOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(2000);
            bool loopComplete = false;
            while (!loopComplete)
            {
                if (driver.FindElements(By.Name("dc")) != null)
                {
                    var element = driver.FindElements(By.Name("dc")).FirstOrDefault().FindElements(By.TagName("a")).FirstOrDefault(x => x.GetAttribute("href").Contains("delTour("));

                    if (element != null)
                    {
                        element.Click();
                        Thread.Sleep(2000);
                    }
                    else
                        loopComplete = true;
                }
                else
                    loopComplete = true;
            }

            /*if (driver.FindElement(By.LinkText(" Save ")) != null)
                driver.FindElement(By.LinkText("Save")).Click();*/
            ((IJavaScriptExecutor)driver).ExecuteScript("jQuery('.button.Save').click();");

            Thread.Sleep(2000);
        }
        public void AddOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            List<DateTime> date = OH.getNextDate(listing, 4);
            Thread.Sleep(1000);
            // HCS-596
            String openHouseType = "O";

            foreach (var local in date)
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
                driver.SetSelect(By.Id("startTimeHour"), openhousestart[0].Split(':').FirstOrDefault() == "12" ? "0" : openhousestart[0].Split(':').FirstOrDefault());
                driver.SetSelect(By.Id("startTimeMin"), openhousestart[0].Split(':').LastOrDefault() == "00" ? "0" : openhousestart[0].Split(':').LastOrDefault());
                driver.SetSelect(By.Id("startTimeAmPm"), openhousestart[1]);

                // Stop Time
                driver.SetSelect(By.Id("stopTimeHour"), openhouseend[0].Split(':').FirstOrDefault() == "12" ? "0" : openhouseend[0].Split(':').FirstOrDefault());
                driver.SetSelect(By.Id("stopTimeMin"), openhouseend[0].Split(':').LastOrDefault() == "00" ? "0" : openhouseend[0].Split(':').LastOrDefault());
                driver.SetSelect(By.Id("stopTimeAmPm"), openhouseend[1]);
                
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

        }

        #region Leasing

        public UploadResult UpdateLease(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);
            this.buttonToWait = driver.UploadInformation.IsNewListing ? "Save & Continue" : "Save Changes";
            if (driver.UploadInformation.IsNewListing)
            {
                Login(driver, listing);
                DoClick(driver, driver.FindElement(By.Id("newlistingLink")));
                NewLeasingProperty(driver, listing);
            }
            else
            {
                Login(driver, listing);
                Thread.Sleep(1000);
                driver.ScrollDown(500);
                driver.FindElement(By.Id("listTxnsLink")).Click();
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("mylistcontainer")));

                driver.ExecuteScript("jQuery('.select-list-styled:eq(1) > select').attr('id','selectlist');");
                driver.SetSelect(By.Id("selectlist"), listing.ListStatus);
                Thread.Sleep(2000);
                driver.ExecuteScript("jQuery('.dctable-cell > a:contains(\"" + listing.MLSNum + "\")').parent().parent().find('div:eq(26) > a:first').click();");
                Thread.Sleep(1000);
                driver.ExecuteScript("jQuery('.modal-body > .inner-modal-body > div').find('button')[2].click();");
                Thread.Sleep(1000);

                try
                {
                    driver.ExecuteScript("jQuery('#concurrentConsent >.modal-dialog > .modal-content > .modal-footer > button:first').click();");
                }
                catch { }
            }

            #region General
            FillGeneralLeasingInformation(driver, listing);
            #endregion

            #region Exterior
            FillExteriorLeasingInformation(driver, listing);
            #endregion

            #region Interior
            FillInteriorLeasingInformation(driver, listing);
            #endregion

            #region Utilities
            FillUtilitiesInformation(driver, listing);
            #endregion

            #region Office
            FillOfficeLeasingInformation(driver, listing);
            #endregion

            #region Remarks
            FillRemarksLeasingInformation(driver, listing);
            #endregion

            //FIXME still need to complete this method
            if (driver.UploadInformation.IsNewListing)
            {
                #region Media
                FillMediaLeasing(driver, listing, media.Value);
                #endregion
            }

            return UploadResult.Success;
        }

        #endregion Leasing

        #region Lots
        public UploadResult UpdateLot(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);
            this.buttonToWait = driver.UploadInformation.IsNewListing ? "Save & Continue" : "Save Changes";
            if (driver.UploadInformation.IsNewListing)
            {
                Login(driver, listing);
                DoClick(driver, driver.FindElement(By.LinkText("Add New Listing")));
                //DoClick(driver, driver.FindElement(By.LinkText("View/Manage My Listings & Transactions")));
                NewLotProperty(driver, listing);
            }
            else
            {
                Login(driver, listing);
                //driver.FindElement(By.LinkText("Edit Listing Details")).Click();
                DoClick(driver, driver.FindElement(By.LinkText("Edit Listing Details")));
                EditProperty(driver, listing.MLSNum);
            }

            driver.wait.Timeout = new TimeSpan(0, 0, 20);
            #region General
            FillGeneralLotInformation(driver, listing);
            #endregion

            driver.wait.Timeout = new TimeSpan(0, 0, 20);
            #region Exterior
            FillLotLandInformation(driver, listing);
            #endregion

            driver.wait.Timeout = new TimeSpan(0, 0, 20);
            #region Utilities
            FillLotUtilitiesInformation(driver, listing);
            #endregion

            driver.wait.Timeout = new TimeSpan(0, 0, 20);
            #region TaxHoa
            FillTaxHoaLotInformation(driver, listing);
            #endregion

            driver.wait.Timeout = new TimeSpan(0, 0, 20);
            #region Office
            FillOfficeLotInformation(driver, listing);
            #endregion

            driver.wait.Timeout = new TimeSpan(0, 0, 20);
            #region Remarks
            FillRemarksLotInformation(driver, listing);
            #endregion

            driver.wait.Timeout = new TimeSpan(0, 0, 20);
            //FIXME still need to complete this method
            if (driver.UploadInformation.IsNewListing)
            {
                #region Media
                FillMediaLot(driver, listing, media.Value);
                #endregion
            }

            return UploadResult.Success;
        }

        /// <summary>
        /// Begins the process of loading a new lesing into the system
        /// </summary>
        private void NewLotProperty(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.SwitchTo("main");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("workspace")));
            driver.SwitchTo("workspace");

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("category")));
            driver.SetSelect(By.Id("category"), "LA"); // Class

            ((IJavaScriptExecutor)driver).ExecuteScript("javascript: jQuery('#afSelectorstax').prop('checked', '');"); // Auto-populate from Tax data

            ((IJavaScriptExecutor)driver).ExecuteScript("javascript: jQuery('#afSelectorsmanual').prop('checked', 'checked');"); // Manually enter all listing data

            driver.FindElements(By.Id("Next>>"))[1].Click();
            //Second Page of new listing
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("Save & Continue")));
            driver.WriteTextbox(By.Id("AREAID"), listing.MLSArea); // Area
            driver.WriteTextbox(By.Id("FERGUSON"), listing.MapscoMapCoord); // Mapsco Grid
            driver.WriteTextbox(By.Id("LISTPRICE"), listing.ListPrice.ToString()); // List Price
            driver.WriteTextbox(By.Id("ADDRNUMBER"), listing.StreetNum != null ? listing.StreetNum.ToString() : ""); // Street Number
            driver.WriteTextbox(By.Id("ADDRSTREET"), listing.StreetName); // Street Name
            driver.WriteTextbox(By.Id("CITYID"), listing.CityCode); // City
            driver.WriteTextbox(By.Id("STATEID"), listing.State); // State
            driver.WriteTextbox(By.Id("zip5"), listing.Zip); // Zip
            driver.WriteTextbox(By.Id("COUNTYID"), listing.County); // County
            if (listing.TaxID != null)
                driver.WriteTextbox(By.Id("COUNTACTNO"), listing.TaxID);

            driver.WriteTextbox(By.Id("MULTIPLE_CANSID"), "NO"); // Multiple County AcctNos


            if (driver.FindElement(By.Id("AREAID")) != null)
            {
                driver.FindElement(By.Id("Save & Continue")).Click();

                try
                {
                    if (driver.FindElement(By.ClassName("address-suggestion")) != null &&
                        driver.FindElement(By.ClassName("address-suggestion")).Displayed)
                    {

                        ((IJavaScriptExecutor)driver).ExecuteScript("ignoreSuggestions();");
                        driver.FindElement(By.Id("Save & Continue")).Click();
                    }
                }
                catch { }
            }
            FillBasicInformation(driver, listing);
        }

        private void FillGeneralLotInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            #region only for edited homes
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("main")));
            driver.SwitchTo("main");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("workspace")));
            driver.SwitchTo("workspace");
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id(this.buttonToWait)));
            if (!driver.UploadInformation.IsNewListing)
            {
                driver.FindElement(By.Id("editBasicLink")).Click();
                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("submit-data")));
                driver.WriteTextbox(By.Id("AREAID"), listing.MLSArea);
                driver.WriteTextbox(By.Id("FERGUSON"), listing.MapscoMapCoord);
                driver.FindElement(By.Id("submit-data")).Click();
                FillBasicInformation(driver, listing);
            }
            #endregion

            Thread.Sleep(1000);

            TimeSpan currentTimeout = driver.wait.Timeout;

            try
            {
                TimeSpan newTimeout = new TimeSpan(0, 0, 3);
                driver.wait.Timeout = newTimeout;
                driver.wait.Until(ExpectedConditions.ElementExists(By.Name("TYPE")));
            }
            catch
            {
                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("main")));
                driver.SwitchTo("main");
                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("workspace")));
                driver.SwitchTo("workspace");
                driver.wait.Until(ExpectedConditions.ElementExists(By.Name("TYPE")));
            }

            driver.wait.Timeout = currentTimeout;

            driver.SetAttribute(By.Name("TYPE"), "LTACR", "value");//Type
            string direction = listing.Directions;
            if (!String.IsNullOrEmpty(direction))
            {
                direction = direction.RemoveSlash();
                int dirLen = direction.Length;
                if (direction.ElementAt(dirLen - 1) == '.')
                    direction = direction.Remove(dirLen - 1);
                else
                    direction = direction + ".";
            }
            driver.WriteTextbox(By.Name("INSTORDIR"), direction);//Inst/Dir
            driver.WriteTextbox(By.Name("BLOCK"), listing.Block);//Block
            driver.WriteTextbox(By.Name("LGLDSCLOT"), listing.LotNum);//Legal Desc-Lot
            driver.SetAttribute(By.Name("SBDIVISION"), listing.Subdivision, "value");//Subdivision (Legal Name)
            driver.SetAttribute(By.Name("SUBDIVISION_CKA"), listing.Subdivision, "value");//Subdivision (Common Name)
                                                                                          //driver.WriteTextbox(By.Name("CONDONAME"), listing.); // Condo Name
            driver.WriteTextbox(By.Name("LEGALDESC"), listing.Legal); //Legal Description
            driver.WriteTextbox(By.Name("ZONING"), "Residential"); // Zoning
            if (listing.YearBuilt.HasValue)
                driver.WriteTextbox(By.Name("YEAR_BUILT"), listing.YearBuilt.Value); //Year Built
            else
            {
                IWebElement elem = null;
                try
                {
                    elem = driver.FindElement(By.Id("unknownYEAR_BUILTU"));
                }
                catch
                { }

                if (elem != null && !elem.Selected)
                {
                    driver.Click(By.Id("unknownYEAR_BUILTU"));
                }
            }
            driver.WriteTextbox(By.Name("SQFEET"), listing.SqFtTotal, true);//Square Feet
            driver.WriteTextbox(By.Name("SOURCESQFT"), "B", true);//Source SQFT/Acre
            driver.SetAttribute(By.Name("SCHLDIST"), listing.SchoolDistrict.ToUpper(), "value");//School District
            driver.SetAttribute(By.Name("ELEMSCHL"), listing.SchoolName1, "value");//Elementary School
            driver.SetAttribute(By.Name("MIDSCHL"), listing.SchoolName2, "value");//Middle School
            driver.SetAttribute(By.Name("HIGHSCHL"), listing.SchoolName3, "value");//High School
            driver.WriteTextbox(By.Name("RESTRICTNS"), "OTHER", true); // Restrictions
            driver.WriteTextbox(By.Name("DCMNTSVLBL"), listing.Documents, true); // Documents Available
            driver.WriteTextbox(By.Name("IMPROVMNTS"), listing.Development, true); // Improvements
            driver.WriteTextbox(By.Name("MISCELANES"), listing.MiscellaneousDesc, true); // Miscellaneous
            driver.WriteTextbox(By.Name("GREEN_FEAT"), listing.GreenFeatures, true);//Green Features
        }

        private void FillLotLandInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);
            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('1') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("TOTALACRS"), listing.SqFtTotal); // Total Acres
            driver.WriteTextbox(By.Name("LOTSIZE"), listing.LotSize); // Lot Size (Acres)
            driver.WriteTextbox(By.Name("FRONTFEET"), listing.NumberOfLakes); // Front Feet  (bad field mapping)
            driver.WriteTextbox(By.Name("DEPTHFEET"), listing.NumberOfWaterMeters); // Depth Feet  (bad field mapping)
            driver.WriteTextbox(By.Name("SITEARFTRS"), listing.CommonFeatures); // Site/Area Features
            driver.WriteTextbox(By.Name("DESCRIPTIN"), listing.LotDesc); // Description
            driver.WriteTextbox(By.Name("LOCATION"), listing.LotFeatures); // Lot Description
            driver.WriteTextbox(By.Name("FRONTAGE"), listing.WaterfrontFeatures); // Frontage (bad field mapping)
            driver.WriteTextbox(By.Name("TERRAIN"), listing.SoilType); // Terrain
            driver.WriteTextbox(By.Name("TREES"), listing.ParcelNumber); // Trees (bad field mapping)
        }

        private void FillLotUtilitiesInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('2') ");
            Thread.Sleep(2000);

            //driver.WriteTextbox(By.Name("WELLDEPTH"), listing.); // Well Depth 
            driver.WriteTextbox(By.Name("UTILTSVLBL"), listing.Utilities); // Utilities Available
            driver.WriteTextbox(By.Name("UTILITSNST"), listing.UtilitiesOther); // Utilities on Site
            driver.WriteTextbox(By.Name("SEPTCSYSTM"), listing.SewerDesc); // Septic System
            driver.WriteTextbox(By.Name("UTSPELEC"), listing.SupElectricity, true);//Utility Supplier: Elec
            driver.WriteTextbox(By.Name("UTSPGAS"), listing.SupGas, true);//Utility Supplier: Gas
            driver.WriteTextbox(By.Name("UTSPWATER"), listing.SupWater, true);//Utility Supplier: Water
            driver.WriteTextbox(By.Name("UTSPSEWER"), listing.SupSewer, true);//Utility Supplier: Sewer
            driver.WriteTextbox(By.Name("UTSPGRBGE"), listing.SupGarbage, true);//Utility Supplier: Grbge
            driver.WriteTextbox(By.Name("UTSPOTHER"), listing.SupOther, true);//Utility Supplier: Other
        }

        private void FillTaxHoaLotInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('3') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("MTPLCNTY"), listing.KeyboxNumber); // Taxed by Mltpl Counties (bad field mapping)
            driver.WriteTextbox(By.Name("COUNTYTAX"), listing.CountyTax); // County Tax
            driver.WriteTextbox(By.Name("CITYTAX"), listing.CityTAx); // City Tax
            driver.WriteTextbox(By.Name("SCHOOLTAX"), listing.SchoolTax); // School Tax
            driver.WriteTextbox(By.Name("OTHERTAX"), listing.OtherTax); // Other Tax
            driver.WriteTextbox(By.Name("HOAMNDTRY"), listing.HOA); // HOA
            driver.FindElement(By.Name("HOAMNDTRY")).SendKeys(Keys.Tab);
            Thread.Sleep(2000);
            driver.SwitchTo().Alert().Accept();
            Thread.Sleep(500);
            ((IJavaScriptExecutor)driver).ExecuteScript("openPicklist('HOAMNDTRY')");
            ((IJavaScriptExecutor)driver).ExecuteScript("selectVals('HOAMNDTRY'); ; HOAMNDTRYActions(); closeDiv();");
            //driver.WriteTextbox(By.Name("TOTALTAX"), listing.TaxRate); //Total Tax (Without Exemptions)
            if (listing.HOA.Trim() == "MAND" || listing.HOA.Trim() == "VOLNT")
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("openPicklist('HOAMNDTRY')");

                driver.WriteTextbox(By.Name("MLTPLHOA"), "NO"); //Multiple HOA
                driver.WriteTextbox(By.Name("HOAFEE"), listing.AssocFee); //HOA Fee
                driver.WriteTextbox(By.Name("HOANAME"), listing.AssocName); //HOA Name
                driver.WriteTextbox(By.Name("PYMNTFREQ"), listing.AssocFeePaid); //Payment Frequency
                driver.WriteTextbox(By.Name("ASNTRNFEE"), listing.AssocTransferFee); //Assoc Transfer Fee
            }
        }

        private void FillOfficeLotInformation(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            Thread.Sleep(1000);

            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('4') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("CONTRACT"), "EA");//Contract

            if (listing.ExpiredDate != null)
                driver.WriteTextbox(By.Name("EXPDATE"), listing.ExpiredDate.Value.Date.ToString("MM/dd/yyyy"));//Expiration Date

            if (driver.UploadInformation.IsNewListing)
            {
                DateTime ListDate = DateTime.Now;
                if (listing.ListStatus.ToLower() == "pnd")
                    ListDate = ListDate.AddDays(-2);
                else if (listing.ListStatus.ToLower() == "sld")
                    ListDate = ListDate.AddDays(-HusaMarketConstants.ListDateSold);
                var now = ListDate.ToString("MM/dd/yyyy");
                // var next = ListDate.AddDays(365).ToString("MM/dd/yyyy");

                //driver.WriteTextbox(By.Name("SCHEDULED_ACTIVATION"), "N"); //Scheduled Activation
                driver.WriteTextbox(By.Name("LSTDATE"), now);//List Date
                //driver.WriteTextbox(By.Name("EXPDATE"), listing.ExpiredDate);//Expiration Date
                driver.WriteTextbox(By.Name("EXPDATE"), DateTime.Now.AddYears(1).Date.ToString("MM/dd/yyyy"));//Expiration Date
            }

            driver.WriteTextbox(By.Name("PROPSDTRMS"), "CONV,TXVET,CASH,INVOK"); //Proposed Terms
            Thread.Sleep(400);
            //driver.WriteTextbox(By.Name("POSSESSION"), "NEGO"); //Possession
            //driver.WriteTextbox(By.Name("PHTOSHOW"), listing.AgentListApptPhone); //Ph to Show
            //driver.WriteTextbox(By.Name("PHTOSHOW"), !String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany) ? listing.AgentListApptPhoneFromCompany : listing.AgentListApptPhone); //Ph to Show
            // BEGIN UP-73
            var apptPhone = String.Empty;
            if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany))
                apptPhone = listing.AgentListApptPhoneFromCompany;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCommunityProfile))
                apptPhone = listing.AgentListApptPhoneFromCommunityProfile;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhone))
                apptPhone = listing.AgentListApptPhone;
            else if (!String.IsNullOrEmpty(listing.OwnerPhone))
                apptPhone = listing.OwnerPhone;

            driver.WriteTextbox(By.Name("PHTOSHOW"), apptPhone); //Ph to Show
            // END BEGIN UP-73
            driver.WriteTextbox(By.Name("SHWCONTCT"), "OWNER"); //Showing Contact
            driver.WriteTextbox(By.Name("LOCKBOX"), "NONE"); //Lockbox Type
            driver.WriteTextbox(By.Name("OWNER"), listing.OwnerName); //Owner
            driver.WriteTextbox(By.Name("LREAORLREB"), "NO");//Owner LREA/LREB

            driver.WriteTextbox(By.Name("SUBAGTCOM"), "0%"); //Subagent Com
            driver.WriteTextbox(By.Name("BUYAGTCOM"), listing.CompBuy); //Buyer Agent Com

            if (!String.IsNullOrEmpty(listing.AgentBonusAmount))
                driver.WriteTextbox(By.Name("BONUS"), "$" + listing.AgentBonusAmount); // Bonus

            driver.WriteTextbox(By.Name("PRFTTL_EONAME"), listing.OwnerName); //Company Name

            if(!String.IsNullOrEmpty(listing.TitleCo))
                driver.WriteTextbox(By.Name("PRFTITLECO"), listing.TitleCo); //Preferred Title Company
            else
                driver.WriteTextbox(By.Name("PRFTITLECO"), listing.OwnerName); //Preferred Title Company
            driver.ScrollDown();
            driver.WriteTextbox(By.Name("POT_SS_YN"), "NO", true); //Potential Short Sale
        }

        private void FillRemarksLotInformation(CoreWebDriver driver, ResidentialListingRequest listing, bool isCompletionUpdate = false)
        {
            Thread.Sleep(1000);

            ((IJavaScriptExecutor)driver).ExecuteScript(" SP('5') ");
            Thread.Sleep(2000);

            driver.WriteTextbox(By.Name("AGTRMRKS"), "");
            driver.WriteTextbox(By.Name("AGTRMRKS"), listing.AgentPrivateRemarks); // Agent Confidential Rmrks

            #region public remarks
            // driver.WriteTextbox(By.Name("REMARKS"), listing.); // Public Remarks ???
            #endregion
        }

        #endregion
    }
}