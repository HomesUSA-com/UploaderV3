using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;
using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Cookie = System.Net.Cookie;

namespace Husa.Core.Uploaders.Houston
{
    public partial class HoustonUploader : IUploader, IEditor, IPriceUploader, IStatusUploader, IImageUploader
    {
        //WebDriverWait wait;
        private ResidentialListingRequest listing;
        private bool newListing;


        public bool IsFlashRequired { get { return true; } }

        public bool CanUpload(ResidentialListingRequest listing)
        {
            //This method must return true if the listing can be uploaded with this MarketSpecific Uploader
            return listing.MarketName == "HoustonTempo";
        }

        public UploadResult Upload(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            //this.wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
            this.listing = listing;
            this.newListing = (listing.MLSNum == null) ? true : false;

            Login(driver, listing);

            if (this.newListing)
            {
                startInsert(driver);
                newProperty(driver);
            }
            else
            {
                EditListing(driver);
                startupdate(driver);
            }

            MLSInformationSection(driver);
            editOfficeinformation(driver);
            editDescriptionandRoomDimensions(driver);
            editRemarksDirections(driver);
            editInteriorExterior(driver);
            editFinancialInformation(driver);

            return UploadResult.Success;
        }
        public UploadResult Edit(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            //this.wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
            this.listing = listing;
            this.newListing = (listing.MLSNum == null) ? true : false;

            Login(driver, listing);

            if (this.newListing)
            {
                startInsert(driver);
                newProperty(driver);
            }
            else
            {
                EditListing(driver);
                startupdate(driver);
            }

            return UploadResult.Success;
        }
        private void startInsert(CoreWebDriver driver)
        {
            #region newProperty
            // This code is for a new property    

            if (listing.BrokerOffice.ToLower() == "hhom01")
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl05_3")));
                driver.FindElement(By.Id("ctl05_3")).Click();
            }
            /*else if (listing.BrokerOffice.ToLower() == "tmrt01")
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl05_2")));
                driver.FindElement(By.Id("ctl05_2")).Click();
            }*/
            else
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl05_4")));
                driver.FindElement(By.Id("ctl05_4")).Click();
            }
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("MenuControl2")));
            driver.FindElement(By.Id("__TAB_2")).Click();
            driver.SwitchTo("main"); // switch to "main" iframe
            driver.SwitchTo("display"); // switch to "display" iframe

            #endregion
        }

        private void EditListing(CoreWebDriver driver)
        {
            #region navigateMenu
            // connect to add/edit page.
            Thread.Sleep(TimeSpan.FromSeconds(5));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("main")));

            switch (listing.BrokerOffice.ToLower())
            {
                case "hhom01":
                    driver.FindElement(By.Id("ctl05_3")).Click();
                    break;
                /*case "tmrt01":
                    driver.FindElement(By.Id("ctl05_2")).Click();
                    break;*/
                default:
                    driver.FindElement(By.Id("ctl05_4")).Click();
                    break;
            }
  
            Thread.Sleep(TimeSpan.FromSeconds(2));// Add/edit button
            driver.SwitchTo("main"); // switch to "main" iframe
            driver.SwitchTo("display"); // switch to "display" iframe
            driver.wait.Until(ExpectedConditions.ElementExists(By.Name("mlsnumlist")));
            driver.WriteTextbox(By.Name("mlsnumlist"), listing.MLSNum); // fill the MLS number
            driver.FindElement(By.Name("loadprplist2")).Click(); // Link "Show Listings by MLS Number"
            driver.wait.Until(ExpectedConditions.ElementExists(By.Name("loadeditprp")));
            #endregion
        }

        /// <summary>
        /// Login an session in the MLS system
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <returns>The final status of the login operation and whether it succeeded or not</returns>
        public LoginResult Login(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            #region Login
            // Connect to the login page 
            driver.Navigate("http://www.harmls.com/TempoCustom/HAR/Authentication/Login.aspx");
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ContractLogo")));

            // Fill the login and password
            driver.WriteTextbox(By.Id("LoginCtrl_txtLoginUsername"), listing.MarketUsername);
            driver.WriteTextbox(By.Id("LoginCtrl_txtPassword"), listing.MarketPassword);
            driver.Click(By.Id("LoginCtrl_btnLogin"), false);
            Thread.Sleep(100);
            #endregion
            if (listing.BrokerOffice.ToLower() == "hhom01" || listing.BrokerOffice.ToLower() == "mihr01" || listing.BrokerOffice.ToLower() == "tmrt01")
            {
                driver.wait.Until(ExpectedConditions.ElementExists(By.LinkText("Log in Now")));
                driver.Click(By.LinkText("Log in Now"), false);

                //driver.wait.Until(x => ((IJavaScriptExecutor)x).ExecuteScript("return document.readyState").Equals("complete"));
                Thread.Sleep(100);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("member_email")));
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("member_pass")));
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("memberloginformsubmit")));

                driver.WriteTextbox(By.Id("member_email"), listing.MarketUsername);
                driver.WriteTextbox(By.Id("member_pass"), listing.MarketPassword);
                driver.Click(By.Id("memberloginformsubmit"), false);

                // UP-79
                WebDriverWait timeout = driver.wait;
                try
                {
                   
                    WebDriverWait newTimeOut = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
                    driver.wait = newTimeOut;
                    driver.wait.Until(x => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                    driver.wait.Until(ExpectedConditions.ElementExists(By.Id("popclose")));
                    driver.wait.Until(x => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                    driver.Click(By.Id("popclose"));
                    driver.wait.Until(x => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                }
                catch { }
                driver.wait = timeout;

                Thread.Sleep(2000);
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Enter TEMPO MLS")));

                driver.Click(By.LinkText("Enter TEMPO MLS"), false);

                try
                {
                    driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("Proceed to TEMPO MLS →")));
                    driver.Click(By.LinkText("Proceed t1o TEMPO MLS →"), false);
                    
                }
                catch { }

                Thread.Sleep(TimeSpan.FromSeconds(3));
                var window = driver.WindowHandles.FirstOrDefault(c => c == driver.CurrentWindowHandle);
                driver.SwitchTo().Window(window).Close();
                 window = driver.WindowHandles.FirstOrDefault();
                driver.SwitchTo().Window(window);
                Thread.Sleep(100);
            }
            else
            {
                // got to Home page
                driver.Navigate().GoToUrl("http://www.harmls.com/default.aspx?showStartPage=true");
            }

            return LoginResult.Logged;
        }

        private void startupdate(CoreWebDriver driver)
        {
            driver.FindElement(By.Name("loadeditprp")).Click(); // "Edit" button
            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("saveincom")));

        }

        #region Edit Sections

        private void newProperty(CoreWebDriver driver)
        {
            #region newProperty

            //ShowContinueBottonByNameElement(driver, "PropType");
            driver.SetSelect(By.Name("PropType"), listing.PropType); //PropType

            //ShowContinueBottonByNameElement(driver, "County");
            driver.SetSelect(By.Name("County"), listing.County); //County
            driver.WriteTextbox(By.Name("StreetNumDisplay"), listing.StreetNum); //Street Number
            // HY-105
            // UP-75
            /*if (!String.IsNullOrWhiteSpace(listing.StreetSuffixFQ))
                driver.WriteTextbox(By.Name("StreetName"), listing.StreetName + " " + listing.StreetSuffixFQ);
            else*/
                driver.WriteTextbox(By.Name("StreetName"), listing.StreetName); //Street Name
            //UP-53
            //driver.WriteTextbox(By.Name("City"), listing.City); //City
            driver.WriteTextbox(By.Name("City"), listing.CityCodeName); //City
            driver.WriteTextbox(By.Name("ListPrice"), listing.ListPrice); //List Price
            driver.WriteTextbox(By.Name("ZipCode"), listing.Zip, false, true); //Zip Code

            DateTime ListDate = new DateTime();
            ListDate = DateTime.Now;
            if (listing.ListStatus.ToLower() != "act")
            {
                if (listing.ListStatus.ToLower() == "pend" || listing.ListStatus.ToLower() == "psho")
                    ListDate = DateTime.Now.AddDays(-2);
                else if (listing.ListStatus.ToLower() == "closd")
                    ListDate = DateTime.Now.AddDays(-HusaMarketConstants.ListDateSold);

                driver.WriteTextbox(By.Name("ListDate"), ListDate.ToShortDateString());
            }
            else
            {
                ListDate = DateTime.Now;
                driver.WriteTextbox(By.Name("ListDate"), ListDate.ToShortDateString());
            }
            if(String.IsNullOrWhiteSpace(listing.MLSNum))
            {
                //driver.WriteTextbox(By.Name("ExpireDate"), Convert.ToDateTime(listing.ExpiredDate).ToShortDateString(), false, true);
                driver.WriteTextbox(By.Name("ExpireDate"), DateTime.Now.AddYears(1).ToShortDateString(), false, true);
            }
            
            driver.FindElement(By.Name("save1")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("saveincom")));

            #endregion
        }
        private void MLSInformationSection(CoreWebDriver driver)
        {
            #region MLS Information Section
            if (String.IsNullOrEmpty(listing.TaxID))
                driver.WriteTextbox(By.Name("TaxID"), "NA"); //Tax ID #
            else
                driver.WriteTextbox(By.Name("TaxID"), listing.TaxID); //Tax ID #

            driver.SetRadioButton(By.Name("ForLease"), "N"); //Also For Lease
            
            ShowMarkToFieldByNameElement(driver, "Area");
            //driver.SetAttribute(By.Name("Area"), listing.MLSArea, "value"); //Area

            //driver.SetAttribute(By.Name("Area2"), listing.MLSArea, "value"); //Area


            //Thread.Sleep(5000);
            ShowMarkToFieldByNameElement(driver, "Location");
            //driver.SetAttribute(By.Name("Location"), listing.LocationDesc, "value"); //Location
            driver.WriteTextbox(By.Name("MapPage"), listing.MapscoMapPage, true); //Key Map Page and Grid

            driver.SetRadioButton(By.Name("LotValue"), "N"); //Priced at Lot Value Only
            
            if (this.newListing)
                driver.WriteTextbox(By.Name("ListPrice"), listing.ListPrice); //List Price

            ShowMarkToFieldByNameElement(driver, "StreetDir");
            //driver.SetSelect(By.Name("StreetDir"), listing.StreetDir, true); //Street Directional
            
            string streetNum = String.IsNullOrEmpty(listing.StreetNumDisplay) ? Convert.ToString(listing.StreetNum) : Convert.ToString(listing.StreetNumDisplay);
            driver.WriteTextbox(By.Name("StreetNumDisplay"), streetNum); //Street Number
            // HY-105
            // UP-75
            /*if (!String.IsNullOrWhiteSpace(listing.StreetSuffixFQ))
                driver.WriteTextbox(By.Name("StreetName"), listing.StreetName + " " + listing.StreetSuffixFQ);
            else*/
                driver.WriteTextbox(By.Name("StreetName"), listing.StreetName); //Street Name

            // UP-53
            //driver.WriteTextbox(By.Name("City"), listing.City); //City
            driver.WriteTextbox(By.Name("City"), listing.CityCodeName); //City
            driver.WriteTextbox(By.Name("ZipCode"), listing.Zip, false, true); //Zip Code

            Thread.Sleep(2000);

            try
            {
                driver.SwitchTo().Alert().Accept();
            }
            catch { }

            driver.WriteTextbox(By.Name("Legal"), listing.Legal, false, true); //Legal Description
            driver.WriteTextbox(By.Name("Subdivision"), listing.Subdivision); //Subdivision

            Thread.Sleep(2000);

            try
            {
                driver.SwitchTo().Alert().Accept();
            }
            catch { }

            ShowMarkToFieldByNameElement(driver, "Legalsubdivision");
            // HY-429
            if(!String.IsNullOrWhiteSpace(listing.LegalsubdivisionDisp))
            {
                // AvailableFields
                if (listing.LegalsubdivisionDisp.Contains("OTHER"))
                {
                    String Legalsubdivision = "OTHER - " + listing.Zip;
                    ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById('LegalsubdivisionDisp').innerHTML = document.EditForm.Legalsubdivision.value = '"+ Legalsubdivision + "'; ");
                }
                else
                    ((IJavaScriptExecutor)driver).ExecuteScript("document.getElementById('LegalsubdivisionDisp').innerHTML = document.EditForm.Legalsubdivision.value = '" + listing.LegalsubdivisionDisp + "'; ");
            }
            
            driver.WriteTextbox(By.Name("SectionNum"), listing.SectionNum); //Section Number

            if (listing.IsPlannedDevelopment == "1")
                driver.SetRadioButton(By.Name("MasterPlannedCommunityYN"), "Y"); //Master Planned Community
            else
                driver.SetRadioButton(By.Name("MasterPlannedCommunityYN"), "N"); //Master Planned Community

            ShowMarkToFieldByNameElement(driver, "Masterplannedcommunity");
            //driver.SetAttribute(By.Name("Masterplannedcommunity"), listing.PlannedDevelopment, "value", true); //Master Planned Community Name
            //driver.WriteTextbox(By.Name("MarketArea"), listing.MLSSubArea, true); //Geo Market Area TODO:revisar
            //Thread.Sleep(20000);
            driver.WriteTextbox(By.Name("SqFtBldg"), listing.SqFtTotal, true); //Building Square Feet

            //driver.SetSelect(By.Name("SqFtSource"), listing.SqFtSource, true); //SqFt Source
            //ShowMarkToFieldByNameElement(driver, "SqFtSource");
            /*switch (listing.SqFtSource)
            {
                case "APPRS":
                case "APDIS":
                case "BUILD":
                case "SELLR":
                    driver.SetSelect(By.Name("SqFtSource"), listing.SqFtSource, true);
                    break;
            }*/
            // UP-175
            driver.SetSelect(By.Name("SqFtSource"), "BUILD", true);


            driver.WriteTextbox(By.Name("YearBuilt"), listing.YearBuilt, true); //Year Built

            //driver.SetSelect(By.Name("YearBuiltSrc"), "BUILD", true); //Year Built Source
            //ShowMarkToFieldByNameElement(driver, "YearBuiltSrc");
            switch (listing.YearBuiltSrc)
            {
                case "APPRS":
                case "APDIS":
                case "BUILD":
                case "SELLR":
                    driver.SetSelect(By.Name("YearBuiltSrc"), listing.YearBuiltSrc, true);
                    break;
            }

            //driver.SetAttribute(By.Name("Schooldistrict"), listing.SchoolDistrict, "value", true); //School District
            ShowMarkToFieldByNameElement(driver, "Schooldistrict");

            driver.WriteTextbox(By.Name("SchoolElem"), listing.SchoolName1, true); //School - Elementary
            driver.WriteTextbox(By.Name("SchoolJunior"), listing.SchoolName2, true); //School - Middle
            driver.WriteTextbox(By.Name("SchoolHigh"), listing.SchoolName3, true); //School - High
            #endregion

            #region UP-80

            // Property Flooded in Harvey
            /*if (listing.HurricanePropertyFlooded != null && listing.HurricanePropertyFlooded == "Y")
            {
                driver.SetSelect(By.Name("IsPropertyFlooded"), "YES12", true);
            } else if (listing.HurricanePropertyFlooded != null && listing.HurricanePropertyFlooded == "N")
            {
                driver.SetSelect(By.Name("IsPropertyFlooded"), "NO123", true);
            }*/

            // Home Flooded in Harvey
            if (listing.HurricaneHomeFlooded != null && listing.HurricaneHomeFlooded == "Y")
            {
                driver.SetSelect(By.Name("IsHomeFlooded"), "YES12", true);
            }
            else if (listing.HurricaneHomeFlooded != null && listing.HurricaneHomeFlooded == "N")
            {
                driver.SetSelect(By.Name("IsHomeFlooded"), "NO123", true);
            }

            #endregion
        }
        private void editOfficeinformation(CoreWebDriver driver)
        {
            #region Office information
            //driver.WriteTextbox(By.Name("PhoneAlt"), listing.OtherPhone, true); //Alternate Phone
            // BEGIN UP-73
            var alternatePhone = String.Empty;

            // alternatePhone
            if (!String.IsNullOrEmpty(listing.AlternatePhoneFromCompany))
                alternatePhone = listing.AlternatePhoneFromCompany;
            else if (!String.IsNullOrEmpty(listing.OtherPhoneFromCommunityProfile))
                alternatePhone = listing.OtherPhoneFromCommunityProfile;
            else if (!String.IsNullOrEmpty(listing.OtherPhone))
                alternatePhone = listing.OtherPhone;
            else if (!String.IsNullOrEmpty(listing.AltPhoneCommunity))
                alternatePhone = listing.AltPhoneCommunity;

            driver.WriteTextbox(By.Name("PhoneAlt"), alternatePhone, true); //Alternate Phone
            // END UP-73


            //driver.WriteTextbox(By.Name("PhoneApptDesk"), listing.AgentListApptPhone); //Appointment Desk Phone
            //driver.WriteTextbox(By.Name("PhoneApptDesk"), !String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany) ? listing.AgentListApptPhoneFromCompany : listing.AgentListApptPhone); //Appointment Desk Phone
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

            driver.WriteTextbox(By.Name("PhoneApptDesk"), apptPhone); //Appointment Desk Phone
            // END BEGIN UP-73

            if (listing.AgentListApptPhone != null || listing.AgentListApptPhoneFromCompany != null)
            {
                driver.SetSelect(By.Name("PhoneApptDesc"), "OFFIC"); //Appointment Phone Description
                //ShowMarkToFieldByNameElement(driver, "PhoneApptDesc");
            }
            else
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("  document.getElementsByName('PhoneApptDesc')[0].selectedIndex = 0;");   
            }


            #endregion
        }
        private void editDescriptionandRoomDimensions(CoreWebDriver driver)
        {
            #region Description and Room Dimensions

            //ShowMarkToFieldByNameElement(driver, "Style");
            driver.SetMultiSelect(By.Name("Style"), listing.HousingStyleDesc); //Style Description
            driver.WriteTextbox(By.Name("Stories"), listing.NumStories); //Number Of Unit Stories

            driver.SetRadioButton(By.Name("NewConstruction"), "Y"); //New Construction

            driver.SetSelect(By.Name("NewConstructionDesc"), listing.YearBuiltDesc); //New Construction Description
            //ShowMarkToFieldByNameElement(driver, "NewConstructionDesc");

            driver.WriteTextbox(By.Name("CompletionDate"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); //Completed Construction Date
            if (listing.YearBuiltDesc == "NVLIV")
                driver.WriteTextbox(By.Name("CompletedConstructionDate"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); //Completed Construction Date
            else
                driver.WriteTextbox(By.Name("CompletionDate"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); //Apprx Completion Date
            //driver.WriteTextbox(By.Name("BuilderName"), listing.BuilderName, true); //Builder Name
            // UP-56
            if(!String.IsNullOrEmpty(listing.BuilderName))
                driver.WriteTextbox(By.Name("BuilderName"), (listing.BuilderName.Equals("T Select by Toll Brothers") ? "T Select" : listing.BuilderName), true); //Builder Name
            driver.WriteTextbox(By.Name("Beds"), listing.Beds); //Bedrooms - Number
            driver.WriteTextbox(By.Name("BathsFull"), listing.BathsFull); //Bathrooms - Full
            driver.WriteTextbox(By.Name("BathsHalf"), listing.BathsHalf); //Bathrooms - Half

            driver.SetMultiSelect(By.Name("PropertyType"), listing.PropSubType); //Property Type
            //ShowMarkToFieldByNameElement(driver, "PropertyType");

            driver.WriteTextbox(By.Name("LotSize"), listing.LotSize, true); //Lot Size

            driver.SetSelect(By.Name("LotSizeSrc"), listing.LotSizeSrc, true); //Lot Size Source
            //ShowMarkToFieldByNameElement(driver, "LotSizeSrc");

            driver.WriteTextbox(By.Name("LotDim"), listing.LotDim, true); //Lot Dimensions

            //driver.SetSelect(By.Name("AcresDesc"), listing.Acres, true); //Acreage
            //ShowMarkToFieldByNameElement(driver, "AcresDesc");

            //List<String> putOptionsAcres = new List<String>();
            //List<String> optionsAcres = String.IsNullOrWhiteSpace(listing.Acres) ? new List<String>() : listing.Acres.Split(',').ToList();
            //foreach (string option in optionsAcres)
            //{
            //    switch (option)
            //    {
            //        case "00TO25":
            //            putOptionsAcres.Add("0-1/4");
            //            break;
            //        case "10TO15":
            //            putOptionsAcres.Add("10-15");
            //            break;
            //        case "15TO20":
            //            putOptionsAcres.Add("15-20");
            //            break;
            //        case "1TO2":
            //            putOptionsAcres.Add("1-2AC");
            //            break;
            //        case "1TO3":
            //            putOptionsAcres.Add("1-2AC");
            //            break;
            //        case "20TO50":
            //            putOptionsAcres.Add("20-50");
            //            break;
            //        case "25TO50":
            //            putOptionsAcres.Add("1/4-2");
            //            break;
            //        case "2TO5":
            //            putOptionsAcres.Add("2-5AC");
            //            break;
            //        case "3TO5":
            //            putOptionsAcres.Add("2-5AC");
            //            break;
            //        case "50TO100":
            //            putOptionsAcres.Add("1/2-1");
            //            break;
            //        case "50TO300":
            //            putOptionsAcres.Add("1/2-1");
            //            break;
            //        case "5TO10":
            //            putOptionsAcres.Add("5-10A");
            //            break;
            //        case "75TO100":
            //            putOptionsAcres.Add("1/2-1");
            //            break;
            //        case "OVER50":
            //            putOptionsAcres.Add("50+AC");
            //            break;
            //    }
            //}

            //if(putOptionsAcres.Count > 0)
            //{
            //    unSelectAllOptions(driver, "AcresDesc");
            //    driver.SetSelect(By.Name("AcresDesc"), String.Join(",", putOptionsAcres), true);
            //}
            
            driver.SetMultiSelect(By.Name("GarageDesc"), listing.GarageDesc, true); //Garage Description  
            //ShowMarkToFieldByNameElement(driver, "GarageDesc");

            driver.WriteTextbox(By.Name("GarageCap"), listing.GarageCapacity, true); //Garage - Number of Spaces
            driver.WriteTextbox(By.Name("RoomLivingDim"), listing.LivingRoom1Dim, true); //Living Room Dimensions
            driver.WriteTextbox(By.Name("RoomDenDim"), listing.LivingRoom2Dim, true); //Den Dimensions
            driver.WriteTextbox(By.Name("RoomGameDim"), listing.OtherRoom2Dim, true); //Gameroom Dimensions
            driver.WriteTextbox(By.Name("RoomDiningDim"), listing.DiningRoomDim, true); //Dining Room Dimensions
            driver.WriteTextbox(By.Name("RoomKitchenDim"), listing.KitchenDim, true); //Kitchen Dimensions
            driver.WriteTextbox(By.Name("RoomBreakfastDim"), listing.BreakfastDim, true); //Breakfast Room Dimensions
            driver.WriteTextbox(By.Name("Bed1Dim"), listing.Bed1Dim, true); //1st Bedroom Dimension
            driver.WriteTextbox(By.Name("Bed2Dim"), listing.Bed2Dim, true); //2st Bedroom Dimension
            driver.WriteTextbox(By.Name("Bed3Dim"), listing.Bed3Dim, true); //3st Bedroom Dimension
            driver.WriteTextbox(By.Name("Bed4Dim"), listing.Bed4Dim, true); //4st Bedroom Dimension
            driver.WriteTextbox(By.Name("Bed5Dim"), listing.Bed5Dim, true); //5st Bedroom Dimension
            driver.WriteTextbox(By.Name("RoomStudyDim"), listing.StudyDim, true); //Study Dimensions
            driver.WriteTextbox(By.Name("RoomOtherDim"), listing.OtherRoom1Dim, true); //Extra Room Dimensions
            driver.WriteTextbox(By.Name("RoomUtilityDim"), listing.UtilityRoomDim, true); //Utility Room Dimensions
            driver.WriteTextbox(By.Name("RoomMediaDim"), listing.MediaRoomDim, true); //Media Room Dimensions

            //driver.SetSelect(By.Name("CarportDesc"), listing.CarportDesc, true); //Carport Description
            //ShowMarkToFieldByNameElement(driver, "CarportDesc");

            //driver.SetMultiSelect(By.Name("GarageCarport"), listing.GarageCarportDesc, true); //Garage/Carport Desc
            //ShowMarkToFieldByNameElement(driver, "GarageCarport");

            driver.SetMultiSelect(By.Name("Access"), listing.AccessInstructionsDesc, true); //Access
            //ShowMarkToFieldByNameElement(driver, "Access");

            //driver.SetMultiSelect(By.Name("FrontDoorFaces"), listing.FacesDesc, true); //Front Door Faces
            //ShowMarkToFieldByNameElement(driver, "FrontDoorFaces");

            List<String> putOptionsFacesDesc = new List<String>();
            List<String> optionsFacesDesc = String.IsNullOrWhiteSpace(listing.FacesDesc) ? new List<string>() : listing.FacesDesc.Split(',').ToList();
            foreach (string option in optionsFacesDesc)
            {
                switch (option)
                {
                    case "EAST":
                        putOptionsFacesDesc.Add("E");
                        break;
                    case "NORTH":
                        putOptionsFacesDesc.Add("N");
                        break;
                    case "SOUTH":
                        putOptionsFacesDesc.Add("S");
                        break;
                    case "WEST":
                        putOptionsFacesDesc.Add("W");
                        break;
                }
            }

            if(putOptionsFacesDesc.Count > 0)
            {
                unSelectAllOptions(driver, "FrontDoorFaces");
                driver.SetMultiSelect(By.Name("FrontDoorFaces"), String.Join(",", putOptionsFacesDesc), true);
            }
            
            #endregion
        }
        private void editRemarksDirections(CoreWebDriver driver)
        {
            #region Remarks/Directions

            String bonusMessage = "";
            if (listing.BonusCheckBox.Equals(true) && listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Bonus & Buyer Incentives; ask Builder for details.";
            else if (listing.BonusCheckBox.Equals(true))
                bonusMessage = "Possible Bonus; ask Builder for details. ";
            else if (listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Buyer Incentives; ask Builder for details. ";
            else
                bonusMessage = "";

            // BEGIN UP-73
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
            driver.WriteTextbox(By.Name("RealRemarks"), bonusMessage + listing.GetPrivateRemarks() + realtorContactEmail, true);//Private Agent Remarks

            List<String> putShowInstr = new List<String>();
            List<String> optionsShowInstr = String.IsNullOrWhiteSpace(listing.ShowingInstructions) ? new List<string>() : listing.ShowingInstructions.Split(',').ToList();
            foreach (string option in optionsShowInstr)
            {
                switch (option)
                {
                    case "ACCMP":
                    case "APPNT":
                    case "BSNCD":
                    case "CNTRL":
                    case "LBFRT":
                    case "LBRGT":
                    case "NOAPT":
                    case "NOSHO":
                    case "NOSPR":
                        putShowInstr.Add(option);
                        break;
                    case "CLOCC":
                    case "LBBCK":
                    case "LBLFT":
                    case "SUPRA":
                        break;
                }
            }

            if (putShowInstr.Count > 0)
            {
                unSelectAllOptions(driver, "ShowInstr");
                driver.SetMultiSelect(By.Name("ShowInstr"), String.Join(",", putShowInstr), true);
            }
            
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
            driver.WriteTextbox(By.Name("Directions"), direction, true); //Directions to Property
            string builtNote = String.Empty;
            string remark = String.Empty;

            builtNote = "MLS# " + listing.MLSNum + " - ";

            if (listing.YearBuiltDesc == "NVLIV")
            {
                //builtNote += "Built by " + listing.CompanyName + " - Ready now! ~ ";
                String dateFormat = "MMM dd";
                int diffDays = DateTime.Now.Subtract((DateTime)listing.BuildCompletionDate).Days;
                if (diffDays > 365)
                    dateFormat = "MMM dd yyyy";
                builtNote += "Built by " + (listing.CompanyName.Equals("T Select by Toll Brothers") ? "T Select" : listing.CompanyName);
                if (!String.IsNullOrEmpty(listing.RemarksFormatFromCompany) && listing.RemarksFormatFromCompany == "SD")
                {
                    // UP-114 (code commented)
                    /*if(listing.RemoveCompletionDate)
                        builtNote += ". NEW HOME ~ ";
                    else*/
                        builtNote += ". CONST. COMPLETED " + listing.BuildCompletionDate.Value.ToString(dateFormat) + " ~ ";
                }
                else
                {
                    builtNote += ". Ready Now! ~ ";
                }
            }
            else if (listing.YearBuiltDesc == "BEBLT")
            {

                if (listing.BuildCompletionDate != null)
                    builtNote += "Built by " + (listing.CompanyName.Equals("T Select by Toll Brothers") ? "T Select" : listing.CompanyName) + " - " + listing.BuildCompletionDate.Value.ToString("MMMM") + " completion! ~ ";

            }

            if (listing.IncludeRemarks != null && listing.IncludeRemarks == false)
                builtNote = "";

            if (!listing.PublicRemarks.Contains('~'))
            {
                remark = (builtNote + listing.PublicRemarks).RemoveSlash();
            }
            else
            {
                var tempIndex = listing.PublicRemarks.IndexOf("~", StringComparison.Ordinal) + 1;
                var temp = listing.PublicRemarks.Substring(tempIndex).Trim();
                remark = (builtNote + temp).RemoveSlash();
            }
             
            driver.WriteTextbox(By.Name("Remarks"), remark, true); //Physical Property Desc - Public
            #endregion
        }
        private void editInteriorExterior(CoreWebDriver driver)
        {
            #region Interior, Exterior, Utilities and Addl Info
            if(listing.HasMicrowave == "1")
                driver.SetRadioButton(By.Name("Microwave"), "Y", true); //Microwave
            else if (listing.HasMicrowave == "0")
                driver.SetRadioButton(By.Name("Microwave"), "N", true); //Microwave
            else
                driver.SetRadioButton(By.Name("Microwave"), "", true); //Microwave

            if (listing.HasDishwasher == "1")
                driver.SetRadioButton(By.Name("Dishwasher"), "Y", true); //Dishwasher
            else if (listing.HasDishwasher == "0")
                driver.SetRadioButton(By.Name("Dishwasher"), "Y", true); //Dishwasher
            else
                driver.SetRadioButton(By.Name("Dishwasher"), "", true); //Dishwasher
            
            if (listing.HasCompactor == "1")
                driver.SetRadioButton(By.Name("Compactor"), "Y", true); //Compactor
            else if (listing.HasCompactor == "0")
                driver.SetRadioButton(By.Name("Compactor"), "N", true); //Compactor
            else
                driver.SetRadioButton(By.Name("Compactor"), "", true); //Compactor

            if (listing.HasDisposal == "1")
                driver.SetRadioButton(By.Name("Disposal"), "Y", true);  //Disposal
            else if (listing.HasDisposal == "0")
                driver.SetRadioButton(By.Name("Disposal"), "N", true);  //Disposal
            else
                driver.SetRadioButton(By.Name("Disposal"), "", true);  //Disposal

            if (listing.HasIcemaker == "1")
                driver.SetRadioButton(By.Name("Icemaker"), "Y", true); //Separate Ice Maker
            else if (listing.HasIcemaker == "0")
                driver.SetRadioButton(By.Name("Icemaker"), "N", true);  //Separate Ice Maker
            else
                driver.SetRadioButton(By.Name("Icemaker"), "", true);  //Separate Ice Maker

            driver.SetMultiSelect(By.Name("OvenType"), listing.OvenDesc, true);  //Oven Type
            //ShowMarkToFieldByNameElement(driver, "OvenType");

            driver.SetMultiSelect(By.Name("RangeType"), listing.RangeDesc, true); //Stove Type
            //ShowMarkToFieldByNameElement(driver, "RangeType");

            if(listing.NumFireplaces != null)
                driver.WriteTextbox(By.Name("Fireplaces"), listing.NumFireplaces, true); //Fireplaces (Number of)

            driver.SetMultiSelect(By.Name("FireplaceDesc"), listing.FireplaceDesc, true); //Fireplace Description
            //ShowMarkToFieldByNameElement(driver, "FireplaceDesc");

            //driver.SetSelect(By.Name("RoomUtilityDesc"), listing.UtilityRoomDesc, true); //Utility Room Description
            //ShowMarkToFieldByNameElement(driver, "RoomUtilityDesc");
            List<String> putOptionsUtilityRoomDesc = new List<String>();
            List<String> optionsUtilityRoomDesc = String.IsNullOrWhiteSpace(listing.UtilityRoomDesc) ? new List<string>() : listing.UtilityRoomDesc.Split( ',' ).ToList();
            foreach (string option in optionsUtilityRoomDesc)
            {
                switch (option)
                {
                    case "INGAR":
                        putOptionsUtilityRoomDesc.Add("GARGE");
                        break;
                    case "INHOU":
                        putOptionsUtilityRoomDesc.Add("HOUSE");
                        break;
                    case "FIRST":
                        putOptionsUtilityRoomDesc.Add("UTIL1");
                        break;
                    case "SECOND":
                        putOptionsUtilityRoomDesc.Add("UTIL2");
                        break;
                    case "THIRD":
                        putOptionsUtilityRoomDesc.Add("UTIL3");
                        break;
                }
            }

            if(putOptionsUtilityRoomDesc.Count > 0)
            {
                //unSelectAllOptions(driver, "RoomUtilityDesc");
                driver.SetMultiSelect(By.Name("RoomUtilityDesc"), String.Join(",", putOptionsUtilityRoomDesc), true);
            }

            driver.SetMultiSelect(By.Name("Connections"), listing.WDConnections, true); //Washer Dryer Connections
            //ShowMarkToFieldByNameElement(driver, "Connections");

            driver.SetMultiSelect(By.Name("GreenCertification"), listing.GreenCerts, true); //Green/Energy Certifications
            //ShowMarkToFieldByNameElement(driver, "GreenCertification");

            driver.SetMultiSelect(By.Name("Energy"), listing.EnergyDesc, true); //Energy Features
            //ShowMarkToFieldByNameElement(driver, "Energy");

            //driver.SetMultiSelect(By.Name("RoomDesc"), listing.OtherRoomDesc, true); //Room description
            //ShowMarkToFieldByNameElement(driver, "RoomDesc");

            List<String> putOptionsOtherRoomDesc = new List<String>();
            List<String> optionsOtherRoomDesc = String.IsNullOrWhiteSpace(listing.OtherRoomDesc) ? new List<String>() : listing.OtherRoomDesc.Split( ',' ).ToList();
            foreach (string option in optionsOtherRoomDesc)
            {
                switch (option)
                {
                    case "BRKFS":
                    case "DEN__":
                    case "FAMRM":
                    case "FRMDN":
                    case "FRMLV":
                    case "GMEDW":
                    case "GMEUP":
                    case "GRAPT":
                    case "GSTSU":
                    case "GSTWK":
                    case "KTDIN":
                    case "LIVNG":
                    case "LOFT_":
                    case "LVDIN":
                    case "LVG2F":
                    case "MEDIA":
                    case "QRTRS":
                    case "STUDY":
                    case "SUNRM":
                    case "WINRM":
                        putOptionsOtherRoomDesc.Add(option);
                        break;
                    case "LVG1F":
                    case "LVG3F":
                        putOptionsOtherRoomDesc.Add("LIVNG");
                        break;
                }
            }
            
            if (putOptionsOtherRoomDesc.Count > 0)
            {
                //unSelectAllOptions(driver, "RoomDesc");
                driver.SetMultiSelect(By.Name("RoomDesc"), String.Join(",", putOptionsOtherRoomDesc), true);
            }

            driver.SetMultiSelect(By.Name("RoomBedDesc"), listing.Bed1Desc, true); //Bedroom Description
            //ShowMarkToFieldByNameElement(driver, "RoomBedDesc");

            //driver.SetMultiSelect(By.Name("BathMasterDesc"), listing.BedBathDesc, true); //Master Bath Description
            //ShowMarkToFieldByNameElement(driver, "BathMasterDesc");
            List<string> putOptionsBedBathDesc = new List<String>();
            List<string> optionsBedBathDesc = String.IsNullOrWhiteSpace(listing.BedBathDesc) ? new List<string>() : listing.BedBathDesc.Split( ',' ).ToList();
            foreach (string option in optionsBedBathDesc)
            {

                switch (option)
                {
                    case "BIDET":
                    case "NOMST":
                        putOptionsBedBathDesc.Add(option);
                        break;
                    case "DBSNK":
                        putOptionsBedBathDesc.Add("DBLSN");
                        break;
                    case "HLFBT":
                        putOptionsBedBathDesc.Add("HALFB");
                        break;
                    case "MSSHW":
                        putOptionsBedBathDesc.Add("SEPSH");
                        break;
                    case "MSHWO":
                        putOptionsBedBathDesc.Add("SHOWR");
                        break;
                    case "MSTUB":
                    case "TUBWS":
                        putOptionsBedBathDesc.Add("TUBSH");
                        break;
                    case "TWOMB":
                        putOptionsBedBathDesc.Add("2MSTR");
                        break;
                    case "WHITU":
                        putOptionsBedBathDesc.Add("WHIRL");
                        break;
                }
            }

            if(putOptionsBedBathDesc.Count > 0)
            {
                unSelectAllOptions(driver, "BathMasterDesc");
                driver.SetMultiSelect(By.Name("BathMasterDesc"), String.Join(",", putOptionsBedBathDesc), true);
            }

            //driver.SetMultiSelect(By.Name("Interior"), listing.InteriorDesc, true); //Interior Features
            //ShowMarkToFieldByNameElement(driver, "Interior");
            List<String> putOptionsInteriorDesc = new List<String>();
            List<String> optionsInteriorDesc = String.IsNullOrWhiteSpace(listing.InteriorDesc) ? new List<string>() : listing.InteriorDesc.Split(',').ToList();
            foreach (string option in optionsInteriorDesc)
            {
                switch (option)
                {
                    case "ALOWN":
                    case "ATRUM":
                    case "BKFST":
                    case "CNTRY":
                    case "DRAPE":
                    case "DRYBR":
                    case "DRYER":
                    case "ELVTR":
                    case "ESHFT":
                    case "HICEL":
                    case "HLYWD":
                    case "HNDCP":
                    case "HOTUB":
                    case "INTCM":
                    case "ISLND":
                    case "PREWR":
                    case "REFRG":
                    case "SMOKE":
                    case "VACUM":
                    case "WASHR":
                    case "WTBAR":
                        putOptionsInteriorDesc.Add(option);
                        break;
                }
            }

            if (putOptionsInteriorDesc.Count > 0)
            {
                unSelectAllOptions(driver, "Interior");
                driver.SetMultiSelect(By.Name("Interior"), String.Join(",", putOptionsInteriorDesc), true);
            }

            driver.SetMultiSelect(By.Name("Floors"), listing.FloorsDesc, true); //Flooring
            //ShowMarkToFieldByNameElement(driver, "Floors");

            driver.WriteTextbox(By.Name("Countertops"), listing.CountertopsDesc, true); //Countertops

            driver.SetMultiSelect(By.Name("Siding"), listing.ExteriorFeaturesDesc); //Exterior Construction
            //ShowMarkToFieldByNameElement(driver, "Siding");

            //driver.SetMultiSelect(By.Name("Exterior"), listing.ExteriorDesc, true); //Exterior Description
            //ShowMarkToFieldByNameElement(driver, "Exterior");
            List<String> putOptionsExteriorDesc = new List<String>();
            List<String> optionsExteriorDesc = String.IsNullOrWhiteSpace(listing.ExteriorDesc) ? new List<string>() : listing.ExteriorDesc.Split(',').ToList();
            foreach(string option in optionsExteriorDesc)
            {
                switch (option)
                {
                    case "AIRHG":
                    case "BARNS":
                    case "BCKYD":
                    case "BKSPC":
                    case "BYFNC":
                    case "COVPT":
                    case "CRSFN":
                    case "DGAQT":
                    case "FULFN":
                    case "GRNHS":
                    case "HOTUB":
                    case "NTFCD":
                    case "ODFPL":
                    case "ODKIT":
                    case "PATIO":
                    case "PORCH":
                    case "PRTFN":
                    case "PRVTN":
                    case "RTDCK":
                    case "SATLT":
                    case "SCPOR":
                    case "SPRNK":
                    case "STORG":
                    case "STSHT":
                    case "SUBAC":
                    case "SUBTN":
                    case "WHEEL":
                    case "WORKS":
                        putOptionsExteriorDesc.Add(option);
                        break;
                }
            }

            if(putOptionsExteriorDesc.Count > 0)
            {
                unSelectAllOptions(driver, "Exterior");
                driver.SetMultiSelect(By.Name("Exterior"), String.Join(",", putOptionsExteriorDesc), true);
            }
            
            driver.SetMultiSelect(By.Name("Roof"), listing.RoofDesc); //Roof Description
            //ShowMarkToFieldByNameElement(driver, "Roof");

            //driver.SetMultiSelect(By.Name("Foundation"), listing.FoundationDesc); //Foundation Description
            // ShowMarkToFieldByNameElement(driver, "Foundation");

            List<String> putOptionsFoundationDesc = new List<String>();
            List<String> optionsFoundationDesc = String.IsNullOrWhiteSpace(listing.FoundationDesc) ? new List<string>() : listing.FoundationDesc.Split(',').ToList();
            foreach (string option in optionsFoundationDesc)
            {
                switch (option)
                {
                    case "BLDER":
                    case "BLOCK":
                    case "PIERB":
                    case "SLAB_":
                    case "STILT":
                        putOptionsFoundationDesc.Add(option);
                        break;
                    case "NOTAP":
                    case "OTHER":
                        putOptionsFoundationDesc.Add("OTHER");
                        break;
                }
            }

            if(putOptionsFoundationDesc.Count > 0)
            {
                unSelectAllOptions(driver, "Foundation");
                driver.SetMultiSelect(By.Name("Foundation"), String.Join(",", putOptionsFoundationDesc));
            }

            if (listing.HasPool == "1")
                driver.SetRadioButton(By.Name("PoolPrivate"), "Y"); //Pool - Private
            else if(listing.HasPool == "0")
                driver.SetRadioButton(By.Name("PoolPrivate"), "N"); //Pool - Private
            else
                driver.SetRadioButton(By.Name("PoolPrivate"), ""); //Pool - Private

            if(listing.HasCommunityPool == "1")
                driver.SetRadioButton(By.Id("PoolArea"), "Y"); // Pool - Area
            else if(listing.HasCommunityPool == "0")
                driver.SetRadioButton(By.Id("PoolArea"), "N"); // Pool - Area
            else
                driver.SetRadioButton(By.Id("PoolArea"), ""); // Pool - Area

            //driver.SetAttribute(By.Name("PoolArea"), listing.HasCommunityPool, "value", true); //Pool - Area
            var select = new SelectElement(driver.FindElement(By.Name("WaterAmenity")));
            select.DeselectAll();

            //driver.SetMultiSelect(By.Name("LotDesc"), listing.LotDesc); //Lot Description
            //ShowMarkToFieldByNameElement(driver, "LotDesc");

            List<String> putOptionsLotDesc = new List<String>();
            List<String> optionsLotDesc = String.IsNullOrWhiteSpace(listing.LotDesc) ? new List<string>() : listing.LotDesc.Split( ',' ).ToList();
            foreach (string option in optionsLotDesc)
            {
                switch (option)
                {
                    case "AIRPK":
                    case "CLEAR":
                    case "CORNR":
                    case "CULDS":
                    case "GREEN":
                    case "INGLF":
                    case "ONGLF":
                    case "PATLT":
                    case "RAVN":
                    case "WTRFT":
                        putOptionsLotDesc.Add(option);
                        break;
                    case "COURT":
                    case "OTHER":
                        putOptionsLotDesc.Add("OTHER");
                        break;
                    case "CANSB":
                        putOptionsLotDesc.Add("SUBDV");
                        break;
                    case "PRTWD":
                    case "WOODD":
                        putOptionsLotDesc.Add("WOODD");
                        break;
                    case "SUBDI":
                    case "SUBDV":
                        putOptionsLotDesc.Add("SUBDV");
                        break;
                }
            }

            if(putOptionsLotDesc.Count > 0)
            {
                unSelectAllOptions(driver, "LotDesc");
                driver.SetMultiSelect(By.Name("LotDesc"), String.Join(",", putOptionsLotDesc), true);
            }

            //driver.SetMultiSelect(By.Name("PoolPrivateDesc"), "N", true); //Private Pool Description
            //ShowMarkToFieldByNameElement(driver, "PoolPrivateDesc");

            if (listing.LotDesc.Contains("ONGLF") || listing.LotDesc.Contains("INGLF"))
                driver.SetAttribute(By.Name("Golfcourse"), listing.GolfCourseName, "value", true); //Golf Course Name
            if (listing.LotDesc.Contains("WTRVW") || listing.LotDesc.Contains("WTRFT"))
                driver.SetMultiSelect(By.Name("WaterAmenity"), listing.WaterfrontDesc); //Waterfront Features

            driver.SetMultiSelect(By.Name("StreetSurface"), listing.StreetSurfaceDesc, true); //Street Surface
            //ShowMarkToFieldByNameElement(driver, "StreetSurface");

            driver.SetMultiSelect(By.Name("HeatSystem"), listing.HeatSystemDesc); //Heating System Description
            //ShowMarkToFieldByNameElement(driver, "HeatSystem");

            //driver.SetMultiSelect(By.Name("CoolSystem"), listing.CoolSystemDesc); //Cooling System Desc
            //ShowMarkToFieldByNameElement(driver, "CoolSystem");

            List<String> putOptionsCoolSystemDesc = new List<String>();
            List<String> optionsCoolSystemDesc = String.IsNullOrWhiteSpace(listing.CoolSystemDesc) ? new List<string>() : listing.CoolSystemDesc.Split(',').ToList();
            foreach (string option in optionsCoolSystemDesc)
            {
                switch (option)
                {
                    case "ATCFN":
                        putOptionsCoolSystemDesc.Add("OTHER"); 
                        break;
                    case "CNELE":
                    case "CNGAS":
                    case "HPUMP":
                    case "NONE_":
                    case "OTHER":
                    case "SOLAR":
                    case "WDUNT":
                    case "ZONED":
                        putOptionsCoolSystemDesc.Add(option);
                        break;
                }
            }

            if(putOptionsCoolSystemDesc.Count > 0)
            {
                unSelectAllOptions(driver, "CoolSystem");
                driver.SetMultiSelect(By.Name("CoolSystem"), String.Join(",", putOptionsCoolSystemDesc), true);
            }

            //driver.SetMultiSelect(By.Name("WaterSewer"), listing.SewerDesc); //Water Sewer Description
            //ShowMarkToFieldByNameElement(driver, "WaterSewer");

            List<String> putOptionsSewerDesc = new List<String>();
            List<String> optionsSewerDesc = String.IsNullOrWhiteSpace(listing.SewerDesc) ? new List<string>() : listing.SewerDesc.Split(',').ToList();
            foreach (string option in optionsSewerDesc)
            {
                switch (option)
                {
                    case "NONE_":
                    case "NOSEW":
                    case "PUBSW":
                    case "PUBWT":
                    case "SEPTI":
                    case "WELL_":
                    case "WTDIS":
                        putOptionsSewerDesc.Add(option);
                        break;
                    case "OTHER":
                    case "PRVWT":
                    case "PUBAR":
                        putOptionsSewerDesc.Add("OTHER");
                        break;
                }
            }

            if(putOptionsSewerDesc.Count > 0)
            {
                unSelectAllOptions(driver, "WaterSewer");
                driver.SetMultiSelect(By.Name("WaterSewer"), String.Join(",", putOptionsSewerDesc), true);
            }
            
            //driver.SetMultiSelect(By.Name("FinanceAvail"), listing.FinancingProposed); //Heating System Description  
            ShowMarkToFieldByNameElement(driver, "FinanceAvail");

            if (listing.UtilitiesDesc == "1")
                driver.SetRadioButton(By.Id("UtilityDistrict"), "Y"); //Utility District
            else if (listing.UtilitiesDesc == "0")
                driver.SetRadioButton(By.Id("UtilityDistrict"), "N"); //Utility District
            else
                driver.SetRadioButton(By.Id("UtilityDistrict"), ""); //Utility District

            //driver.SetMultiSelect(By.Name("Restrictions"), "UNKWN");  //Restrictions
            //ShowMarkToFieldByNameElement(driver, "Restrictions");

            List<String> putOptionsRestrictions = new List<String>();
            List<String> optionsRestrictions = String.IsNullOrWhiteSpace(listing.Restrictions) ? new List<string>() : listing.Restrictions.Split(',').ToList();

            foreach (string option in optionsRestrictions)
            {
                switch (option)
                {
                    /*case "DEEDR":
                    case "HISRS":
                    case "HORSE":
                    case "MOBIL":
                    case "RSTRC":
                        putOptionsRestrictions.Add(option);
                        break;
                    case "UNKWN":
                    case "UNKWV":
                        putOptionsRestrictions.Add("UNKWN");
                        break;
                    case "NONE_":
                        putOptionsRestrictions.Add("UNRST");
                        break;
                    case "UNREC":
                    case "UNRST":
                        putOptionsRestrictions.Add("UNRST");
                        break;
                    case "ZONED":
                    case "ZONING":
                        putOptionsRestrictions.Add("ZONING");
                        break;*/
                    default:
                        putOptionsRestrictions.Add(option);
                        break;
                }
            }

            if(putOptionsRestrictions.Count > 0)
                driver.SetMultiSelect(By.Name("Restrictions"), String.Join(",", putOptionsRestrictions), true);
            
            driver.SetMultiSelect(By.Name("Defects"), "NODEF"); //Defects

            //driver.SetMultiSelect(By.Name("Disclosures"), "NONE_"); //Heating System Description
            //ShowMarkToFieldByNameElement(driver, "Disclosures");
            
            List<String> putOptionsDisclosures = new List<String>();
            List<String> optionsDisclosures = String.IsNullOrWhiteSpace(listing.Disclosures) ? new List<string>() : listing.Disclosures.Split(',').ToList();
            foreach (string option in optionsDisclosures)
            {
                switch (option)
                {
                    /*case "ADDND":
                    case "CORPL":
                    case "EXCLS":
                    case "FORCL":
                    case "HMPRT":
                    case "LEVEE":
                    case "MILND":
                    case "MUD12":
                    case "OWNAG":
                    case "SHORT":
                    case "SNIOR":
                        putOptionsDisclosures.Add(option);
                        break;
                    case "COVEN":
                    case "NORFN":
                    case "OTHER":
                    case "PETS_":
                    case "RFUSL":
                    case "SELLR":
                        putOptionsDisclosures.Add("OTHER");
                        break;*/
                    default:
                        putOptionsDisclosures.Add(option);
                        break;
                }
            }

            if(putOptionsDisclosures.Count > 0)
                driver.SetMultiSelect(By.Name("Disclosures"), String.Join(",", putOptionsDisclosures), true);
            
            driver.SetSelect(By.Name("ListType"), "EXAGY"); //List Type 
            //ShowMarkToFieldByNameElement(driver, "ListType");

            driver.WriteTextbox(By.Name("Exclusions"), listing.Excludes, true); //Exclusions

            if(listing.HasHOA == "1")
                driver.SetRadioButton(By.Name("HOAMandatory"), "Y"); //Mandatory HOA / Mngmnt Co
            else
                driver.SetRadioButton(By.Name("HOAMandatory"), "N"); //Mandatory HOA / Mngmnt Co

            driver.WriteTextbox(By.Name("ManagementCo"), listing.AssocName, true); //Mandatory HOA / Mngmt Co Name
            driver.WriteTextbox(By.Name("HOAPhone"), listing.AssocPhone, true); //Mandatory HOA / Mngmnt Co Phone
            driver.WriteTextbox(By.Name("CompSubAgent"), "0%"); //Sale Subagent Compensation
            //driver.WriteTextbox(By.Name("CompBuy"), "3%"); //Sale Buyer Agent Compensation
            // QLIST-113
            driver.WriteTextbox(By.Name("CompBuy"), listing.CompBuy); //Sale Buyer Agent Compensation
            driver.WriteTextbox(By.Name("CompBonus"), listing.CompBuyBonus, true); //Bonus
            driver.WriteTextbox(By.Name("BonusEndDate"), listing.CompBuyBonusExpireDate, true); //Bonus End Date

            driver.SetRadioButton(By.Name("VariableDualRate"), "N"); //Variable Dual Rate
            #endregion
        }
        private void editFinancialInformation(CoreWebDriver driver)
        {
            #region Financial Information

            driver.SetSelect(By.Name("LoanAssumable"), "NO123"); //1st Lien Assumable

            driver.SetSelect(By.Name("AnnualMaintDesc"), listing.HOA); //Maintenance Fee
            //ShowMarkToFieldByNameElement(driver, "AnnualMaintDesc");

            driver.WriteTextbox(By.Name("AnnualMaintFee"), listing.AssocFee, true); //Maintenance Fee Amount

            //driver.SetSelect(By.Name("MaintFeePaySchedule"), listing.AssocFeePaid, true); //Maintenance Fee Payment Schedule
            //ShowMarkToFieldByNameElement(driver, "MaintFeePaySchedule");

            List<String> putOptionsAssocFeePaid = new List<String>();
            List<String> optionsAssocFeePaid = String.IsNullOrWhiteSpace(listing.AssocFeePaid) ? new List<string>() : listing.AssocFeePaid.Split(',').ToList();
            foreach (string option in optionsAssocFeePaid)
            {
                switch (option)
                {
                    case "ANNUA":
                        putOptionsAssocFeePaid.Add("ANNUALLY");
                        break;
                    case "MONTH":
                        putOptionsAssocFeePaid.Add("MONTHLY");
                        break;
                    case "QUART":
                        putOptionsAssocFeePaid.Add("QUARTERLY");
                        break;
                }
            }

            if(putOptionsAssocFeePaid.Count > 0)
            {
                unSelectAllOptions(driver, "MaintFeePaySchedule");
                driver.SetSelect(By.Name("MaintFeePaySchedule"), String.Join(",", putOptionsAssocFeePaid), true);
            }
            
            if(listing.HasOtherFees == "1")
                driver.SetRadioButton(By.Name("FeeOther"), "Y"); //Other Mandatory Fees
            else
                driver.SetRadioButton(By.Name("FeeOther"), "N"); //Other Mandatory Fees

            driver.WriteTextbox(By.Name("FeeOtherAmount"), listing.OtherFees, true); //Other Mandatory Fee Amount
            driver.WriteTextbox(By.Name("OtherMandatoryFee"), listing.AssocFeeOtherDesc, true); //Other Mandatory Fees Include
            driver.WriteTextbox(By.Name("TaxYear"), listing.TaxYear, true); //Tax Year
            driver.WriteTextbox(By.Name("TaxRate"), decimal.Round(listing.TaxRate, 2), true); //Total Tax Rate
            driver.WriteTextbox(By.Name("Exemptions"), listing.ExemptionsDesc, true); //Exemptions

            //driver.SetSelect(By.Name("Ownership"), "FULL_", true); //Ownership Type
            ShowMarkToFieldByNameElement(driver, "Ownership");
            #endregion

        }
        #endregion

        #region Edit Price
        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {

            //this.wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            this.listing = listing;
            Login(driver, listing);
            EditListing(driver);
            driver.Click(By.Name("loadchangestatus"), false); //Change Status/Price/Listing Agent
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Name("pricechng")));
            driver.WriteTextbox(By.Name("chngListPrice"), listing.ListPrice);
            // driver.Click(By.Name("pricechng"), false);
            return UploadResult.Success;
        }
        #endregion

        #region Edit Images

        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            //this.wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
            this.listing = listing;
            this.newListing = false;

            Login(driver, listing);

            // Open Media Manager
            Thread.Sleep(TimeSpan.FromSeconds(5));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("main")));

            //if (listing.BrokerOffice.ToLower() == "hhom01")
            //    driver.FindElement(By.Id("ctl05_3")).Click();
            //else
            //    driver.FindElement(By.Id("ctl05_4")).Click();
            if (listing.BrokerOffice.ToLower() == "hhom01")
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl05_3")));
                driver.FindElement(By.Id("ctl05_3")).Click();
            }
            /*else if (listing.BrokerOffice.ToLower() == "tmrt01")
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl05_2")));
                driver.FindElement(By.Id("ctl05_2")).Click();
            }*/
            else
            {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl05_4")));
                driver.FindElement(By.Id("ctl05_4")).Click();
            }
            
            Thread.Sleep(TimeSpan.FromSeconds(2));// Add/edit button
            driver.SwitchTo("main"); // switch to "main" iframe
            driver.SwitchTo("display"); // switch to "display" iframe
            driver.wait.Until(ExpectedConditions.ElementExists(By.Name("mlsnumlist")));
            driver.WriteTextbox(By.Name("mlsnumlist"), listing.MLSNum); // fill the MLS number
            driver.FindElement(By.Name("loadprplist2")).Click(); // Link "Show Listings by MLS Number"
            driver.wait.Until(ExpectedConditions.ElementExists(By.Name("loadmediamanager")));
            driver.FindElement(By.Name("loadmediamanager")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var window = driver.WindowHandles.FirstOrDefault(c => c != driver.CurrentWindowHandle);
            driver.SwitchTo().Window(window);
            // Capture MLS-specific ID for the listing

            var urlParams = driver.Url.Replace("http://www.harmls.com/TempoCommon/MediaManager/Media.html?", "").Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            var listingId = urlParams[0].Replace("PrpUid=", "");
            var propType = urlParams[1].Replace("PropType=", "");

            // Capture all cookies
            var cookies = driver.Manage().Cookies.AllCookies.Select(c => new { c.Name, c.Value, c.Path, c.Domain });

            var handler = new HttpClientHandler()
            {
                CookieContainer = new CookieContainer()
            };

            foreach (var cookie in cookies)
            {
                handler.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }

            //Configure HTTP Client
            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://www.harmls.com/TempoCommon/MediaManager/"),
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.118 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "ShockwaveFlash/17.0.0.134");
            httpClient.DefaultRequestHeaders.Add("Referer", "http://www.harmls.com/TempoCommon/MediaManager/Media.swf");
            httpClient.DefaultRequestHeaders.Add("Origin", "http://www.harmls.com");

            var ids = GetListOfImages(httpClient, listingId, propType);

            foreach (var img in ids)
            {
                RemoveImage(httpClient, img);
            }

            driver.Navigate().Refresh();

            try
            {
                driver.SwitchTo().Alert().Accept();
            }
            catch { }

            Thread.Sleep(2000);

            var idsImages = GetListOfImages(httpClient, listingId, propType);
            if (idsImages != null && idsImages.Count<dynamic>() > 0)
            {
                driver.ExecuteScript(" alert('Some photos could not be deleted. Please delete manually these photos in order to continue the upload process.')");

                while (GetListOfImages(httpClient, listingId, propType).Count<dynamic>() > 0)
                {
                    // wait
                }
            }

            //var i = 0;
            
            //foreach (var img in media.OfType<ResidentialListingMedia>())
            foreach (var img in media.OfType<ResidentialListingMedia>().OrderBy(c => c.Order))
            {
                //img.Order = i++;
                AddNewImage(httpClient, listingId, propType, listing, img);

                driver.Navigate().Refresh();

            }
             
            driver.Navigate().Refresh();

            return UploadResult.Success;
        }

        private IEnumerable<dynamic> GetListOfImages(HttpClient client, string listingId, string propType)
        {
            var xmlRequest = string.Format("<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:s=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                                           "<SOAP-ENV:Body><tns:GetMediaListDTO xmlns:tns=\"http://marketlinx.com/\"><tns:uidPrp>{0}</tns:uidPrp><tns:prpType>{1}</tns:prpType></tns:GetMediaListDTO>" +
                                           "</SOAP-ENV:Body></SOAP-ENV:Envelope>", listingId, propType);

            var message = new HttpRequestMessage(HttpMethod.Post, "MediaManagerWebService.asmx")
            {
                Content = new StringContent(xmlRequest, Encoding.UTF8, "text/xml")
            };

            message.Headers.Add("SOAPAction", "\"http://marketlinx.com/GetMediaListDTO\"");

            var t = client.SendAsync(message).Result;
            var f = t.Content.ReadAsStringAsync().Result;

            var doc = XmlToDynamic.RemoveAllNamespaces(XDocument.Load(new StringReader(f)).Elements().First());
            dynamic root = new ExpandoObject();

            var resultsXml = doc.XPathSelectElement("/Body/GetMediaListDTOResponse/GetMediaListDTOResult");

            XmlToDynamic.Parse(root, resultsXml);

            var list = (IDictionary<string, object>)root.GetMediaListDTOResult;

            if (list.ContainsKey("Result"))
            {
                if (list["Result"] is IEnumerable<object>)
                {
                    var resultsObj = (List<object>)root.GetMediaListDTOResult.Result;
                    return resultsObj.Where(c => c is ExpandoObject).Cast<ExpandoObject>();
                }
                else
                {
                    return new List<object>() { list["Result"] };
                }
            }
            else
            {
                return new List<object>();
            }
        }

        #region AddSoapRequest
        private const string AddSoapRequest =
           "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:s=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
           "  <SOAP-ENV:Body>" +
           "    <tns:SaveMedia xmlns:tns=\"http://marketlinx.com/\">" +
           "      <tns:mediaListDTO>" +
           "        <tns:MediaObjectDTO>" +
           "          <tns:MediaUid>" +
           "            <tns:v>0</tns:v>" +
           "            <tns:hv>false</tns:hv>" +
           "          </tns:MediaUid>" +
           "          <tns:MediaType>pic</tns:MediaType>" +
           "          <tns:MediaSource xsi:nil=\"true\"/>" +
           "          <tns:MediaName>{0}</tns:MediaName>" +
           "          <tns:MediaSize>" +
           "            <tns:v>0</tns:v>" +
           "            <tns:hv>false</tns:hv>" +
           "          </tns:MediaSize>" +
           "          <tns:TableUid>" +
           "            <tns:v>{1}</tns:v>" +
           "            <tns:hv>true</tns:hv>" +
           "          </tns:TableUid>" +
           "          <tns:TableName>{2}</tns:TableName>" +
           "          <tns:MediaDescr><![CDATA[{3}]]></tns:MediaDescr>" +
           "          <tns:Status>rev</tns:Status>" +
           "          <tns:DisplayOrder>" +
           "            <tns:v>{4}</tns:v>" +
           "            <tns:hv>true</tns:hv>" +
           "          </tns:DisplayOrder>" +
           "          <tns:InputDate xsi:nil=\"true\"/>" +
           "          <tns:InputID xsi:nil=\"true\"/>" +
           "          <tns:InputName xsi:nil=\"true\"/>" +
           "          <tns:Modified xsi:nil=\"true\"/>" +
           "          <tns:ModifyID xsi:nil=\"true\"/>" +
           "          <tns:ModifyName xsi:nil=\"true\"/>" +
           "          <tns:PrimaryPic>{5}</tns:PrimaryPic>" +
           "          <tns:Buyer>yes</tns:Buyer>" +
           "          <tns:MediaPath xsi:nil=\"true\"/>" +
           "          <tns:MediaPathHighRes xsi:nil=\"true\"/>" +
           "          <tns:Action>{6}</tns:Action>" +
           "          <tns:MediaFile>{7}</tns:MediaFile>" +
           "          <tns:ReviewDate xsi:nil=\"true\"/>" +
           "          <tns:ReviewID xsi:nil=\"true\"/>" +
           "          <tns:ReviewName xsi:nil=\"true\"/>" +
           "          <tns:MlsNum>{8}</tns:MlsNum>" +
           "          <tns:PropType>{2}</tns:PropType>" +
           "          <tns:ListStatus>act</tns:ListStatus>" +
           "          <tns:TabId>0</tns:TabId>" +
           "          <tns:IsFax>false</tns:IsFax>" +
           "          <tns:HasChanges>true</tns:HasChanges>" +
           "          <tns:ProcessIndex>0</tns:ProcessIndex>" +
           "        </tns:MediaObjectDTO>" +
           "      </tns:mediaListDTO>" +
           "    </tns:SaveMedia>" +
           "  </SOAP-ENV:Body>" +
           "</SOAP-ENV:Envelope>";
        #endregion

        private void AddNewImage(HttpClient client, string listingId, string propType, ResidentialListingRequest listing, ResidentialListingMedia media)
        {
            var xmlRequest = string.Format(AddSoapRequest, Path.GetFileName(media.PathOnDisk), listingId, propType, media.Caption, media.Order, media.IsPrimary ? "y" : "n", "Add", Convert.ToBase64String(media.Data), listing.MLSNum);

            var message = new HttpRequestMessage(HttpMethod.Post, "MediaManagerWebService.asmx")
            {
                Content = new StringContent(xmlRequest, Encoding.UTF8, "text/xml")
            };

            message.Headers.Add("SOAPAction", "\"http://marketlinx.com/SaveMedia\"");

            client.SendAsync(message).Wait();
        }

        #region DeleteSoapRequest
        private const string DeleteSoapRequest =
       "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:s=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
       "  <SOAP-ENV:Body>" +
       "    <tns:SaveMedia xmlns:tns=\"http://marketlinx.com/\">" +
       "      <tns:mediaListDTO>" +
       "        <tns:MediaObjectDTO>" +
       "          <tns:MediaUid>" +
       "            <tns:v>{0}</tns:v>" +
       "            <tns:hv>true</tns:hv>" +
       "          </tns:MediaUid>" +
       "          <tns:MediaType>pic</tns:MediaType>" +
       "          <tns:MediaSource>{1}</tns:MediaSource>" +
       "          <tns:MediaName>{2}</tns:MediaName>" +
       "          <tns:MediaSize>" +
       "            <tns:v>{3}</tns:v>" +
       "            <tns:hv>true</tns:hv>" +
       "          </tns:MediaSize>" +
       "          <tns:TableUid>" +
       "            <tns:v>{4}</tns:v>" +
       "            <tns:hv>true</tns:hv>" +
       "          </tns:TableUid>" +
       "          <tns:TableName>{5}</tns:TableName>" +
       "          <tns:MediaDescr>{6}</tns:MediaDescr>" +
       "          <tns:Status>{7}</tns:Status>" +
       "          <tns:DisplayOrder>" +
       "            <tns:v>{8}</tns:v>" +
       "            <tns:hv>true</tns:hv>" +
       "          </tns:DisplayOrder>" +
       "          <tns:InputDate xsi:nil=\"true\"/>" +
       "          <tns:InputID xsi:nil=\"true\"/>" +
       "          <tns:InputName xsi:nil=\"true\"/>" +
       "          <tns:Modified xsi:nil=\"true\"/>" +
       "          <tns:ModifyID xsi:nil=\"true\"/>" +
       "          <tns:ModifyName xsi:nil=\"true\"/>" +
       "          <tns:PrimaryPic>{9}</tns:PrimaryPic>" +
       "          <tns:Buyer>{10}</tns:Buyer>" +
       "          <tns:MediaPath>{11}</tns:MediaPath>" +
       "          <tns:MediaPathHighRes xsi:nil=\"true\"/>" +
       "          <tns:Action>Delete</tns:Action>" +
       "          <tns:MediaFile xsi:nil=\"true\"/>" +
       "          <tns:ReviewDate xsi:nil=\"true\"/>" +
       "          <tns:ReviewID xsi:nil=\"true\"/>" +
       "          <tns:ReviewName xsi:nil=\"true\"/>" +
       "          <tns:MlsNum>{12}</tns:MlsNum>" +
       "          <tns:PropType>{13}</tns:PropType>" +
       "          <tns:ListStatus>act</tns:ListStatus>" +
       "          <tns:TabId>1</tns:TabId>" +
       "          <tns:IsFax>false</tns:IsFax>" +
       "          <tns:HasChanges>true</tns:HasChanges>" +
       "          <tns:ProcessIndex>0</tns:ProcessIndex>" +
       "        </tns:MediaObjectDTO>" +
       "      </tns:mediaListDTO>" +
       "    </tns:SaveMedia>" +
       "  </SOAP-ENV:Body>" +
       "</SOAP-ENV:Envelope>";
        #endregion

        private void RemoveImage(HttpClient client, dynamic media)
        {
            var xmlRequest = string.Format(DeleteSoapRequest, media.MediaUid.v, media.MediaSource, media.MediaName, media.MediaSize.v, media.TableUid.v, media.TableName, media.MediaDescr, media.Status, media.DisplayOrder.v,
            media.PrimaryPic, media.Buyer, media.MediaPath, media.MlsNum, media.PropType);

            var message = new HttpRequestMessage(HttpMethod.Post, "MediaManagerWebService.asmx")
            {
                Content = new StringContent(xmlRequest, Encoding.UTF8, "text/xml")
            };

            message.Headers.Add("SOAPAction", "\"http://marketlinx.com/SaveMedia\"");

            client.SendAsync(message).Wait();
        }

        #endregion

        #region Edit Status
        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            //this.wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            this.listing = listing;
            Login(driver, listing);
            EditListing(driver);
            driver.Click(By.Name("loadchangestatus"), false); //Change Status/Price/Listing Agent

            //listing.ListStatus = "pend";
            //listing.PendingDate = DateTime.Today;
            //listing.EstClosedDate = DateTime.Today;
            //listing.SellingAgentPresent = true;
            driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Name("cancel")));
            Thread.Sleep(500);


            var listStatus = listing.ListStatus.ToLower();

            switch (listStatus)
            {
                case "op":
                    if (ExpectedConditions.ElementExists(By.Name("opbutton")) != null)
                    {
                        #region Change To Option Pending
                        if (listing.PendingDate != null)
                            driver.WriteTextbox(By.Name("opPendingDate"), listing.PendingDate.Value.ToShortDateString());//Pending Date
                        if (listing.EstClosedDate != null)
                            driver.WriteTextbox(By.Name("opEstClosedDate"), listing.EstClosedDate.Value.ToShortDateString()); //Estimated Closed Date
                        if (listing.ExpiredDateOption != null)
                            driver.WriteTextbox(By.Name("opOptionEndDate"), listing.ExpiredDateOption.Value.ToShortDateString());

                        //driver.SetRadioButton(By.Name("opend_BuyerContingent"), listing.CoopSale); //Contingent on Sale of Other Property by Buyer
                        ShowContinueBottonByNameElement(driver, "opend_BuyerContingent");

                        Thread.Sleep(500);
                        handleWindow(driver, By.Name("getopsellagt"));

                        if (Convert.ToBoolean(listing.SellingAgentPresent))
                        {
                            //driver.SetRadioButton(By.Name("op_BuyerRep_YN"), "Y"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "op_BuyerRep_YN");
                        }
                        else
                        {
                            //driver.SetRadioButton(By.Name("op_BuyerRep_YN"), "N"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "op_BuyerRep_YN");
                        }

                        //driver.Click(By.Name("opbutton"), false);
                        #endregion
                    }
                    break;
                case "pend":
                case "psho":
                    if (listing.OldListStatus.ToLower() == "act")
                    {
                        #region Change To Pending Continue To Show
                        if (listing.PendingDate != null)
                            driver.WriteTextbox(By.Name("pshPendingDate"), listing.PendingDate.Value.ToShortDateString());//Pending Date
                        if (listing.EstClosedDate != null)
                            driver.WriteTextbox(By.Name("pshEstClosedDate"), listing.EstClosedDate.Value.ToShortDateString()); //Estimated Closed Date

                        //driver.SetRadioButton(By.Name("pend_BuyerContingent"), listing.CoopSale.ToLower());//Contingent on Sale of Other Property by Buyer
                        ShowContinueBottonByNameElement(driver, "pend_BuyerContingent");

                        Thread.Sleep(500);
                        handleWindow(driver, By.Name("getpshsellagt"));

                        if (Convert.ToBoolean(listing.SellingAgentPresent))
                        {
                            //driver.SetRadioButton(By.Name("psh_BuyerRep_YN"), "Y"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "psh_BuyerRep_YN");
                        }
                        else
                        {
                            //driver.SetRadioButton(By.Name("psh_BuyerRep_YN"), "N"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "psh_BuyerRep_YN");
                        }
                        
                        //driver.Click(By.Name("pendupdbutton"), false);
                        #endregion
                    } else if ((listing.OldListStatus.ToLower() == "psho" || listing.OldListStatus.ToLower() == "pend"))
                    {
                        #region  Pending OR Pending to show
                        if (listing.PendingDate != null)
                            driver.WriteTextbox(By.Name("pshPendingDate"), listing.PendingDate.Value.ToShortDateString());//Pending Date
                        if (listing.EstClosedDate != null)
                            driver.WriteTextbox(By.Name("pshEstClosedDate"), listing.EstClosedDate.Value.ToShortDateString()); //Estimated Closed Date
                        driver.SetAttribute(By.Name("pend_BuyerContingent"), listing.CoopSale.ToLower(), "value"); //Contingent on Sale of Other Property by Buyer
                        Thread.Sleep(500);
                        handleWindow(driver, By.Name("getpshsellagt"));

                        if (Convert.ToBoolean(listing.SellingAgentPresent))
                        {
                            //driver.SetRadioButton(By.Name("psh_BuyerRep_YN"), "Y"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "psh_BuyerRep_YN");
                        }
                        else
                        {
                            //driver.SetRadioButton(By.Name("psh_BuyerRep_YN"), "N"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "psh_BuyerRep_YN");
                        }
                        
                        //driver.Click(By.Name("pendbutton"), false);
                        #endregion
                    }
                    break;
                case "closd":
                    if (ExpectedConditions.ElementExists(By.Name("closd")) != null)
                    {
                        #region Change To Sold/Leased


                        if (newListing)
                        {
                            driver.WriteTextbox(By.Name("clPendingDate"), DateTime.Now.AddDays(-2).ToShortDateString());//Pending Date
                            driver.WriteTextbox(By.Name("ClosedDate"), DateTime.Now.ToShortDateString());//Closed Date
                        }
                        else
                        {
                            if (listing.PendingDate == null)
                                driver.WriteTextbox(By.Name("clPendingDate"), DateTime.Now.AddDays(-31).ToShortDateString());//Pending Date
                            else
                                driver.WriteTextbox(By.Name("clPendingDate"), listing.ClosedDate.Value.ToShortDateString(), true);//Pending Date


                            if (listing.ClosedDate != null)
                                driver.WriteTextbox(By.Name("ClosedDate"), listing.ClosedDate.Value.ToShortDateString());//Closed Date
                        }
                        driver.WriteTextbox(By.Name("clEstClosedDate"), DateTime.Now.AddDays(31).ToShortDateString(), true); //Estimated Closed Date
                        driver.WriteTextbox(By.Name("SalesPrice"), listing.SalesPrice); //Estimated Closed Date
                                                                                        //driver.SetSelect(By.Name("SoldTerms"), listing.SoldTerms); //Sold Terms
                                                                                        //driver.WriteTextbox(By.Name("PointsDiscount"), listing.BuyerPoints); //Total Discount Points
                        driver.WriteTextbox(By.Name("SalesPrice"), listing.SalesPrice); //Total Discount Points
                                                                                        //driver.WriteTextbox(By.Name("LoanAmountNew"), listing.Loan1Amount); //New Loan Amount
                                                                                        //driver.WriteTextbox(By.Name("AmortizedYears"), listing.Loan1Years); //Amortized Years
                        driver.WriteTextbox(By.Name("CoopSale"), listing.CoopSale); //CoOp Sale
                        driver.WriteTextbox(By.Name("SellerToClosingCosts"), listing.SellerPaid); //Seller Contribution to Buyer Costs
                        driver.WriteTextbox(By.Name("TitlePaidBy"), listing.TitlePaidBy); //Owner's Title Paid By
                        driver.WriteTextbox(By.Name("RepairSeller"), listing.RepairsAmount); //Repair Paid Seller
                        Thread.Sleep(500);
                        handleWindow(driver, By.Name("getsellagt"));
                        if (Convert.ToBoolean(listing.SellingAgentPresent))
                        {
                            //driver.SetRadioButton(By.Name("cls_BuyerRep_YN"), "Y"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "cls_BuyerRep_YN");
                        }
                        else
                        {
                            //driver.SetRadioButton(By.Name("cls_BuyerRep_YN"), "N"); //Buyer Represented by licensed agent
                            ShowContinueBottonByNameElement(driver, "cls_BuyerRep_YN");
                        }
                                                                                    //driver.Click(By.Name("closd"), false);
                        #endregion
                    }
                    break;
                case "with":
                    if (ExpectedConditions.ElementExists(By.Name("with")) != null)
                    {
                        #region Change To Withdrawn
                        driver.WriteTextbox(By.Name("WithdrawnDate"), DateTime.Now.ToShortDateString()); //Withdrawn Date
                                                                                                         //driver.Click(By.Name("with"), false);   //Change to Withdrawn
                        #endregion
                    }
                    break;
                case "term":
                case "can":
                    if (ExpectedConditions.ElementExists(By.Name("term")) != null)
                    {
                        #region Change To Terminated
                        driver.WriteTextbox(By.Name("TerminationDate"), DateTime.Now.ToShortDateString());    //Off Market Date
                                                                                                              //driver.Click(By.Name("term"), false); //Change to Terminated
                        #endregion
                    }
                    break;
                case "act":
                    if (ExpectedConditions.ElementExists(By.Name("act")) != null)
                    {
                        #region Return to Active
                        //var expirationDate = (listing.ListDate ?? DateTime.Now.Date).AddYears(1);

                        //driver.WriteTextbox(By.Name("ActiveExpireDate"), listing.ExpiredDate.Value.ToShortDateString());   //New Expiration Date
                        driver.WriteTextbox(By.Name("ExtExpireDate"), listing.ExpiredDate.Value.ToShortDateString());   //New Expiration Date
                                                                                                                        //driver.Click(By.Name("act"), false); //Return to Active
                        #endregion
                    }
                    break;

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
            driver.Navigate("http://members.har.com/mopx/doLogout.cfm");
            return UploadResult.Success;
        }

        public void handleWindow(CoreWebDriver driver, By by)
        {
            driver.Click(by, false);
            Thread.Sleep(1500);
            List<string> handles = driver.WindowHandles.ToList<String>();
            driver.SwitchTo().Window(handles.Last());
            driver.wait.Until(x => ExpectedConditions.ElementIsVisible(By.Name("FirstName")));
            Thread.Sleep(1500);
            if (listing.AgentID_SELL == null)
            {
                driver.WriteTextbox(By.Name("Pubid"), "NONMLS");
                driver.Click(By.Name("B1"), false);

            }
            else
            {
                driver.WriteTextbox(By.Name("FirstName"), listing.SellingAgentFristName);
                driver.WriteTextbox(By.Name("LastName"), listing.SellingAgentLastName);
                driver.WriteTextbox(By.Name("OfficeID"), listing.SellingAgentUIDOFFICE);
                driver.Click(By.Name("B1"), false);
            }

            IWebElement element = driver.FindElements(By.TagName("a")).FirstOrDefault();
            if (element != null)
                element.Click();

            driver.SwitchTo().Window(handles.First());


        }

        /// <summary>
        /// Temporal method for resolve lookups problem
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="elementID"></param>
        //private void ShowContinueBottonByIdElement(CoreWebDriver driver, String elementID)
        //{
        //    ((IJavaScriptExecutor)driver).ExecuteScript("var button= document.createElement('input'); button.type='button'; button.onclick= function() { this.id='btn_continue'; }; button.id='husaBtn'; button.style.backgroundColor= '#FF0000'; button.value='CONTINUE...'; document.getElementById('" + elementID + "').parentNode.appendChild(button); ");

        //    driver.wait.Until(ExpectedConditions.ElementExists(By.Id("btn_continue")));

        //    ((IJavaScriptExecutor)driver).ExecuteScript("var button = document.getElementById('btn_continue'); var parentN = button.parentNode; parentN.removeChild(button);");
        //}

        /// <summary>
        /// Temporal method for resolve lookups problem
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="elementName"></param>
        private void ShowContinueBottonByNameElement(CoreWebDriver driver, String elementName)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("var button= document.createElement('input'); button.type='button'; button.onclick= function() { this.id='btn_continue'; }; button.id='husaBtn'; button.style.backgroundColor= '#FF0000'; button.value='CONTINUE...'; document.getElementsByName('" + elementName + "')[0].parentNode.appendChild(button); ");

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("btn_continue")));

            ((IJavaScriptExecutor)driver).ExecuteScript("var button = document.getElementById('btn_continue'); var parentN = button.parentNode; parentN.removeChild(button);");
        }

        /// <summary>
        /// Draws a mark for the fiel
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="elementName"></param>
        private void ShowMarkToFieldByNameElement(CoreWebDriver driver, String elementName)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(" var elem = document.createElement('img'); elem.setAttribute('tooltip', 'Requires attention'); elem.setAttribute('src', 'http://www.fancyicons.com/free-icons/221/modern-anti-malware/png/24/security_warning_24.png');elem.setAttribute('height', '24');elem.setAttribute('width', '24'); document.getElementsByName('" + elementName + "')[0].parentNode.appendChild(elem); document.getElementsByName('" + elementName + "')[0].parentNode.setAttribute('style', 'background-color: #ffe3a3;') ");
        }


        private void unSelectAllOptions(CoreWebDriver driver, String elementName)
        {
            //try
            //{
                ((IJavaScriptExecutor)driver).ExecuteScript("var elements = document.getElementsByName('" + elementName + "')[0].options; for (var i = 0; i < elements.length; i++) { elements[i].selected = false; } ");
            //}
            //catch { }
        }

        public UploadResult UploadLeasing(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            throw new NotImplementedException();
        }
    }
}
