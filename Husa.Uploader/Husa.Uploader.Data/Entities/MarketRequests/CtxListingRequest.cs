namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;

    public class CtxListingRequest : ResidentialListingRequest
    {
        private readonly ListingSaleRequestQueryResponse listingResponse;
        private readonly ListingSaleRequestDetailResponse listingDetailResponse;

        public CtxListingRequest(ListingSaleRequestQueryResponse listingResponse)
            : this()
        {
            this.listingResponse = listingResponse ?? throw new ArgumentNullException(nameof(listingResponse));
        }

        public CtxListingRequest(ListingSaleRequestDetailResponse listingDetailResponse)
            : this()
        {
            this.listingDetailResponse = listingDetailResponse ?? throw new ArgumentNullException(nameof(listingDetailResponse));
        }

        private CtxListingRequest()
            : base()
        {
            this.InExtraTerritorialJurisdiction = false;
            this.IsManufacturedAllowed = false;
            this.GeographicID = "0";
            this.EarnestMoney = "0";
            this.ProspectsExempt = "0";
        }

        public override MarketCode MarketCode => MarketCode.CTX;

        public override ResidentialListingRequest CreateFromApiResponse() => new CtxListingRequest()
        {
            ResidentialListingRequestID = this.listingResponse.Id,
            OwnerName = this.listingResponse.OwnerName,
            CompanyName = this.listingResponse.OwnerName,
            MLSNum = this.listingResponse.MlsNumber,
            MarketName = this.listingResponse.Market,
            CityCode = this.listingResponse.City.ToStringFromEnumMember(),
            Subdivision = this.listingResponse.Subdivision,
            Zip = this.listingResponse.ZipCode,
            Address = this.listingResponse.Address,
            ListPrice = (int)this.listingResponse.ListPrice,
            SysCreatedOn = this.listingResponse.SysCreatedOn,
            SysCreatedBy = this.listingResponse.SysCreatedBy,
        };

        public override ResidentialListingRequest CreateFromApiResponseDetail()
        {
            var residentialListingRequest = new CtxListingRequest
            {
                ResidentialListingRequestID = this.listingDetailResponse.Id,
                ResidentialListingRequestGUID = this.listingDetailResponse.Id,
                ResidentialListingID = this.listingDetailResponse.ListingSaleId,
                MarketName = this.MarketCode.GetEnumDescription(),
                ListPrice = (int)this.listingDetailResponse.ListPrice,
                MLSNum = this.listingDetailResponse.MlsNumber,
                MlsStatus = this.listingDetailResponse.MlsStatus.ToStringFromEnumMember(),
                ListStatus = this.listingDetailResponse.MlsStatus.ToStringFromEnumMember(),
                SysCreatedOn = this.listingDetailResponse.SysCreatedOn,
                SysCreatedBy = this.listingDetailResponse.SysCreatedBy,
                SysModifiedOn = this.listingDetailResponse.SysModifiedOn,
                SysModifiedBy = this.listingDetailResponse.SysModifiedBy,
                BuilderName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName,
                CompanyName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName,
                OwnerName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName,
                PlanProfileID = this.listingDetailResponse.SaleProperty.SalePropertyInfo.PlanId,
                CommunityProfileID = this.listingDetailResponse.SaleProperty.SalePropertyInfo.CommunityId,
                StreetNum = this.listingDetailResponse.SaleProperty.AddressInfo.StreetNumber,
                StreetName = this.listingDetailResponse.SaleProperty.AddressInfo.StreetName,
                City = this.listingDetailResponse.SaleProperty.AddressInfo.City.GetEnumDescription(),
                CityCode = this.listingDetailResponse.SaleProperty.AddressInfo.City.ToStringFromEnumMember(),
                State = this.listingDetailResponse.SaleProperty.AddressInfo.State.GetEnumDescription(),
                StateCode = this.listingDetailResponse.SaleProperty.AddressInfo.State.ToStringFromEnumMember(),
                Zip = this.listingDetailResponse.SaleProperty.AddressInfo.ZipCode,
                County = this.listingDetailResponse.SaleProperty.AddressInfo.County?.ToStringFromEnumMember(),
                LotNum = this.listingDetailResponse.SaleProperty.AddressInfo.LotNum,
                Block = this.listingDetailResponse.SaleProperty.AddressInfo.Block,
                Subdivision = this.listingDetailResponse.SaleProperty.AddressInfo.Subdivision,
                BuildCompletionDate = this.listingDetailResponse.SaleProperty.PropertyInfo.ConstructionCompletionDate,
                YearBuiltDesc = this.listingDetailResponse.SaleProperty.PropertyInfo.ConstructionStage.ToString(),
                YearBuilt = this.listingDetailResponse.SaleProperty.PropertyInfo.ConstructionStartYear,
                Legal = this.listingDetailResponse.SaleProperty.PropertyInfo.LegalDescription,
                TaxID = this.listingDetailResponse.SaleProperty.PropertyInfo.TaxId,
                MLSArea = this.listingDetailResponse.SaleProperty.PropertyInfo.MlsArea,
                ////MapscoMapBook = this.listingDetailResponse.SaleProperty.PropertyInfo.MapscoGrid,
                ////MapscoMapCoord = this.listingDetailResponse.SaleProperty.PropertyInfo.MapscoGrid,
                ////LotDim = this.listingDetailResponse.SaleProperty.PropertyInfo.LotDimension,
                ////LotSize = this.listingDetailResponse.SaleProperty.PropertyInfo.LotSize,
                ////LotDesc = this.listingDetailResponse.SaleProperty.PropertyInfo.LotDescription.ToStringFromEnumMembers(),
                ////Occupancy = this.listingDetailResponse.SaleProperty.PropertyInfo.Occupancy.ToStringFromEnumMembers(),
                Latitude = this.listingDetailResponse.SaleProperty.PropertyInfo.Latitude,
                Longitude = this.listingDetailResponse.SaleProperty.PropertyInfo.Longitude,
                ////Category = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.TypeCategory.ToStringFromEnumMember(),
                ////NumStories = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.Stories?.ToStringFromEnumMember(),
                ////SqFtTotal = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.SqFtTotal,
                ////SqFtSource = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.SqFtSource?.ToStringFromEnumMember(),
                ////InteriorDesc = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.SpecialtyRooms.ToStringFromEnumMembers(),
                BathsFull = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.BathsFull,
                BathsHalf = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.BathsHalf,
                ////ParkingDesc = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.GarageDescription.ToStringFromEnumMembers(),
                ////OtherParking = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.OtherParking?.ToStringFromEnumMembers(),
                Beds = this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo.NumBedrooms,
                InclusionsDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Inclusions?.ToStringFromEnumMembers(),
                ////NumFireplaces = this.listingDetailResponse.SaleProperty.FeaturesInfo.Fireplaces,
                FireplaceDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.FireplaceDescription.ToStringFromEnumMembers(),
                FloorsDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Floors?.ToStringFromEnumMembers(),
                ////WindowCoverings = this.listingDetailResponse.SaleProperty.FeaturesInfo.WindowCoverings?.ToStringFromEnumMembers(),
                ////AccessibilityDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Accessibility.ToStringFromEnumMembers(),
                HousingStyleDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.HousingStyle.ToStringFromEnumMembers(),
                ExteriorFeatures = this.listingDetailResponse.SaleProperty.FeaturesInfo.ExteriorFeatures.ToStringFromEnumMembers(),
                RoofDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.RoofDescription.ToStringFromEnumMembers(),
                FoundationDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Foundation.ToStringFromEnumMembers(),
                ////ExteriorDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.Exterior.ToStringFromEnumMembers(),
                ////HasPool = this.listingDetailResponse.SaleProperty.FeaturesInfo.HasPrivatePool,
                ////PoolDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.PrivatePool.ToStringFromEnumMembers(),
                ////FacesDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.HomeFaces.ToStringFromEnumMembers(),
                ////SupElectricity = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierElectricity,
                ////SupWater = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierWater,
                ////SupGarbage = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierGarbage,
                ////SupGas = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierGas,
                ////SupSewer = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierSewer,
                ////SupOther = this.listingDetailResponse.SaleProperty.FeaturesInfo.SupplierOther,
                HeatSystemDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.HeatSystem.ToStringFromEnumMembers(),
                CoolSystemDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.CoolingSystem.ToStringFromEnumMembers(),
                ////HeatingFuel = this.listingDetailResponse.SaleProperty.FeaturesInfo.HeatingFuel.ToStringFromEnumMembers(),
                WaterDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.WaterSewer.ToStringFromEnumMembers(),
                ////GreenCerts = this.listingDetailResponse.SaleProperty.FeaturesInfo.GreenCertification.ToStringFromEnumMembers(),
                EnergyDesc = this.listingDetailResponse.SaleProperty.FeaturesInfo.EnergyFeatures.ToStringFromEnumMembers(),
                ////GreenFeatures = this.listingDetailResponse.SaleProperty.FeaturesInfo.GreenFeatures.ToStringFromEnumMembers(),
                TaxRate = this.listingDetailResponse.SaleProperty.FinancialInfo.TaxRate.ToSafeString(),
                TaxYear = this.listingDetailResponse.SaleProperty.FinancialInfo.TaxYear.ToSafeString(),
                ////IsMultiParcel = this.listingDetailResponse.SaleProperty.FinancialInfo.IsMultipleTaxed.ToString(),
                TitleCo = this.listingDetailResponse.SaleProperty.FinancialInfo.TitleCompany,
                PROPSDTRMS = this.listingDetailResponse.SaleProperty.FinancialInfo.ProposedTerms.ToStringFromEnumMembers(),
                ////HasMultipleHOA = this.listingDetailResponse.SaleProperty.FinancialInfo.HasMultipleHOA.ToString(),
                ////AgentBonusAmount = this.listingDetailResponse.SaleProperty.FinancialInfo.AgentBonusAmount.ToString(),
                ////CompBuyBonusExpireDate = this.listingDetailResponse.SaleProperty.FinancialInfo.BonusExpirationDate,
                ////BuyerIncentive = this.listingDetailResponse.SaleProperty.FinancialInfo.HasBuyerIncentive.ToString(),
                ////CompBuy = this.listingDetailResponse.SaleProperty.FinancialInfo.BuyersAgentCommission?.ToString(),
                ////AltPhoneCommunity = this.listingDetailResponse.SaleProperty.ShowingInfo.AltPhoneCommunity,
                ////AgentListApptPhone = this.listingDetailResponse.SaleProperty.ShowingInfo.AgentListApptPhone,
                ////Showing = this.listingDetailResponse.SaleProperty.ShowingInfo.Showing?.ToStringFromEnumMember(),
                ////RealtorContactEmail = this.listingDetailResponse.SaleProperty.ShowingInfo.RealtorContactEmail,
                Directions = this.listingDetailResponse.SaleProperty.ShowingInfo.Directions,
                AgentPrivateRemarks = this.listingDetailResponse.SaleProperty.ShowingInfo.AgentPrivateRemarks,
                SchoolDistrict = this.listingDetailResponse.SaleProperty.SchoolsInfo.SchoolDistrict?.GetEnumDescription(),
                SchoolName1 = this.listingDetailResponse.SaleProperty.SchoolsInfo.ElementarySchool?.ToStringFromEnumMember(),
                SchoolName2 = this.listingDetailResponse.SaleProperty.SchoolsInfo.MiddleSchool?.ToStringFromEnumMember(),
                SchoolName3 = this.listingDetailResponse.SaleProperty.SchoolsInfo.HighSchool?.ToStringFromEnumMember(),
                ////HOA = this.listingDetailResponse.SaleProperty.FinancialInfo.HOARequirement?.ToStringFromEnumMember(),
                ////PublicRemarks = this.listingDetailResponse.SaleProperty.FeaturesInfo.PropertyDescription,
            };

            return residentialListingRequest;
        }

        public override string GetPublicRemarks()
        {
            var builtNote = "MLS# " + this.MLSNum;

            if (!string.IsNullOrWhiteSpace(this.CompanyName))
            {
                if (!string.IsNullOrWhiteSpace(builtNote))
                {
                    builtNote += " - ";
                }

                builtNote += "Built by " + this.CompanyName + " - ";
            }

            switch (GetBuiltStatus())
            {
                case BuiltStatus.ToBeBuilt:
                    builtNote += "To Be Built! ~ ";
                    break;

                case BuiltStatus.ReadyNow:
                    string dateFormat = "MMM dd";
                    int diffDays = DateTime.Now.Subtract((DateTime)this.BuildCompletionDate).Days;
                    if (diffDays > 365)
                    {
                        dateFormat = "MMM dd yyyy";
                    }

                    if (!string.IsNullOrEmpty(this.RemarksFormatFromCompany) && this.RemarksFormatFromCompany == "SD")
                    {
                        builtNote += "CONST. COMPLETED " + this.BuildCompletionDate.Value.ToString(dateFormat) + " ~ ";
                    }
                    else
                    {
                        builtNote += "Ready Now! ~ ";
                    }

                    break;

                case BuiltStatus.WithCompletion:
                    if (this.BuildCompletionDate != null)
                    {
                        builtNote += this.BuildCompletionDate.Value.ToString("MMMM") + " completion! ~ ";
                    }

                    break;

                default:
                    break;
            }

            return GetRemarks();

            BuiltStatus GetBuiltStatus() => this.YearBuiltDesc switch
            {
                "TB" => BuiltStatus.ToBeBuilt,
                "NW" => BuiltStatus.ReadyNow,
                "UC" => BuiltStatus.WithCompletion,
                _ => BuiltStatus.WithCompletion,
            };

            string GetRemarks()
            {
                string remark;

                if (this.IncludeRemarks != null && this.IncludeRemarks == false)
                {
                    builtNote = string.Empty;
                }

                if (!this.PublicRemarks.Contains('~'))
                {
                    remark = (builtNote + this.PublicRemarks).RemoveSlash();
                }
                else
                {
                    var tempIndex = this.PublicRemarks.IndexOf("~", StringComparison.Ordinal) + 1;
                    var temp = this.PublicRemarks.Substring(tempIndex).Trim();
                    remark = (builtNote + temp).RemoveSlash();
                }

                return remark.Replace("\t", string.Empty).Replace("\n", " ");
            }
        }
    }
}
