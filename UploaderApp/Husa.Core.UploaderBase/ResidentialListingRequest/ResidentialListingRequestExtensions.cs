using System;
using System.Linq;
using Serilog;

namespace Husa.Core.UploaderBase
{
    public static class ResidentialListingRequestExtensions
    {
        public static string RemoveSlash(this string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                return p;

            if (p.Contains('/'))
                p = p.Replace('/', '-');
            else if (p.Contains('\\'))
                p = p.Replace('\\', '-');
            else if (p.Contains('"'))
                p = p.Replace("\"", "");
            return p;
        }
        public static string GetPrivateRemarks(this ResidentialListingRequest listing, bool useExtendedRemarks = true, bool addPlanName = true)
        {
            var privateRemarks = @"LIMITED SERVICE LISTING: Buyer verifies dimensions & ISD info. Use Bldr contract. ";

            var extendedRemarks = string.Empty;

            if (useExtendedRemarks)
                extendedRemarks = GetExtendedRemarks(listing);

            var fieldRemarks = extendedRemarks + privateRemarks.Replace("\"", "");

            if (addPlanName)
            {
                const string prex = "Plan: ";
                fieldRemarks = fieldRemarks.TrimEnd() + " " + prex + listing.PlanProfileName + ". ";
            }

            return fieldRemarks;
        }

        public static string GetPublicRemarks(this ResidentialListingRequest listing, BuiltStatus status)
        {
            var builtNote = "MLS# " + listing.MLSNum;

            if (!string.IsNullOrWhiteSpace(listing.CompanyName))
            {
                if (!string.IsNullOrWhiteSpace(builtNote))
                {
                    builtNote += " - ";
                }

                builtNote += "Built by " + listing.CompanyName + " - ";
            }

            switch (status)
            {
                case BuiltStatus.ToBeBuilt:
                    builtNote += "To Be Built! ~ ";
                    break;

                case BuiltStatus.ReadyNow:
                    String dateFormat = "MMM dd";
                    int diffDays = DateTime.Now.Subtract((DateTime)listing.BuildCompletionDate).Days;
                    if (diffDays > 365)
                        dateFormat = "MMM dd yyyy";
                    if (!String.IsNullOrEmpty(listing.RemarksFormatFromCompany) && listing.RemarksFormatFromCompany == "SD")
                    {
                        // UP-114 (code commented)
                        /*if (listing.RemoveCompletionDate)
                            builtNote += ". NEW HOME ~ ";
                        else*/
                            builtNote += "CONST. COMPLETED " + listing.BuildCompletionDate.Value.ToString(dateFormat) + " ~ ";
                    }
                    else
                    {
                        builtNote += "Ready Now! ~ ";
                    }
                    
                    break;

                case BuiltStatus.WithCompletion:
                    if (listing.BuildCompletionDate != null) { 
                        builtNote += listing.BuildCompletionDate.Value.ToString("MMMM") + " completion! ~ ";
                        }
                    break;

                default:
                    Log.Error("Unknown BuiltStatus value reported {BuiltStatusValue}", status);
                    break;

            }


            string remark;

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

            return remark.Replace("\t", "").Replace("\n", " ");
        }

        public static string GetPublicRemarksLeasing(this ResidentialListingRequest listing, BuiltStatus status)
        {
            var builtNote = "MLS# " + listing.MLSNum;

            if (!string.IsNullOrWhiteSpace(listing.CompanyName))
            {
                if (!string.IsNullOrWhiteSpace(builtNote))
                {
                    builtNote += " - ";
                }

                builtNote += "Built by " + listing.CompanyName + " - ";
            }

            switch (status)
            {
                case BuiltStatus.ToBeBuilt:
                    builtNote += "To Be Built! ~ ";
                    break;

                case BuiltStatus.ReadyNow:
                    String dateFormat = "MMM dd";
                    int diffDays = DateTime.Now.Subtract((DateTime)listing.BuildCompletionDate).Days;
                    if (diffDays > 365)
                        dateFormat = "MMM dd yyyy";
                    if (!String.IsNullOrEmpty(listing.RemarksFormatFromCompany) && listing.RemarksFormatFromCompany == "SD")
                    {
                        // UP-114 (code commented)
                        /*if (listing.RemoveCompletionDate)
                            builtNote += ". NEW HOME ~ ";
                        else*/
                        builtNote += "CONST. COMPLETED " + listing.BuildCompletionDate.Value.ToString(dateFormat) + " ~ ";
                    }
                    else
                    {
                        builtNote += "Ready Now! ~ ";
                    }

                    break;

                case BuiltStatus.WithCompletion:
                    if (listing.BuildCompletionDate != null)
                    {
                        builtNote += listing.BuildCompletionDate.Value.ToString("MMMM") + " completion! ~ ";
                    }
                    break;

                default:
                    Log.Error("Unknown BuiltStatus value reported {BuiltStatusValue}", status);
                    break;

            }


            string remark;

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

            // Caballero Holdings
            if (listing.SysOwnedBy == 1092)
            {
                if (remark.Contains('~'))
                {
                    var tempIndex = remark.IndexOf("~", StringComparison.Ordinal) + 1;
                    remark = remark.Substring(tempIndex).Trim().RemoveSlash();
                }
            }

            return remark.Replace("\t", "").Replace("\n", " ");
        }

        private static string GetExtendedRemarks(ResidentialListingRequest listing)
        {
            var salesOfficeAddr = string.Empty;

            if (listing.CommunityProfileSalesOfficeStreetNum.HasValue && !string.IsNullOrWhiteSpace(listing.CommunityProfileSalesOfficeStreetName))
            {
                string soCity = listing.CommunityProfileSalesOfficeCity;
                string soZip = listing.CommunityProfileSalesOfficeZip;
                salesOfficeAddr = " Sales Office at " +
                                   listing.CommunityProfileSalesOfficeStreetNum.Value.ToString()
                                   + " " + listing.CommunityProfileSalesOfficeStreetName
                                   + (!string.IsNullOrWhiteSpace(soCity) ? ", " + soCity : string.Empty)
                                   + (!string.IsNullOrWhiteSpace(soZip) ? ", " + soZip : string.Empty);
            }
            var commPhone = listing.CommunityProfilePhone ?? string.Empty;

            string extendedRemarks = string.Format("Call Sales Associate {0}. {1}. ", commPhone, salesOfficeAddr);

            // BEGIN UP-73
            var apptPhone = String.Empty;
            var alternatePhone = String.Empty;

            // apptPhone
            if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCompany))
                apptPhone = listing.AgentListApptPhoneFromCompany;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhone))
                apptPhone = listing.AgentListApptPhone;
            else if (!String.IsNullOrEmpty(listing.AgentListApptPhoneFromCommunityProfile))
                apptPhone = listing.AgentListApptPhoneFromCommunityProfile;
            else if (!String.IsNullOrEmpty(listing.OwnerPhone))
                apptPhone = listing.OwnerPhone;

            // alternatePhone
            if (!String.IsNullOrEmpty(listing.AlternatePhoneFromCompany))
                alternatePhone = listing.AlternatePhoneFromCompany;
            else if (!String.IsNullOrEmpty(listing.OtherPhoneFromCommunityProfile))
                alternatePhone = listing.OtherPhoneFromCommunityProfile;
            else if (!String.IsNullOrEmpty(listing.OtherPhone))
                alternatePhone = listing.OtherPhone;
            else if (!String.IsNullOrEmpty(listing.AltPhoneCommunity))
                alternatePhone = listing.AltPhoneCommunity;
            // END UP-73


            switch (listing.MarketName)
            {
                case "Austin":
                    if (!String.IsNullOrEmpty(listing.OtherPhone))
                    {
                        extendedRemarks = string.Format("Call Sales Associate {0}. ", listing.OtherPhone);

                        if (!String.IsNullOrEmpty(listing.OwnerPhone) && listing.OtherPhone != listing.OwnerPhone)
                            extendedRemarks = string.Format("Call Sales Associate {0} or {1}. ", listing.OtherPhone, listing.OwnerPhone);
                    }

                    break;
                case "San Antonio":
                    if (!String.IsNullOrEmpty(apptPhone) && alternatePhone != apptPhone)
                    {
                        extendedRemarks = string.Format("Call Sales Associate {0}. ", apptPhone);

                        if (!String.IsNullOrEmpty(alternatePhone))
                            extendedRemarks = string.Format("Call Sales Associate {0} or {1}. ", apptPhone, alternatePhone);
                    }
                    
                    break;
                case "Houston":
                    if (!String.IsNullOrEmpty(apptPhone) && alternatePhone != apptPhone)
                    {
                        extendedRemarks = string.Format("Call Sales Associate {0}. ", apptPhone);
                        alternatePhone = listing.AltPhoneCommunity;
                        if (!String.IsNullOrEmpty(alternatePhone))
                            extendedRemarks = string.Format("Call Sales Associate {0} or {1}. ", apptPhone, alternatePhone);
                    }
                    
                    break;
            }

            return extendedRemarks;
        }

        public static string GetCompletionText(this ResidentialListingRequest listing)
        {
            string value = String.Empty;

            int p = listing.BuildCompletionDate.Value.Month;

            switch (p)
            {
                case 1:
                    value = "January";
                    break;
                case 2:
                    value = "February";
                    break;
                case 3:
                    value = "March";
                    break;
                case 4:
                    value = "April";
                    break;
                case 5:
                    value = "May";
                    break;
                case 6:
                    value = "June";
                    break;
                case 7:
                    value = "July";
                    break;
                case 8:
                    value = "August";
                    break;
                case 9:
                    value = "September";
                    break;
                case 10:
                    value = "October";
                    break;
                case 11:
                    value = "November";
                    break;
                case 12:
                    value = "December";
                    break;
                default:
                    value = "January";
                    break;
            }
            return value;
        }
    }

    public enum BuiltStatus
    {
        ToBeBuilt,
        ReadyNow,
        WithCompletion
    }
}
