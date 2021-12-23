using Husa.Core.UploaderBase;
using OpenQA.Selenium;
using System;

// ReSharper disable UnusedMember.Global

namespace Husa.Core.Uploaders.Houston
{
    public partial class HoustonUploader : ICompletionDateUploader
    {
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
            startupdate(driver);

            driver.SetSelect(By.Name("NewConstructionDesc"), listing.YearBuiltDesc); //New Construction Description
            driver.WriteTextbox(By.Name("CompletionDate"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); //Completed Construction Date
            if (listing.YearBuiltDesc == "NVLIV")
                driver.WriteTextbox(By.Name("CompletedConstructionDate"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); //Completed Construction Date
            else
                driver.WriteTextbox(By.Name("CompletionDate"), listing.BuildCompletionDate.Value.ToShortDateString(), true, true); //Apprx Completion Date

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
                remark = (builtNote + listing.PublicRemarks).RemoveSlash();
            }
            else
            {
                var tempIndex = listing.PublicRemarks.IndexOf("~", StringComparison.Ordinal) + 1;
                var temp = listing.PublicRemarks.Substring(tempIndex).Trim();
                remark = (builtNote + temp).RemoveSlash();
            }

            driver.WriteTextbox(By.Name("Remarks"), remark, true); //Physical Property Desc - Public

            return UploadResult.Success;
        }
    }
}