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
using System.Text.RegularExpressions;

namespace Husa.Core.Uploaders.Houston
{
    public partial class HoustonHARUploader :   IUploader, 
                                                IEditor, 
                                                IPriceUploader, 
                                                IStatusUploader, 
                                                IImageUploader, 
                                                ICompletionDateUploader, 
                                                IUpdateOpenHouseUploader,
                                                IUploadVirtualTourUploader,
                                                ILeaseUploader
    {
        OpenHouseBase OH = new OpenHouseBase();

        //WebDriverWait wait;
        private ResidentialListingRequest listing;
        private bool newListing; 

        public System.Globalization.TextInfo textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        
        public bool IsFlashRequired { get { return true; } }

        public bool CanUpload(ResidentialListingRequest listing)
        {
            //This method must return true if the listing can be uploaded with this MarketSpecific Uploader
            return listing.MarketName == "Houston";
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
                //newProperty(driver);
            }
            else
            {
                EditListing(driver);
                driver.wait.Until(ExpectedConditions.ElementExists(By.LinkText("Single-Family Add/Edit")));
                driver.FindElement(By.LinkText("Single-Family Add/Edit")).Click();
            }

            // 1. Listing Information
            UpdateListingInformation(driver);

            // 2. Map Information
            UpdateMapInformation(driver);

            // 3. Property Information
            UpdatePropertyInformation(driver);

            // 4. Rooms
            UpdateRooms(driver);

            // 5. Financial Information
            UpdateFinancialInformation(driver);

            // 6. Showing Information
            UpdateShowingInformation(driver);

            // 7. Remarks
            UpdateRemarks(driver);

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
                //newProperty(driver);
            }
            else
            {
                EditListing(driver);
                //startupdate(driver);
            }

            return UploadResult.Success;
        }
        private void startInsert(CoreWebDriver driver)
        {
            #region newProperty
            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("TopMenuTable")));
            driver.Navigate("http://matrix.harmls.com/Matrix/AddEditInput");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            
            driver.FindElement(By.LinkText("Add new")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_dlInputList_ctl00_m_btnSelect")));
            driver.FindElement(By.Id("m_dlInputList_ctl00_m_btnSelect")).Click();
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")));
            driver.FindElement(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")).Click();

            try {
                driver.FindElement(By.Id("m_rpPageList_ctl02_lbPageLink")).Click();
            } catch { }

            //driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_658")));
            //driver.SetSelect(By.Id("Input_658"), listing.ListStatus); // Status

            /*driver.WriteTextbox(By.Id("m_txtSourceCommonID"), listing.MLSNum); // fill the MLS number

            driver.FindElement(By.Id("m_lbEdit")).Click();

            driver.FindElement(By.Id("m_dlInputList_ctl00_m_btnSelect")).Click();*/

            #endregion
        }

        private void EditListing(CoreWebDriver driver)
        {
            #region navigateMenu
            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("TopMenuTable")));
            driver.Navigate("http://matrix.harmls.com/Matrix/AddEditInput");
            Thread.Sleep(TimeSpan.FromSeconds(2));

            driver.FindElement(By.LinkText("Edit existing")).Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_lbSearch")));
            driver.WriteTextbox(By.Id("m_txtSourceCommonID"), listing.MLSNum); // fill the MLS number

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lbEdit")));
            Thread.Sleep(TimeSpan.FromSeconds(1));
            driver.FindElement(By.Id("m_lbEdit")).Click();

            #endregion
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

            #region Login
            // Connect to the login page 
            driver.Navigate("https://secure.har.com/login#member");
            Thread.Sleep(100);
            #endregion
            
            driver.wait.Until(x => ((IJavaScriptExecutor)x).ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(100);
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("member_email")));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("member_pass")));
            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("memberloginformsubmit")));

            driver.WriteTextbox(By.Id("member_email"), listing.MarketUsername);
            driver.WriteTextbox(By.Id("member_pass"), listing.MarketPassword);
            driver.ExecuteScript("jQuery('#memberloginformsubmit').click();");
            
            Thread.Sleep(2000);

            try {
                driver.ExecuteScript("jQuery('#popup').hide();jQuery('.modal-backdrop').remove();");
                
            } catch { }

            try {
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.LinkText("ENTER MATRIX MLS")));
            } catch { }
            
            driver.Click(By.LinkText("ENTER MATRIX MLS"), false);

            Thread.Sleep(TimeSpan.FromSeconds(3));
            var window = driver.WindowHandles.FirstOrDefault(c => c == driver.CurrentWindowHandle);
            driver.SwitchTo().Window(window).Close();
            window = driver.WindowHandles.FirstOrDefault();
            driver.SwitchTo().Window(window);
            Thread.Sleep(100);

            return LoginResult.Logged;
        }

        private void startupdate(CoreWebDriver driver)
        {
            driver.FindElement(By.Name("loadeditprp")).Click(); // "Edit" button
            Thread.Sleep(TimeSpan.FromSeconds(2));
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("saveincom")));

        }

        #region Edit Sections

        #region  Edit All Listing data

        private void UpdateListingInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Listing Information")).Click();
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("Input_177_TB")));
            

            driver.SetSelect(By.Id("Input_181"), "EXAGY"); //List Type 

            driver.WriteTextbox(By.Id("Input_182"), listing.ListPrice); //List Price



            if (this.newListing)
            {
                DateTime? listDate = null;
                string linkText = string.Empty;
                switch (listing.ListStatus)
                {
                    case "ACT":
                        listDate = DateTime.Now;
                        break;
                    case "PEND":
                    case "OP":
                    case "PSHO":
                        listDate = DateTime.Now.AddDays(-2);
                        break;
                    case "TERM":
                    case "EXP":
                    case "CLOSD":
                        listDate = DateTime.Now.AddDays(-HusaMarketConstants.ListDateSold);
                        break;
                }

                if (listDate.HasValue)
                {
                    driver.WriteTextbox(By.Id("Input_183"), listDate.Value.ToShortDateString());  // List Date
                }
            }
            else
            {
                if (listing.ListDate != null)
                    driver.WriteTextbox(By.Id("Input_183"), listing.ListDate.Value.ToShortDateString()); // List Date
            }

            // UP-138
            if (this.newListing)
            {
                if (listing.AutopopulateExpirationDate && listing.ExpiredDate != null)
                    driver.WriteTextbox(By.Id("Input_184"), listing.ExpiredDate.Value.ToShortDateString()); // Expiration Date
                else
                    driver.WriteTextbox(By.Id("Input_184"), (listing.SysCreatedOn != null ? listing.SysCreatedOn.Value.AddYears(1).ToShortDateString() : DateTime.Today.AddYears(1).ToShortDateString())); // Expiration Date
            }

            driver.SetSelect(By.Id("Input_185"), "0"); // Also For Lease

            driver.SetSelect(By.Id("Input_186"), "0"); // Priced at Lot Value Only

            if(listing.HurricaneHomeFlooded == "Y")
                driver.SetSelect(By.Id("Input_531"), "YES"); // House Flooded During Harvey
            else
                driver.SetSelect(By.Id("Input_531"), "NO"); // House Flooded During Harvey

            string streetNum = String.IsNullOrEmpty(listing.StreetNumDisplay) ? Convert.ToString(listing.StreetNum) : Convert.ToString(listing.StreetNumDisplay);
            driver.WriteTextbox(By.Id("Input_156"), streetNum); // Street Number

            //driver.SetSelect(By.Id("Input_157"), listing.StreetDir); // St Direction

            driver.WriteTextbox(By.Id("Input_158"), listing.StreetName.Replace(".", "")); // Street Name
            
            driver.WriteTextbox(By.Id("Input_160"), listing.UnitNum); // Unit #

            if (!String.IsNullOrEmpty(listing.City))
            {
                //driver.FindElement(By.Id("Input_161_A")).Click();
                //driver.SwitchTo().Window("Input Field Search");
                //driver.WriteTextbox(By.Id("m_txtSearch"), listing.City);

                String City = textInfo.ToTitleCase(listing.City.ToLower());
                driver.WriteTextbox(By.Id("Input_161_TB"), "");
                driver.FindElement(By.Id("Input_161_TB")).SendKeys(City);
                string js = " jQuery('[data-id=\"" + listing.CityCode + "\"]').click(); ";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }

            driver.SetSelect(By.Id("Input_162"), listing.StateCode); // State

            driver.WriteTextbox(By.Id("Input_163"), listing.Zip); // Zip Code

            driver.SetSelect(By.Id("Input_164"), listing.County); // County

            driver.SetSelect(By.Id("Input_318"), "UNITED"); // Country

            driver.WriteTextbox(By.Id("Input_165"), listing.Subdivision); // Subdivision

            driver.WriteTextbox(By.Id("Input_171"), listing.SectionNum); // Section #

            driver.WriteTextbox(By.Id("Input_302"), listing.Legal); // Legal Description

            //if(!String.IsNullOrEmpty(listing.LegalsubdivisionDisp) && !String.IsNullOrEmpty(listing.Zip))
            //    driver.WriteTextbox(By.Id("Input_320"), listing.LegalsubdivisionDisp + " - " + listing.Zip); // Legal Subdivision
            if (!String.IsNullOrWhiteSpace(listing.LegalsubdivisionDisp))
            {
                // AvailableFields
                if (listing.LegalsubdivisionDisp.Contains("OTHER"))
                {
                    String Legalsubdivision = "OTHER - " + listing.Zip;
                    driver.WriteTextbox(By.Id("Input_320"), Legalsubdivision);
                }
                else
                    driver.WriteTextbox(By.Id("Input_320"), listing.LegalsubdivisionDisp); // Legal Subdivision
            }

            driver.SetSelect(By.Id("Input_172"), listing.IsPlannedDevelopment); // Master Planned Community Y/N


            if(!String.IsNullOrEmpty(listing.MasterPlannedCommunityName)) {
                String MasterPlannedCommunityName = textInfo.ToTitleCase(listing.MasterPlannedCommunityName.ToLower());

                //driver.WriteTextbox(By.Id("Input_173_TB"), listing.PlannedDevelopment );  // Master Planned Community Name
                driver.WriteTextbox(By.Id("Input_173_TB"), ""); // Master Planned Community Name
                driver.FindElement(By.Id("Input_173_TB")).SendKeys(MasterPlannedCommunityName);
                string js = " jQuery('[data-id=\"" + listing.PlannedDevelopment + "\"]').click(); ";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }
            
            if (!String.IsNullOrEmpty(listing.TaxID) && listing.TaxID != "0" && !listing.TaxID.Contains("-0000"))
                driver.WriteTextbox(By.Id("Input_174"), listing.TaxID); // Tax ID #
            else
                driver.WriteTextbox(By.Id("Input_174"), "NA"); // Tax ID #

            driver.WriteTextbox(By.Id("Input_175"), listing.MapscoMapPage); // Key Map

            // driver.WriteTextbox(By.Id("Input_176"), ); // Census Tract

            if (!String.IsNullOrEmpty(listing.SchoolDistrictLongName))
            {
                String SchoolDistrict = textInfo.ToTitleCase(listing.SchoolDistrict.ToLower());
                //driver.SetAttribute(By.Id("Input_177_TB"), listing.SchoolDistrict, "value", true); // School District
                driver.WriteTextbox(By.Id("Input_177_TB"), ""); // School District
                driver.FindElement(By.Id("Input_177_TB")).SendKeys(listing.SchoolDistrictLongName);
                string js = "jQuery('[data-id*=\"" + listing.SchoolDistrict + "\"]').click();";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }
        }

        private void UpdateLeaseInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Listing Information")).Click();
            driver.wait.Until(ExpectedConditions.ElementExists(By.Id("Input_177_TB")));


            driver.SetSelect(By.Id("Input_181"), "EXAGY"); //List Type 

            driver.WriteTextbox(By.Id("Input_182"), listing.ListPrice); //List Price



            if (this.newListing)
            {
                DateTime? listDate = null;
                string linkText = string.Empty;
                switch (listing.ListStatus)
                {
                    case "ACT":
                        listDate = DateTime.Now;
                        break;
                    case "PEND":
                    case "OP":
                    case "PSHO":
                        listDate = DateTime.Now.AddDays(-2);
                        break;
                    case "TERM":
                    case "EXP":
                    case "CLOSD":
                        listDate = DateTime.Now.AddDays(-HusaMarketConstants.ListDateSold);
                        break;
                }

                if (listDate.HasValue)
                {
                    driver.WriteTextbox(By.Id("Input_183"), listDate.Value.ToShortDateString());  // List Date
                }
            }
            else
            {
                if (listing.ListDate != null)
                    driver.WriteTextbox(By.Id("Input_183"), listing.ListDate.Value.ToShortDateString()); // List Date
            }

            // UP-138
            if (this.newListing)
            {
                if (listing.AutopopulateExpirationDate && listing.ExpiredDate != null)
                    driver.WriteTextbox(By.Id("Input_184"), listing.ExpiredDate.Value.ToShortDateString()); // Expiration Date
                else
                    driver.WriteTextbox(By.Id("Input_184"), (listing.SysCreatedOn != null ? listing.SysCreatedOn.Value.AddYears(1).ToShortDateString() : DateTime.Today.AddYears(1).ToShortDateString())); // Expiration Date
            }

            driver.SetSelect(By.Id("Input_388"), listing.PropType); // Property Class

            driver.SetSelect(By.Id("Input_185"), "0"); // Also For Lease

            driver.SetSelect(By.Id("Input_186"), "0"); // Priced at Lot Value Only

            if (listing.HurricaneHomeFlooded == "Y")
                driver.SetSelect(By.Id("Input_531"), "YES"); // House Flooded During Harvey
            else
                driver.SetSelect(By.Id("Input_531"), "NO"); // House Flooded During Harvey

            string streetNum = String.IsNullOrEmpty(listing.StreetNumDisplay) ? Convert.ToString(listing.StreetNum) : Convert.ToString(listing.StreetNumDisplay);
            driver.WriteTextbox(By.Id("Input_156"), streetNum); // Street Number

            //driver.SetSelect(By.Id("Input_157"), listing.StreetDir); // St Direction

            driver.WriteTextbox(By.Id("Input_158"), listing.StreetName); // Street Name

            driver.SetSelect(By.Id("Input_159"), listing.StreetType); // Street Type

            driver.WriteTextbox(By.Id("Input_160"), listing.UnitNum); // Unit #

            if (!String.IsNullOrEmpty(listing.City))
            {
                //driver.FindElement(By.Id("Input_161_A")).Click();
                //driver.SwitchTo().Window("Input Field Search");
                //driver.WriteTextbox(By.Id("m_txtSearch"), listing.City);

                String City = textInfo.ToTitleCase(listing.City.ToLower());
                driver.WriteTextbox(By.Id("Input_161_TB"), "");
                driver.FindElement(By.Id("Input_161_TB")).SendKeys(City);
                string js = " jQuery('[data-id=\"" + listing.CityCode + "\"]').click(); ";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }

            driver.SetSelect(By.Id("Input_162"), listing.StateCode); // State

            driver.WriteTextbox(By.Id("Input_163"), listing.Zip); // Zip Code

            driver.SetSelect(By.Id("Input_164"), listing.County); // County

            driver.SetSelect(By.Id("Input_318"), "UNITED"); // Country

            driver.WriteTextbox(By.Id("Input_165"), listing.Subdivision); // Subdivision

            driver.WriteTextbox(By.Id("Input_171"), listing.SectionNum); // Section #

            driver.WriteTextbox(By.Id("Input_302"), listing.Legal); // Legal Description

            //if(!String.IsNullOrEmpty(listing.LegalsubdivisionDisp) && !String.IsNullOrEmpty(listing.Zip))
            //    driver.WriteTextbox(By.Id("Input_320"), listing.LegalsubdivisionDisp + " - " + listing.Zip); // Legal Subdivision
            if (!String.IsNullOrWhiteSpace(listing.LegalsubdivisionDisp))
            {
                // AvailableFields
                if (listing.LegalsubdivisionDisp.Contains("OTHER"))
                {
                    String Legalsubdivision = "OTHER - " + listing.Zip;
                    driver.WriteTextbox(By.Id("Input_320"), Legalsubdivision);
                }
                else
                    driver.WriteTextbox(By.Id("Input_320"), listing.LegalsubdivisionDisp); // Legal Subdivision
            }

            driver.SetSelect(By.Id("Input_172"), listing.IsPlannedDevelopment); // Master Planned Community Y/N


            if (!String.IsNullOrEmpty(listing.MasterPlannedCommunityName))
            {
                String MasterPlannedCommunityName = textInfo.ToTitleCase(listing.MasterPlannedCommunityName.ToLower());

                //driver.WriteTextbox(By.Id("Input_173_TB"), listing.PlannedDevelopment );  // Master Planned Community Name
                driver.WriteTextbox(By.Id("Input_173_TB"), ""); // Master Planned Community Name
                driver.FindElement(By.Id("Input_173_TB")).SendKeys(MasterPlannedCommunityName);
                string js = " jQuery('[data-id=\"" + listing.PlannedDevelopment + "\"]').click(); ";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }

            if (!String.IsNullOrEmpty(listing.TaxID) && listing.TaxID != "0" && !listing.TaxID.Contains("-0000"))
                driver.WriteTextbox(By.Id("Input_174"), listing.TaxID); // Tax ID #
            else
                driver.WriteTextbox(By.Id("Input_174"), "NA"); // Tax ID #

            driver.WriteTextbox(By.Id("Input_175"), listing.MapscoMapPage); // Key Map

            // driver.WriteTextbox(By.Id("Input_176"), ); // Census Tract

            if (!String.IsNullOrEmpty(listing.SchoolDistrictLongName))
            {
                String SchoolDistrict = textInfo.ToTitleCase(listing.SchoolDistrict.ToLower());
                //driver.SetAttribute(By.Id("Input_177_TB"), listing.SchoolDistrict, "value", true); // School District
                driver.WriteTextbox(By.Id("Input_177_TB"), ""); // School District
                driver.FindElement(By.Id("Input_177_TB")).SendKeys(listing.SchoolDistrictLongName);
                string js = "jQuery('[data-id*=\"" + listing.SchoolDistrict + "\"]').click();";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }
        }

        private void UpdateMapInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            if(this.newListing)
            {
                driver.FindElement(By.LinkText("Map Information")).Click();

                if (listing.Latitude != null)
                    driver.WriteTextbox(By.Id("INPUT__93"), listing.Latitude.Value.ToString("0.000000")); // Latitude 

                if (listing.Longitude != null)
                    driver.WriteTextbox(By.Id("INPUT__94"), listing.Longitude.Value.ToString("0.000000")); // Longitude
            }
        }

        private void UpdatePropertyInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Property Information")).Click();

            driver.WriteTextbox(By.Id("Input_245"), listing.SqFtTotal); // Building SqFt

            driver.SetSelect(By.Id("Input_246"), "BUILD"); // SqFt Source

            driver.WriteTextbox(By.Id("Input_243"), listing.YearBuilt); // Year Built

            //driver.SetSelect(By.Id("Input_244"), listing.YearBuiltSrc); // Year Built Source
            driver.SetSelect(By.Id("Input_244"), "BUILD"); // Year Built Source

            driver.WriteTextbox(By.Id("Input_242"), listing.NumStories); // Stories

            driver.SetSelect(By.Id("Input_247"), "1"); // New Construction (1 , 0)

            driver.SetSelect(By.Id("Input_248"), listing.YearBuiltDesc); // New Construction Desc

            driver.WriteTextbox(By.Id("Input_251"), listing.BuilderName);  // Builder Name

            // driver.WriteTextbox(By.Id("Input_249"), listing.BuildCompletionDate.Value.ToShortDateString()); // Approx Completion Date

            //if (listing.BuildCompletionDate != null)
            //    driver.WriteTextbox(By.Id("Input_301"), listing.BuildCompletionDate.Value.ToShortDateString()); // Completion Date


            if (listing.BuildCompletionDate != null)
            {
                if (listing.YearBuiltDesc == "NVLIV")
                {
                    driver.WriteTextbox(By.Id("Input_249"), "", true, true); // Approx Completion Date
                    driver.WriteTextbox(By.Id("Input_301"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
                else if (listing.YearBuiltDesc == "BEBLT")
                {
                    driver.WriteTextbox(By.Id("Input_301"), "", true, true); // Approx Completion Date
                    driver.WriteTextbox(By.Id("Input_249"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
            }

            driver.SetSelect(By.Id("Input_142"), listing.UtilitiesDesc); // Utility District  (1 , 0)

            driver.WriteTextbox(By.Id("Input_143"), listing.LotSize); // Lot Size

            driver.SetSelect(By.Id("Input_145"), listing.LotSizeSrc); // Lot Size Source
            //UP-175
            //driver.SetSelect(By.Id("Input_145"), "UKNWN"); // Lot Size Source

            // driver.WriteTextbox(By.Id("Input_147"), listing.LotSize); // Acres

            driver.SetSelect(By.Id("Input_148"), listing.Acres); // Acreage

            driver.WriteTextbox(By.Id("Input_144"), listing.LotDim); // Lot Dimensions

            driver.WriteTextbox(By.Id("Input_202"), listing.GarageCapacity); // Garage - Number of Spaces

            driver.WriteTextbox(By.Id("Input_204"), listing.CarportCapacity); // Carport - Number of Spaces

            driver.SetSelect(By.Id("Input_205"), listing.CarportDesc); // Carport - Description

            driver.SetMultipleCheckboxById("Input_207", listing.AccessInstructionsDesc); // Access -- MLS-51 AccessibilityDesc -> AccessInstructionsDesc

            driver.SetMultipleCheckboxById("Input_203", listing.GarageDesc); // Garage Description

            //driver.SetMultipleCheckboxById("Input_206", listing.Restrictions); // Garage/Carport Description

            driver.SetMultipleCheckboxById("Input_152", listing.Restrictions); // Restrictions

            driver.SetMultipleCheckboxById("Input_329", listing.PropSubType); // Property Type

            driver.SetMultipleCheckboxById("Input_241", listing.HousingStyleDesc);  // Style

            driver.SetMultipleCheckboxById("Input_146", listing.LotDesc.ToUpper()); // Lot Description

            driver.SetMultipleCheckboxById("Input_150", listing.WaterfrontDesc); // Waterfront Features

            driver.SetSelect(By.Id("Input_208"), listing.HasMicrowave); // Microwave (0, 1)

            driver.SetSelect(By.Id("Input_209"), listing.HasDishwasher); // Dishwasher

            driver.SetSelect(By.Id("Input_210"), listing.HasDisposal); // Disposal

            driver.WriteTextbox(By.Id("Input_253"), listing.CountertopsDesc); // Countertops

            driver.SetSelect(By.Id("Input_211"), listing.HasCompactor); // Compactor (1, 0)

            driver.SetSelect(By.Id("Input_212"), listing.HasIcemaker); // Separate Ice Maker

            driver.WriteTextbox(By.Id("Input_255"), listing.NumFireplaces); // Fireplace - Number

            driver.SetMultipleCheckboxById("Input_256", listing.FireplaceDesc); // Fireplace Description

            driver.SetMultipleCheckboxById("Input_254", listing.FacesDesc); // Front Door Faces

            driver.SetMultipleCheckboxById("Input_269", listing.OvenDesc); // Oven Type

            driver.SetMultipleCheckboxById("Input_270", listing.RangeDesc);  // Stove Type

            driver.SetMultipleCheckboxById("Input_328", listing.WDConnections); // Washer Dryer Connection

            if (!String.IsNullOrEmpty(listing.GolfCourseFullName))
            {
                String GolfCourseFullName = textInfo.ToTitleCase(listing.GolfCourseFullName.ToLower());
                //driver.SetAttribute(By.Id("Input_151_TB"), listing.SchoolDistrict, "value", true); // School District
                driver.WriteTextbox(By.Id("Input_151_TB"), ""); // School District
                driver.FindElement(By.Id("Input_151_TB")).SendKeys(GolfCourseFullName);
                string js = "jQuery('[data-id*=\"" + listing.GolfCourseName + "\"]').click();";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }
            
            driver.SetSelect(By.Id("Input_265"), listing.HasCommunityPool); // Pool - Area (1, 0)

            driver.SetSelect(By.Id("Input_263"), listing.HasPool); // Pool - Private (1, 0)

            driver.SetMultipleCheckboxById("Input_264", listing.PoolDesc); // Private Pool Description

            driver.SetMultipleCheckboxById("Input_252", listing.InteriorDesc); // Interior Features

            driver.SetMultipleCheckboxById("Input_266", listing.FloorsDesc); // Flooring

            driver.SetMultipleCheckboxById("Input_259", listing.ExteriorDesc); // Exterior Description

            driver.SetMultipleCheckboxById("Input_260", listing.ExteriorFeaturesDesc); // Exterior Construction

            driver.SetMultipleCheckboxById("Input_261", listing.RoofDesc); // Roof Description

            driver.SetMultipleCheckboxById("Input_262", listing.FoundationDesc); // Foundation Description

            driver.SetMultipleCheckboxById("Input_258", listing.EnergyDesc); // Energy Features

            driver.SetMultipleCheckboxById("Input_257", listing.GreenCerts); // Green/Energy Certifications

            driver.SetMultipleCheckboxById("Input_139", listing.HeatSystemDesc); // Heating System Description

            driver.SetMultipleCheckboxById("Input_506", listing.CoolSystemDesc); // Cooling System Description

            driver.SetMultipleCheckboxById("Input_141", listing.SewerDesc); // Water/Sewer Description

            driver.SetMultipleCheckboxById("Input_149", listing.StreetSurfaceDesc); // Street Surface

        }

        private void UpdatePropertyLeaseInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Property Information")).Click();

            driver.WriteTextbox(By.Id("Input_245"), listing.SqFtTotal); // Building SqFt

            driver.SetSelect(By.Id("Input_246"), "BUILD"); // SqFt Source

            driver.WriteTextbox(By.Id("Input_243"), listing.YearBuilt); // Year Built

            //driver.SetSelect(By.Id("Input_244"), listing.YearBuiltSrc); // Year Built Source
            driver.SetSelect(By.Id("Input_244"), "BUILD"); // Year Built Source

            driver.WriteTextbox(By.Id("Input_242"), listing.NumStories); // Stories

            driver.SetSelect(By.Id("Input_247"), "1"); // New Construction (1 , 0)

            driver.SetSelect(By.Id("Input_248"), listing.YearBuiltDesc); // New Construction Desc

            //driver.WriteTextbox(By.Id("Input_251"), listing.BuilderName);  // Builder Name

            // driver.WriteTextbox(By.Id("Input_249"), listing.BuildCompletionDate.Value.ToShortDateString()); // Approx Completion Date

            //if (listing.BuildCompletionDate != null)
            //    driver.WriteTextbox(By.Id("Input_301"), listing.BuildCompletionDate.Value.ToShortDateString()); // Completion Date


            if (listing.BuildCompletionDate != null)
            {
                if (listing.YearBuiltDesc == "NVLIV")
                {
                    driver.WriteTextbox(By.Id("Input_249"), "", true, true); // Approx Completion Date
                    driver.WriteTextbox(By.Id("Input_301"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
                else if (listing.YearBuiltDesc == "BEBLT")
                {
                    driver.WriteTextbox(By.Id("Input_301"), "", true, true); // Approx Completion Date
                    driver.WriteTextbox(By.Id("Input_249"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
            }

            driver.SetSelect(By.Id("Input_142"), listing.UtilitiesDesc); // Utility District  (1 , 0)

            driver.SetSelect(By.Id("Input_421"), listing.Furnished); // Furnished

            driver.SetSelect(By.Id("Input_391"), listing.RentalType); // Rental Type

            driver.WriteTextbox(By.Id("Input_143"), listing.LotSize); // Lot Size

            driver.SetSelect(By.Id("Input_145"), listing.LotSizeSrc); // Lot Size Source
            //UP-175
            //driver.SetSelect(By.Id("Input_145"), "UKNWN"); // Lot Size Source

            // driver.WriteTextbox(By.Id("Input_147"), listing.LotSize); // Acres

            driver.SetSelect(By.Id("Input_148"), listing.Acres); // Acreage

            driver.WriteTextbox(By.Id("Input_144"), listing.LotDim); // Lot Dimensions

            driver.WriteTextbox(By.Id("Input_202"), listing.GarageCapacity); // Garage - Number of Spaces

            driver.WriteTextbox(By.Id("Input_204"), listing.CarportCapacity); // Carport - Number of Spaces

            driver.SetSelect(By.Id("Input_205"), listing.CarportDesc); // Carport - Description

            //driver.SetMultipleCheckboxById("Input_207", listing.AccessInstructionsDesc); // Access -- MLS-51 AccessibilityDesc -> AccessInstructionsDesc

            driver.SetMultipleCheckboxById("Input_203", listing.GarageDesc); // Garage Description

            driver.SetMultipleCheckboxById("Input_206", listing.Restrictions); // Garage/Carport Description

            driver.SetMultipleCheckboxById("Input_152", listing.Restrictions); // Restrictions

            //driver.SetMultipleCheckboxById("Input_329", listing.PropSubType); // Property Type

            driver.SetMultipleCheckboxById("Input_241", listing.HousingStyleDesc);  // Style

            driver.SetMultipleCheckboxById("Input_510", listing.LotDesc); // Lot Description

            driver.SetMultipleCheckboxById("Input_150", listing.WaterfrontDesc); // Waterfront Features

            driver.SetSelect(By.Id("Input_208"), listing.HasMicrowave); // Microwave (0, 1)

            driver.SetSelect(By.Id("Input_209"), listing.HasDishwasher); // Dishwasher

            driver.SetSelect(By.Id("Input_210"), listing.HasDisposal); // Disposal

            driver.WriteTextbox(By.Id("Input_253"), listing.CountertopsDesc); // Countertops

            driver.SetSelect(By.Id("Input_211"), listing.HasCompactor); // Compactor (1, 0)

            driver.SetSelect(By.Id("Input_212"), listing.HasIcemaker); // Separate Ice Maker

            driver.WriteTextbox(By.Id("Input_255"), listing.NumFireplaces); // Fireplace - Number

            driver.SetMultipleCheckboxById("Input_256", listing.FireplaceDesc); // Fireplace Description

            driver.SetMultipleCheckboxById("Input_254", listing.FacesDesc); // Front Door Faces

            driver.SetMultipleCheckboxById("Input_269", listing.OvenDesc); // Oven Type

            driver.SetMultipleCheckboxById("Input_270", listing.RangeDesc);  // Stove Type

            driver.SetMultipleCheckboxById("Input_328", listing.WDConnections); // Washer Dryer Connection

            if (!String.IsNullOrEmpty(listing.GolfCourseFullName))
            {
                String GolfCourseFullName = textInfo.ToTitleCase(listing.GolfCourseFullName.ToLower());
                //driver.SetAttribute(By.Id("Input_151_TB"), listing.SchoolDistrict, "value", true); // School District
                driver.WriteTextbox(By.Id("Input_151_TB"), ""); // School District
                driver.FindElement(By.Id("Input_151_TB")).SendKeys(GolfCourseFullName);
                string js = "jQuery('[data-id*=\"" + listing.GolfCourseName + "\"]').click();";
                ((IJavaScriptExecutor)driver).ExecuteScript(@js);
            }

            driver.SetSelect(By.Id("Input_265"), listing.HasCommunityPool); // Pool - Area (1, 0)

            driver.SetSelect(By.Id("Input_263"), listing.HasPool); // Pool - Private (1, 0)

            driver.SetMultipleCheckboxById("Input_264", listing.PoolDesc); // Private Pool Description

            driver.SetMultipleCheckboxById("Input_511", listing.InteriorDesc); // Interior Features

            driver.SetMultipleCheckboxById("Input_266", listing.FloorsDesc); // Flooring

            driver.SetMultipleCheckboxById("Input_512", listing.ExteriorDesc); // Exterior Description

            //driver.SetMultipleCheckboxById("Input_260", listing.ExteriorFeaturesDesc); // Exterior Construction

            //driver.SetMultipleCheckboxById("Input_261", listing.RoofDesc); // Roof Description

            //driver.SetMultipleCheckboxById("Input_262", listing.FoundationDesc); // Foundation Description

            driver.SetMultipleCheckboxById("Input_514", listing.EnergyDesc); // Energy Features

            driver.SetMultipleCheckboxById("Input_257", listing.GreenCerts); // Green/Energy Certifications

            driver.SetMultipleCheckboxById("Input_139", listing.HeatSystemDesc); // Heating System Description

            driver.SetMultipleCheckboxById("Input_140", listing.CoolSystemDesc); // Cooling System Description

            driver.SetMultipleCheckboxById("Input_513", listing.SewerDesc); // Water/Sewer Description

            driver.SetMultipleCheckboxById("Input_149", listing.StreetSurfaceDesc); // Street Surface

        }

        private void UpdateRooms(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Rooms")).Click();

            if (!driver.UploadInformation.IsNewListing)
            {
                var elems = driver.FindElements(By.CssSelector("table[id^=_Input_191__del_REPEAT] a"));
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
                        //driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink"));
                        //driver.ExecuteScript("Subforms['s_191'].deleteRow('_Input_191__del_REPEAT" + index + "_');");
                        //Thread.Sleep(400);
                        driver.ExecuteScript("jQuery(document).scrollTop(0);");
                        driver.ScrollToTop();
                        continue;
                    }
                }

                /*foreach (var elem in elems.Where(c => c.Displayed))
                {
                    driver.ScrollDown(elem.Location.Y);
                    driver.ExecuteScript("Subforms['s_253'].deleteRow('_Input_253__del_REPEAT" + index  + "_');");
                    Thread.Sleep(400);
                    index++;
                }*/
            }

            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Rooms")).Click();

            driver.WriteTextbox(By.Id("Input_267"), listing.Beds); // Bedrooms

            // driver.WriteTextbox(By.Id("Input_271"), listing.NumBedsMainLevel); // Bedrooms - Max

            driver.WriteTextbox(By.Id("Input_268"), listing.BathsFull); // Baths - Full

            driver.WriteTextbox(By.Id("Input_196"), listing.BathsHalf); // Baths - Half

            driver.SetMultipleCheckboxById("Input_635", listing.BedroomDescription); // Bedroom Description

            driver.SetMultipleCheckboxById("Input_636", listing.RoomDescription); // Room Description

            driver.SetMultipleCheckboxById("Input_637", listing.BedBathDesc); // Bathroom Description

            driver.SetMultipleCheckboxById("Input_634", listing.KitchenDescription); // Kitchen Description

            var roomTypes = ReadRoomAndFeatures(listing);

            //driver.Click(By.Id("m_rpPageList_ctl02_lbPageLink"));
            Thread.Sleep(1000);

            var i = 0;

            //string[] roomsType = (string[]) ((IJavaScriptExecutor)driver).ExecuteScript("return jQuery(\"input[id ^= '_Input_191__REPEAT']\").attr('id');");

            foreach (var roomType in roomTypes)
            {
                if (!String.IsNullOrEmpty(roomType.Dim))
                {
                    if (i > 0)
                    {
                        driver.Click(By.Id("_Input_191_more"));
                        Thread.Sleep(400);
                    }

                    driver.SetSelect(By.Id("_Input_191__REPEAT" + i + "_187"), roomType.Name, true); // Room Type

                    driver.WriteTextbox(By.Id("_Input_191__REPEAT" + i + "_189"), roomType.Dim, true); // Room Dimension 

                    driver.SetSelect(By.Id("_Input_191__REPEAT" + i + "_190"), roomType.Location, true); // Location

                    Thread.Sleep(400);
                    driver.ScrollDown();

                    i++;
                }
            }
        }

        private void UpdateFinancialInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Financial Information")).Click();

            driver.SetSelect(By.Id("Input_275"), listing.HasHOA); // Mandatory HOA/Mgmt Co (1, 0)

            driver.WriteTextbox(By.Id("Input_278"), listing.AssocName); // Mandatory HOA/Mgmt Co Name

            if(!String.IsNullOrEmpty(listing.AssocPhone))
                driver.WriteTextbox(By.Id("Input_276"), listing.AssocPhone.Replace(" ", "-").Replace("(", "").Replace(")", "")); // Mandatory HOA/Mgmt Co Phone

            // driver.WriteTextbox(By.Id("Input_277"), ); // Mandatory HOA/Mgmt Co Website

            //driver.SetMultipleCheckboxById("Input_272", "NODEF");  // Defects

            driver.SetMultipleCheckboxById("Input_273", listing.Disclosures);  // Disclosures

            driver.WriteTextbox(By.Id("Input_274"), listing.Excludes); // Exclusions

            // driver.SetSelect(By.Id("Input_228"), ); // Loss Mitigation (1, 0)

            driver.SetMultipleCheckboxById("Input_280", listing.FinancingProposed); // Financing Considered
            
            driver.SetSelect(By.Id("Input_471"), listing.HOA); // Maintenance Fee
            
            driver.WriteTextbox(By.Id("Input_282"), listing.AssocFee); // Maintenance Fee Amount

            driver.SetSelect(By.Id("Input_283"), listing.AssocFeePaid); // Maintenance Fee Payment Sched

            driver.SetSelect(By.Id("Input_347"), listing.HasOtherFees); // Other Mandatory Fees

            driver.WriteTextbox(By.Id("Input_286"), listing.OtherFees); // Other Mandatory Fees Amount

            driver.WriteTextbox(By.Id("Input_285"), listing.OtherFeesInclude); // Other Mandatory Fees Include

            driver.WriteTextbox(By.Id("Input_290"), listing.TaxYear); // Tax Year

            // driver.WriteTextbox(By.Id("Input_291"), );  // Taxes

            driver.WriteTextbox(By.Id("Input_292"), listing.TaxRate);  // Total Tax Rate

            driver.WriteTextbox(By.Id("Input_293"), listing.ExemptionsDesc); // Exemptions

            driver.SetSelect(By.Id("Input_294"), listing.OwnershipType); // Ownership Type

            // driver.SetSelect(By.Id("Input_294"), ); // Vacation Rental

            // driver.WriteTextbox(By.Id("Input_296"), ); // Seller's Email

            // driver.SetSelect(By.Id("Input_297"), ); // Subject to Auction

            // driver.WriteTextbox(By.Id("Input_298"), ); // Auction Date

            // driver.SetSelect(By.Id("Input_299"), ); // Online Bidding

            // driver.WriteTextbox(By.Id("Input_300"), );  // Bidding Deadline/Review Date
        }

        private void UpdateShowingInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Showing Information")).Click();

            /*var apptPhone = String.Empty;
            if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany))
                apptPhone = listing.AgentListApptPhoneFromCompany;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCommunityProfile))
                apptPhone = listing.AgentListApptPhoneFromCommunityProfile;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhone))
                apptPhone = listing.AgentListApptPhone;
            else if (!String.IsNullOrEmpty(listing.OwnerPhone))
                apptPhone = listing.OwnerPhone;
            driver.WriteTextbox(By.Id("Input_304"), apptPhone.Replace(" ", "-").Replace("(","").Replace(")", ""));*/  // Appointment Desk Phone
            driver.WriteTextbox(By.Id("Input_304"), listing.AgentListApptPhone.Replace(" ", "-").Replace("(", "").Replace(")", ""));  // Appointment Desk Phone

            driver.SetSelect(By.Id("Input_303"), "OFFIC");  // Appointment Phone Desc

            //driver.WriteTextbox(By.Id("Input_234"), ); // Office Phone Ext

            if(!String.IsNullOrEmpty(listing.AltPhoneCommunity))
                driver.WriteTextbox(By.Id("Input_236"), listing.AltPhoneCommunity.Replace(" ", "-").Replace("(", "").Replace(")", "")); // Agent Alternate Phone

            // driver.WriteTextbox(By.Id("Input_237"), ); // Alternate Phone Desc

            // driver.WriteTextbox(By.Id("Input_238"), ); // Night Phone

            // driver.WriteTextbox(By.Id("Input_239"), ); // Fax Phone

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
            driver.WriteTextbox(By.Id("Input_136"), direction, true); // Directions

            driver.SetMultipleCheckboxById("Input_218", listing.ShowingInstructions);  // Showing Instructions

            driver.WriteTextbox(By.Id("Input_327"), listing.CompBuy); // Buyer Agency Compensation

            driver.WriteTextbox(By.Id("Input_226"), "0%"); // Sub Agency Compensation
            
            if (!String.IsNullOrEmpty(listing.Bonus))
                driver.WriteTextbox(By.Id("Input_229"), listing.Bonus); // Bonus
            else
                driver.WriteTextbox(By.Id("Input_229"), ""); // Bonus

            if(listing.BonusEndDate != null)
                driver.WriteTextbox(By.Id("Input_230"), listing.BonusEndDate); // Bonus End Date
            else
                driver.WriteTextbox(By.Id("Input_230"), ""); // Bonus End Date

            driver.SetSelect(By.Id("Input_216"), "0");  // Variable Compensation

        }

        private void UpdateRemarks(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Remarks/Tour Links")).Click();

            // Public Remarks
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
                remark = (builtNote + listing.PublicRemarks);
            }
            else
            {
                var tempIndex = listing.PublicRemarks.IndexOf("~", StringComparison.Ordinal) + 1;
                var temp = listing.PublicRemarks.Substring(tempIndex).Trim();
                remark = (builtNote + temp);
            }
            driver.WriteTextbox(By.Id("Input_135"), remark, true); // Public Remarks

            // Agent Remarks
            String bonusMessage = "";
            if (listing.BonusCheckBox.Equals(true) && listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Bonus & Buyer Incentives; ask Builder for details. ";
            else if (listing.BonusCheckBox.Equals(true))
                bonusMessage = "Possible Bonus; ask Builder for details. ";
            else if (listing.BuyerCheckBox.Equals(true))
                bonusMessage = "Possible Buyer Incentives; ask Builder for details. ";
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
            realtorContactEmail =
                (!String.IsNullOrWhiteSpace(realtorContactEmail) &&
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains("email contact") &&
                !(bonusMessage + listing.GetPrivateRemarks()).ToLower().Contains(realtorContactEmail)) ? "Email contact: " + realtorContactEmail + ". " : "";

            string message = bonusMessage + listing.GetPrivateRemarks(true) + realtorContactEmail;

            string incompletedBuiltNote = "";
            if (listing.YearBuiltDesc == "BEBLT" 
                && !message.Contains("Home is under construction. For your safety, call appt number for showings"))
            {
                incompletedBuiltNote = "Home is under construction. For your safety, call appt number for showings. ";
            }

            driver.WriteTextbox(By.Id("Input_137"), "");
            driver.WriteTextbox(By.Id("Input_137"), incompletedBuiltNote + message, true); // Agent Remarks
            
            driver.WriteTextbox(By.Id("Input_341"), listing.VirtualTourLink); // Virtual Tour Link
        }

        #endregion

        #region Edit Price
        public UploadResult UpdatePrice(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            this.listing = listing;

            Login(driver, listing);

            EditListing(driver);

            driver.wait.Until(ExpectedConditions.ElementExists(By.LinkText("Price Change")));
            driver.FindElement(By.LinkText("Price Change")).Click();

            driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_9")));
            driver.WriteTextbox(By.Id("Input_9"), listing.ListPrice);

            return UploadResult.Success;
        }
        #endregion

        #region Edit Images

        public UploadResult UpdateImages(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;
            this.listing = listing;

            Login(driver, listing);

            EditListing(driver);
            driver.wait.Until(ExpectedConditions.ElementExists(By.LinkText("Manage Photos")));
            driver.FindElement(By.LinkText("Manage Photos")).Click();

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
                //driver.Click(By.Id("m_lbManagePhotos"));

                //Prepare Media
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("m_lbSave")));
                DeleteAllImages(driver);
                UploadNewImages(driver, media.OfType<ResidentialListingMedia>());
            }

            return UploadResult.Success;
        }

        #endregion

        #region Edit Status
        public UploadResult UpdateStatus(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            driver.UploadInformation.IsNewListing = false;
            this.listing = listing;

            Login(driver, listing);
            EditListing(driver);



            //LMDLongName LMDShortName    LMDCode LCode   LShortName LLongName
            //Active ACT ACT ACT Active Active
            //Expired EXP EXP EXP Expired Expired
            //Option Pending  OP OP  PND Pending Pending
            //Pending PEND PEND    PND Pending Pending
            //Pending Continue to Show PSHO    PSHO PCS Pending Showing Pending Showing
            //Sold CLOSD   CLOSD SLD Sold Sold
            //Terminated TERM    TERM CAN Cancelled Cancelled
            //Withdrawn WITH    WITH TOM Temp Off Market Temporarily Off Market



            //  var expirationDate = (listing.ListDate.HasValue ? listing.ListDate.Value : DateTime.Now.Date).AddYears(1);
            Thread.Sleep(1500);

            switch (listing.ListStatus)
            {
                #region Sold
                case "CLOSD":

                    driver.Click(By.LinkText("Change to Sold"));
                    Thread.Sleep(500);

                    driver.WriteTextbox(By.Id("Input_74"), listing.SalesPrice); // 	Sale Price

                    if (listing.PendingDate != null)
                        driver.WriteTextbox(By.Id("Input_321"), listing.PendingDate.Value.ToShortDateString()); // Pending Date
                     
                    if (listing.ClosedDate != null)
                        driver.WriteTextbox(By.Id("Input_120"), listing.ClosedDate.Value.ToShortDateString()); // Closed Date

                    driver.SetSelect(By.Id("Input_119"), "0"); // Coop Sale

                    driver.WriteTextbox(By.Id("Input_121"), listing.SellerPaid); // Seller Pd Buyer Clsg Costs
                    driver.WriteTextbox(By.Id("Input_123"), "0"); // Repair Paid Seller

                    // RL Data Form:
                    // Both
                    // None
                    // Buyer
                    // Seller
                    driver.SetSelect(By.Id("Input_122"), listing.TitlePaidBy); // Title Paid By
                    if (listing.SellingAgentPresent != null && listing.SellingAgentPresent == true)
                        driver.SetSelect(By.Id("Input_310"), "Y");  // Did Selling Agent Represent Buyer
                    else
                        driver.SetSelect(By.Id("Input_310"), "N");  // Did Selling Agent Represent Buyer

                    driver.SetSelect(By.Id("Input_525"), listing.SoldTerms); // Sold Terms
                                                                             // driver.WriteTextbox(By.Id("Input_312"), ""); // Buyer Email
                                                                             // driver.WriteTextbox(By.Id("Input_315"), ""); // Seller Email

                    if (!String.IsNullOrEmpty(listing.SellingAgentUID))
                    {
                        driver.WriteTextbox(By.Id("Input_342"), listing.SellingAgentUID); // Selling Agent MLSID

                        string js = " document.getElementById('Input_342_Refresh').value='1';RefreshToSamePage(); ";
                        ((IJavaScriptExecutor)driver).ExecuteScript(@js);
                    }

                    if (!String.IsNullOrEmpty(listing.SellingAgentLicenseNum) && listing.SellingAgentLicenseNum != "NONMLS")
                    {
                        driver.ScrollDown();
                        driver.SetSelect(By.Id("Input_124"), "0"); // Buyer Represented by NONMLS Licensed Agent
                        driver.WriteTextbox(By.Id("Input_125"), listing.SellingAgentLicenseNum); // TREC License Number
                    }
                    
                    break;

                #endregion

                #region Pending
                case "PEND":
                    //driver.Click(By.Id("m_dlInputList_ctl05_m_btnSelect"));
                    driver.Click(By.LinkText("Change to Pending"));
                    Thread.Sleep(500);

                    if (listing.PendingDate != null)
                        driver.WriteTextbox(By.Id("Input_321"), listing.PendingDate.Value.ToShortDateString()); // Pending Date

                    if (listing.EstClosedDate != null)
                        driver.WriteTextbox(By.Id("Input_311"), listing.EstClosedDate.Value.ToShortDateString()); // Estimated Closed Date

                    if (listing.SellingAgentPresent != null && listing.SellingAgentPresent == true)
                        driver.SetSelect(By.Id("Input_310"), "Y");  // Did Selling Agent Represent Buyer
                    else
                        driver.SetSelect(By.Id("Input_310"), "N");  // Did Selling Agent Represent Buyer

                    if (!String.IsNullOrEmpty(listing.ContingencyInfo))
                        driver.SetSelect(By.Id("Input_132"), "1"); // Contingent on Sale of Other Property
                    else
                        driver.SetSelect(By.Id("Input_132"), "0"); // Contingent on Sale of Other Property

                    // driver.WriteTextbox(By.Id("Input_314"), ""); // Buyer Email
                    // driver.WriteTextbox(By.Id("Input_315"), ""); // Seller Email

                    if (!String.IsNullOrEmpty(listing.SellingAgentUID))
                    {
                        driver.WriteTextbox(By.Id("Input_342"), listing.SellingAgentUID); // Selling Agent MLSID

                        string js = " document.getElementById('Input_342_Refresh').value='1';RefreshToSamePage(); ";
                        ((IJavaScriptExecutor)driver).ExecuteScript(@js);
                    }

                    if (!String.IsNullOrEmpty(listing.SellingAgentLicenseNum) && listing.SellingAgentLicenseNum != "NONMLS")
                    {
                        driver.ScrollDown();
                        driver.SetSelect(By.Id("Input_528"), "0"); // Buyer Represented by NONMLS Licensed Agent
                        driver.WriteTextbox(By.Id("Input_131"), listing.SellingAgentLicenseNum); // TREC License Number
                    }

                    break;
                #endregion

                #region Option Pending
                case "OP":
                    //driver.Click(By.Id("m_dlInputList_ctl03_m_btnSelect"));
                    driver.Click(By.LinkText("Change to Option Pending"));
                    Thread.Sleep(500);

                    if (listing.PendingDate != null)
                        driver.WriteTextbox(By.Id("Input_83"), listing.PendingDate.Value.ToShortDateString()); // Pending Date

                    if (listing.EstClosedDate != null)
                        driver.WriteTextbox(By.Id("Input_128"), listing.EstClosedDate.Value.ToShortDateString()); // Estimated Closed Date

                    if (listing.ExpiredDateOption != null)
                        driver.WriteTextbox(By.Id("Input_129"), listing.ExpiredDateOption.Value.ToShortDateString()); // Option End Date

                    if (listing.SellingAgentPresent != null && listing.SellingAgentPresent == true)
                        driver.SetSelect(By.Id("Input_310"), "Y");  // Did Selling Agent Represent Buyer
                    else
                        driver.SetSelect(By.Id("Input_310"), "N");  // Did Selling Agent Represent Buyer
                    
                    if( !String.IsNullOrEmpty(listing.ContingencyInfo))
                        driver.SetSelect(By.Id("Input_132"), "1"); // Contingent on Sale of Other Property
                    else
                        driver.SetSelect(By.Id("Input_132"), "0"); // Contingent on Sale of Other Property

                    // driver.WriteTextbox(By.Id("Input_317"), ""); // Buyer Email
                    // driver.WriteTextbox(By.Id("Input_316"), ""); // Seller Email

                    if (!String.IsNullOrEmpty(listing.SellingAgentUID))
                    {
                        driver.WriteTextbox(By.Id("Input_342"), listing.SellingAgentUID); // Selling Agent MLSID

                        string js = " document.getElementById('Input_342_Refresh').value='1';RefreshToSamePage(); ";
                        ((IJavaScriptExecutor)driver).ExecuteScript(@js);
                    }

                    if (!String.IsNullOrEmpty(listing.SellingAgentLicenseNum) && listing.SellingAgentLicenseNum != "NONMLS")
                    {
                        driver.ScrollDown();
                        driver.SetSelect(By.Id("Input_130"), "0"); // Buyer Represented by NONMLS Licensed Agent
                        driver.WriteTextbox(By.Id("Input_131"), listing.SellingAgentLicenseNum); // TREC License Number
                    }

                    break;

                #endregion

                #region Pending Continue to Show

                case "PSHO":
                    //driver.Click(By.Id("m_dlInputList_ctl04_m_btnSelect"));
                    driver.Click(By.LinkText("Change to Pending Continue to Show"));
                    Thread.Sleep(500);

                    if (listing.PendingDate != null)
                        driver.WriteTextbox(By.Id("Input_83"), listing.PendingDate.Value.ToShortDateString()); // Pending Date

                    if (listing.EstClosedDate != null)
                        driver.WriteTextbox(By.Id("Input_128"), listing.EstClosedDate.Value.ToShortDateString()); // Estimated Closed Date

                    if (listing.SellingAgentPresent != null && listing.SellingAgentPresent == true)
                        driver.SetSelect(By.Id("Input_310"), "Y");  // Did Selling Agent Represent Buyer
                    else
                        driver.SetSelect(By.Id("Input_310"), "N");  // Did Selling Agent Represent Buyer
                    //driver.SetSelect(By.Id("Input_132"), ""); // Contingent on Sale of Other Property

                    // driver.WriteTextbox(By.Id("Input_314"), ""); // Buyer Email
                    // driver.WriteTextbox(By.Id("Input_315"), ""); // Seller Email

                    if (!String.IsNullOrEmpty(listing.SellingAgentUID))
                    {
                        driver.WriteTextbox(By.Id("Input_342"), listing.SellingAgentUID); // Selling Agent MLSID

                        string js = " document.getElementById('Input_342_Refresh').value='1';RefreshToSamePage(); ";
                        ((IJavaScriptExecutor)driver).ExecuteScript(@js);
                    }

                    if (!String.IsNullOrEmpty(listing.SellingAgentLicenseNum) && listing.SellingAgentLicenseNum != "NONMLS")
                    {
                        driver.ScrollDown();
                        driver.SetSelect(By.Id("Input_130"), "0"); // Buyer Represented by NONMLS Licensed Agent
                        driver.WriteTextbox(By.Id("Input_131"), listing.SellingAgentLicenseNum); // TREC License Number
                    }
                    
                    break;

                #endregion

                #region Active

                case "ACT":
                    driver.Click(By.LinkText("Change to Active"));
                    Thread.Sleep(500);
                    if (listing.ExpiredDate != null)
                        driver.WriteTextbox(By.Id("Input_5"), listing.ExpiredDate.Value.ToShortDateString());

                    break;
                #endregion

                #region Withdrawn

                case "WITH":
                    driver.Click(By.LinkText("Change to Withdrawn"));
                    Thread.Sleep(500);
                    if (listing.WithdrawnDate != null)
                        driver.WriteTextbox(By.Id("Input_113"), listing.WithdrawnDate.Value.ToShortDateString());

                    break;
                #endregion

                #region Cancelled / Expired

                case "TERM":
                    driver.Click(By.LinkText("Change to Terminated"));
                    Thread.Sleep(500);
                    //if(listing.ExpiredDate != null)
                    driver.WriteTextbox(By.Id("Input_522"), DateTime.Now.ToShortDateString());

                    break;
                case "EXP":
                    driver.Click(By.LinkText("Change Expiration Date"));
                    Thread.Sleep(500);
                    //if(listing.ExpiredDate != null)
                    driver.WriteTextbox(By.Id("Input_8"), listing.ExpiredDate.Value.ToShortDateString());

                    break;
                #endregion

                default:
                    throw new ArgumentOutOfRangeException("listing.ListingStatus", listing.ListStatus, "Invalid Status for Houston Listing with Id '" + listing.ResidentialListingID + "'");
            }

            return UploadResult.Success;
        }
        #endregion

        #region Edit Completion Date
        /// <summary>
        /// Updates a listing's completion date in the DFW MLS system.
        /// </summary>
        /// <param name="driver">The Selenium Web Driver (wrapped in a CoreWebDriver instance) that is to be used to drive the web browser</param>
        /// <param name="listing">The listing to upload</param>
        /// <returns>The final status of the Completion Date update operation and whether it succeeded or not</returns>
        public UploadResult UpdateCompletionDate(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            this.listing = listing;
            this.newListing = (listing.MLSNum == null) ? true : false;

            Login(driver, listing);
            EditListing(driver);

            driver.wait.Until(ExpectedConditions.ElementExists(By.LinkText("Single-Family Add/Edit")));
            driver.FindElement(By.LinkText("Single-Family Add/Edit")).Click();

            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Property Information")).Click();

            driver.SetSelect(By.Id("Input_248"), listing.YearBuiltDesc); // New Construction Desc

            if (listing.BuildCompletionDate != null)
            {
                if (listing.YearBuiltDesc == "NVLIV")
                {
                    driver.WriteTextbox(By.Id("Input_249"), "", true, true); // Approx Completion Date
                    driver.WriteTextbox(By.Id("Input_301"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
                else if(listing.YearBuiltDesc == "BEBLT")
                {
                    driver.WriteTextbox(By.Id("Input_301"), "", true, true); // Approx Completion Date
                    driver.WriteTextbox(By.Id("Input_249"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); // Completion Date
                }
            }
            
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
                if (!String.IsNullOrEmpty(listing.RemarksFormatFromCompany) && listing.RemarksFormatFromCompany == "SD")
                {
                    builtNote += "Built by " + listing.CompanyName + " - CONST. COMPLETED " + listing.BuildCompletionDate.Value.ToString(dateFormat) + " ~ ";
                }
                else
                {
                    builtNote += "Built by " + listing.CompanyName + " - Ready Now! ~ ";
                }
            }
            else if (listing.YearBuiltDesc == "BEBLT")
            {

                if (listing.BuildCompletionDate != null)
                    builtNote += "Built by " + listing.CompanyName + " - " + listing.BuildCompletionDate.Value.ToString("MMMM") + " completion! ~ ";

            }

            if (listing.IncludeRemarks != null && listing.IncludeRemarks == false)
                builtNote = "";

            if (!listing.PublicRemarks.Contains("~"))
            {
                remark = (builtNote + listing.PublicRemarks);
            }
            else
            {
                var tempIndex = listing.PublicRemarks.IndexOf("~", StringComparison.Ordinal) + 1;
                var temp = listing.PublicRemarks.Substring(tempIndex).Trim();
                remark = (builtNote + temp);
            }

            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Remarks/Tour Links")).Click();

            driver.WriteTextbox(By.Id("Input_135"), remark, true); //Physical Property Desc - Public

            return UploadResult.Success;
        }
        #endregion

        private void UpdateLeaseAndAdditionalInfo(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Lease and Additional Information")).Click();

            Thread.Sleep(1000);

            driver.SetSelect(By.Id("Input_397"), listing.SmokingAllowed); // 	Is Smoking Allowed

            driver.SetSelect(By.Id("Input_398"), listing.Pets); // Are Pets Allowed

            if(!String.IsNullOrEmpty(listing.NonRefunPetFee))
                driver.SetSelect(By.Id("Input_417"), listing.NonRefunPetFee == "True" ? 1 : 0); // Pet Deposit Required

            driver.SetSelect(By.Id("Input_400"), listing.ApprovalRequired); // Is Approval Required

            driver.WriteTextbox(By.Id("Input_402"), listing.AppFeeAmount); // Application Fee

            if (!String.IsNullOrEmpty(listing.AppFee))
                driver.SetSelect(By.Id("Input_347"), listing.AppFee == "True" ? 1 : 0);  // Other Mandatory Fees

            driver.WriteTextbox(By.Id("Input_286"), listing.DepositAmount); // Other Mandatory Fees Amount

            driver.WriteTextbox(By.Id("Input_406"), listing.Date); // Date Available

            driver.SetSelect(By.Id("Input_414"), listing.ManagementCoYN); // Property Mgmt Co

            driver.WriteTextbox(By.Id("Input_278"), listing.ManagementCompany);  // Property Mgmt Co Name

            if (!String.IsNullOrEmpty(listing.PhoneMgmtCo))
                driver.WriteTextbox(By.Id("Input_415"), listing.PhoneMgmtCo.Replace("(", "").Replace(")", "").Replace(" ", "-"));  // Property Mgmt Co Phone

            driver.SetMultipleCheckboxById("Input_422", listing.ExemptionsDesc); // Disclosures

            driver.SetMultipleCheckboxById("Input_410", listing.RentalTerms); // Rental Terms

        }

        private void UpdateLeaseShowingInformation(CoreWebDriver driver)
        {
            driver.ScrollToTop();
            driver.FindElement(By.LinkText("Showing Information")).Click();

            /*var apptPhone = String.Empty;
            if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany))
                apptPhone = listing.AgentListApptPhoneFromCompany;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCommunityProfile))
                apptPhone = listing.AgentListApptPhoneFromCommunityProfile;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhone))
                apptPhone = listing.AgentListApptPhone;
            else if (!String.IsNullOrEmpty(listing.OwnerPhone))
                apptPhone = listing.OwnerPhone;
            driver.WriteTextbox(By.Id("Input_304"), apptPhone.Replace(" ", "-").Replace("(","").Replace(")", ""));*/  // Appointment Desk Phone
            if(!String.IsNullOrEmpty(listing.AgentListApptPhone))
                driver.WriteTextbox(By.Id("Input_304"), listing.AgentListApptPhone.Replace("(", "").Replace(")","").Replace(" ", "-"));  // Appointment Desk Phone

            driver.SetSelect(By.Id("Input_303"), "OFFIC");  // Appointment Phone Desc

            //driver.WriteTextbox(By.Id("Input_234"), ); // Office Phone Ext

            if (!String.IsNullOrEmpty(listing.AltPhoneCommunity))
                driver.WriteTextbox(By.Id("Input_236"), listing.AltPhoneCommunity.Replace("(", "").Replace(")", "").Replace(" ", "-"));  // Agent Alternate Phone

            // driver.WriteTextbox(By.Id("Input_237"), ); // Alternate Phone Desc

            // driver.WriteTextbox(By.Id("Input_238"), ); // Night Phone

            // driver.WriteTextbox(By.Id("Input_239"), ); // Fax Phone

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
            driver.WriteTextbox(By.Id("Input_136"), direction, true); // Directions

            driver.SetMultipleCheckboxById("Input_218", listing.ShowingInstructions);  // Showing Instructions

            //driver.WriteTextbox(By.Id("Input_327"), listing.CompBuy); // Buyer Agency Compensation

            driver.WriteTextbox(By.Id("Input_226"), "0%"); // Sub Agency Compensation

            driver.WriteTextbox(By.Id("Input_227"), listing.CommissionLease); // Bonus

            if (!String.IsNullOrEmpty(listing.Bonus))
                driver.WriteTextbox(By.Id("Input_229"), listing.Bonus); // Bonus
            else
                driver.WriteTextbox(By.Id("Input_229"), ""); // Bonus

            if (listing.BonusEndDate != null)
                driver.WriteTextbox(By.Id("Input_230"), listing.BonusEndDate); // Bonus End Date
            else
                driver.WriteTextbox(By.Id("Input_230"), ""); // Bonus End Date

            driver.SetSelect(By.Id("Input_216"), "0");  // Variable Compensation

        }

        #endregion

        #region Rooms Tab

        private IEnumerable<RoomType> ReadRoomAndFeatures(ResidentialListingRequest listing)
        {
            return new List<RoomType>()
            {
                #region Master Bedroom
                new RoomType("MBEDR", listing.Bed1Dim, listing.Bed1Location),
                #endregion

                #region Master Bath
                new RoomType("MBATH", listing.Bath1Dim, listing.Bath1Location),
                #endregion

                #region Bedroom 2
                new RoomType("BERDR", listing.Bed2Dim, listing.Bed2Location),
                #endregion

                #region Bedroom 3
                new RoomType("BERDR", listing.Bed3Dim, listing.Bed3Location),
                #endregion

                #region Bedroom 4
                new RoomType("BERDR", listing.Bed4Dim, listing.Bed4Location),
                #endregion

                #region Bedroom 5
                new RoomType("BERDR", listing.Bed5Dim, listing.Bed5Location),
                #endregion

                #region Bedroom 6
                new RoomType("BERDR", listing.Bed6Dim, listing.Bath1Desc),
                #endregion

                #region LivingRoom 1
                new RoomType("LIVRM", listing.LivingRoom1Dim, listing.LivingRoomLocation),
                #endregion

                #region LivingRoom 2
                new RoomType("DENNN", listing.LivingRoom2Dim, listing.DenLocation),
                #endregion

                #region GameRoom
                new RoomType("GAMER", listing.OtherRoom2Dim, listing.GameroomLocation),
                #endregion

                #region MediaRoom
                new RoomType("MEDIA", listing.MediaRoomDim, listing.MediaRoomLocation),
                #endregion

                #region ExtraRooom
                new RoomType("EXTRA", listing.OtherRoom1Dim, listing.ExtraRoomLocation),
                #endregion

                #region DinningRoom
                new RoomType("DINIR", listing.DiningRoomDim, listing.DiningLocation),
                #endregion

                #region BreakfastRoom
                new RoomType("BRKFS", listing.BreakfastDim, listing.BreakfastLocation),
                #endregion

                #region KitchenRoom
                new RoomType("KITCH", listing.KitchenDim, listing.KitchenLocation),
                #endregion

                #region StudyRoom
                new RoomType("STUDY", listing.StudyDim, listing.StudyLocation),
                #endregion

                #region UtilityRoom
                new RoomType("UTILY", listing.UtilityRoomDim, listing.UtilityLocation),
                #endregion
            };
        }

        private class RoomType
        {
            internal readonly string Name;
            internal readonly string Dim;
            internal readonly string Location;

            public RoomType(String name, String dim, String location)
            {
                Name = name;
                Dim = dim;
                Location = location;
            }

            //public bool IsValid()
            //{
            //    return !string.IsNullOrWhiteSpace(Level) && !string.IsNullOrWhiteSpace(Length);
            //}
        }

        #endregion

        #region Media Manage 
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
            foreach (var image in media)
            {
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

                string descriptionFieldId = js.ExecuteScript("return jQuery('#photoCell_" + i + " > div > table > tbody > tr:nth-child(3) > td > textarea').attr('id');").ToString();

                driver.WriteTextbox(By.Id(descriptionFieldId), image.Caption);
                driver.FindElement(By.Id(descriptionFieldId)).SendKeys(Keys.Enter);

                i++;
            }
        }
        #endregion

        #region Open House
        public UploadResult UpdateOpenHouse(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            this.listing = listing;
            driver.UploadInformation.IsNewListing = false;

            Login(driver, listing);

            Thread.Sleep(1000);

            EditListing(driver);

            Thread.Sleep(1000);

            driver.Click(By.LinkText("Add Open Houses"), false, false);

            Thread.Sleep(1000);

            this.DeleteOpenHouses(driver, listing);

            Thread.Sleep(1000);

            if (listing.EnableOpenHouse && listing.AgreeOpenHouseConditions)
            {
                AddOpenHouses(driver, listing);
                driver.ScrollDown();
                Thread.Sleep(2000);
            } 
            
            return UploadResult.Success;
        }

        public void AddOpenHouses(CoreWebDriver driver, ResidentialListingRequest listing)
        {
            int maxDays = 9;
            List<DateTime> dateToUpdate = OH.getDatesToUpdate(maxDays);
            int i = 0;

            // HCS-596
            String openHouseType = "Public";


            Thread.Sleep(1000);
            driver.ScrollDown();
            foreach (DateTime local in dateToUpdate)
            {
                string[] openhousestart;
                string[] openhouseend;

                string day = local.DayOfWeek.ToString().Substring(0, 3);
                if (listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null) != null && listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null) != null)
                {
                    openhousestart = this.OH.GetOpenHouseTime(listing.GetType().GetProperty("OHStartTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.START, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);
                    openhouseend = this.OH.GetOpenHouseTime(listing.GetType().GetProperty("OHEndTime" + day).GetValue(listing, null).ToString(), TypeOpenHouseHour.END, (listing.ChangeOpenHouseHours != null && true.Equals(listing.ChangeOpenHouseHours)) ? true : false);

                    // 	Date
                    driver.WriteTextbox(By.Id("_Input_337__REPEAT" + i + "_332"), local.ToShortDateString(), false, false, false, false);

                    // Start Time
                    driver.WriteTextbox(By.Id("_Input_337__REPEAT" + i + "_TextBox_333"), openhousestart[0], false, false, false, false);
                    if (openhousestart[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id("_Input_337__REPEAT" + i + "_RadioButtonList_333_0"), openhousestart[1], false);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id("_Input_337__REPEAT" + i + "_RadioButtonList_333_1"), openhousestart[1], false);
                    }

                    // End Time
                    driver.WriteTextbox(By.Id("_Input_337__REPEAT" + i + "_TextBox_334"), openhouseend[0], false, false, false, false);
                    if (openhouseend[1] == "AM")
                    {
                        driver.SetRadioButton(By.Id("_Input_337__REPEAT" + i + "_RadioButtonList_334_0"), openhouseend[1], false);
                    }
                    else
                    {
                        driver.SetRadioButton(By.Id("_Input_337__REPEAT" + i + "_RadioButtonList_334_1"), openhouseend[1], false);
                    }

                    // Open House Type
                    driver.SetSelect(By.Id("_Input_337__REPEAT" + i + "_330"), openHouseType, false);

                    // Comments
                    if (listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null) != null)
                    {
                        driver.WriteTextbox(By.Id("_Input_337__REPEAT" + i + "_339"), listing.GetType().GetProperty("OHComments" + day).GetValue(listing, null), false);
                    }

                    i++;
                    int countButtons = driver.FindElements(By.LinkText("Delete")).Count();
                    if (maxDays >= (i + 1) && (countButtons < (i + 1)))
                    {
                        try
                        {
                            if (driver.FindElements(By.Id("_Input_337__REPEAT" + i + "_330")).Count() <= 0)
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
            driver.ScrollDown();
            Thread.Sleep(2000);
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
                    try { driver.ExecuteScript("Subforms['s_337'].deleteRow('_Input_337__del_REPEAT" + i + "_');"); } catch { }

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

        #endregion Open House

        #region Virtual Tour

        public UploadResult UploadVirtualTour(CoreWebDriver driver, ResidentialListingRequest listing, IEnumerable<IListingMedia> media)
        {
            driver.UploadInformation.IsNewListing = false;
            this.listing = listing;

            Login(driver, listing);

            EditListing(driver);
            driver.wait.Until(ExpectedConditions.ElementExists(By.LinkText("Manage Tours Links")));
            driver.FindElement(By.LinkText("Manage Tours Links")).Click();

            Thread.Sleep(1000);

            var virtualTour = media.OfType<ResidentialListingVirtualTour>().ToList();
            if (virtualTour != null && virtualTour.Count > 0)
            {
                driver.WriteTextbox(By.Id("Input_341"), virtualTour[0].VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded
                if (virtualTour.Count > 1)
                {
                    driver.WriteTextbox(By.Id("Input_532"), virtualTour[1].VirtualTourAddress.Replace("http://", "").Replace("https://", "")); // Virtual Tour URL Unbranded
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
            driver.Navigate("https://secure.har.com/login/dologout");
            return UploadResult.Success;
        }

        public UploadResult UpdateLease(CoreWebDriver driver, ResidentialListingRequest listing, Lazy<IEnumerable<IListingMedia>> media)
        {
            //driver.UploadInformation.IsNewListing = string.IsNullOrWhiteSpace(listing.MLSNum);

            //Login(driver, listing);

            //if (driver.UploadInformation.IsNewListing)
            //{
            //    StartInsertLeasing(driver);
            //}
            //else
            //{
            //    StartUpdate(driver, listing);
            //    driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl02_m_divFooterContainer")));
            //    driver.Click(By.Id("m_dlInputList_ctl00_m_btnSelect")); // Tab: Input | Option: Residential
            //}

            //EditLeasingGeneralTab(driver, listing);
            //EditRoomsTab(driver, listing);
            //EditFeaturesLeasingTab(driver, listing);
            //EditLotUtilityEnvironmentTab(driver, listing);
            //EditFinancialLeasingTab(driver, listing);
            //EditShowingLeasingTab(driver, listing);
            //EditRemarksLeaseTab(driver, listing);

            //return UploadResult.Success;
            //this.wait = new WebDriverWait(driver, new TimeSpan(0, 0, 20));
            this.listing = listing;
            this.newListing = (listing.MLSNum == null) ? true : false;

            Login(driver, listing);

            if (this.newListing)
            {
                startInsert(driver);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("TopMenuTable")));
                driver.Navigate("http://matrix.harmls.com/Matrix/AddEditInput");
                Thread.Sleep(TimeSpan.FromSeconds(2));

                driver.FindElement(By.LinkText("Add new")).Click();
                Thread.Sleep(TimeSpan.FromSeconds(2));

                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_dlInputList_ctl06_m_btnSelect")));
                driver.FindElement(By.Id("m_dlInputList_ctl06_m_btnSelect")).Click();
                driver.wait.Until(ExpectedConditions.ElementExists(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")));
                driver.FindElement(By.Id("m_rpFillFromList_ctl03_m_lbPageLink")).Click();

                driver.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("Input_658")));
                driver.SetSelect(By.Id("Input_658"), listing.ListStatus); // Status
            }
            else
            {
                EditListing(driver);
                driver.wait.Until(ExpectedConditions.ElementExists(By.LinkText("Single-Family Add/Edit")));
                driver.FindElement(By.LinkText("Single-Family Add/Edit")).Click();
            }

            // 1. Listing Information
            UpdateLeaseInformation(driver);

            // 2. Map Information
            UpdateMapInformation(driver);

            // 3. Property Information
            UpdatePropertyLeaseInformation(driver);

            // 4. Rooms
            UpdateRooms(driver);

            // 5. Financial Information
            UpdateLeaseAndAdditionalInfo(driver);

            // 6. Showing Information
            UpdateLeaseShowingInformation(driver);

            // 7. Remarks
            UpdateRemarks(driver);

            return UploadResult.Success;
        }
    }
}
