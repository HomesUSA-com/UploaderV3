namespace Husa.Uploader.Data.Entities.MarketRequests
{
    using System.Collections.Generic;
    using System.Linq;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Contracts.Response;
    using Husa.Quicklister.CTX.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Uploader.Crosscutting.Converters;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;

    public class CtxListingRequest : ResidentialListingRequest
    {
        private const string DefaultIntegerAsStringValue = "0";

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
            this.GeographicID = DefaultIntegerAsStringValue;
            this.EarnestMoney = DefaultIntegerAsStringValue;
            this.ProspectsExempt = DefaultIntegerAsStringValue;
            this.Rooms = new List<ResidentialListingRequestRoom>();
            this.OpenHouse = new List<OpenHouseRequest>();
        }

        public override MarketCode MarketCode => MarketCode.CTX;
        public override BuiltStatus BuiltStatus => this.YearBuiltDesc switch
        {
            "TOBEB" => BuiltStatus.ToBeBuilt,
            "COMPL" => BuiltStatus.ReadyNow,
            "UC" => BuiltStatus.WithCompletion,
            _ => BuiltStatus.WithCompletion,
        };

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
            ListStatus = this.listingResponse.MlsStatus.ToStringFromEnumMember(),
            SysCreatedOn = this.listingResponse.SysCreatedOn,
            SysCreatedBy = this.listingResponse.SysCreatedBy,
            AllowPendingList = this.listingResponse.ShowOpenHousesPending,
            EnableOpenHouse = this.listingResponse.EnableOpenHouses,
            UpdateGeocodes = this.listingResponse.UpdateGeocodes,
            UseShowingTime = this.listingResponse.UseShowingTime,
        };

        public override ResidentialListingRequest CreateFromApiResponseDetail(CompanyServicesManager.Api.Contracts.Response.CompanyDetail company)
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
                ExpectedActiveDate = DateTime.Now.ToString("MM/dd/yy"),
                RemarksFormatFromCompany = company?.MlsInfo?.RemarksForCompletedHomes,
                ShowingTime = this.listingDetailResponse.ShowingTime,
            };

            FillSalePropertyInfo(this.listingDetailResponse.SaleProperty.SalePropertyInfo);
            FillAddressInfo(this.listingDetailResponse.SaleProperty.AddressInfo);
            FillPropertyInfo(this.listingDetailResponse.SaleProperty.PropertyInfo);
            FillSpacesDimensionsInfo(this.listingDetailResponse.SaleProperty.SpacesDimensionsInfo);
            FillFeaturesInfo(this.listingDetailResponse.SaleProperty.FeaturesInfo);
            FillFinancialInfo(this.listingDetailResponse.SaleProperty.FinancialInfo);
            FillShowingInfo(this.listingDetailResponse.SaleProperty.ShowingInfo);
            FillSchoolsInfo(this.listingDetailResponse.SaleProperty.SchoolsInfo);
            FillStatusInfo(this.listingDetailResponse.StatusFieldsInfo);
            FillRoomsInfo(this.listingDetailResponse.SaleProperty.Rooms);
            FillOpenHouseInfo(this.listingDetailResponse.SaleProperty.OpenHouses);

            return residentialListingRequest;

            void FillSalePropertyInfo(SalePropertyResponse salePropertyInfo)
            {
                if (salePropertyInfo is null)
                {
                    throw new ArgumentNullException(nameof(salePropertyInfo));
                }

                residentialListingRequest.BuilderName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName;
                residentialListingRequest.CompanyName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName;
                residentialListingRequest.OwnerName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.OwnerName;
                residentialListingRequest.ExpiredDate = this.listingDetailResponse.ExpirationDate;
                residentialListingRequest.PlanProfileName = this.listingDetailResponse.SaleProperty.SalePropertyInfo.PlanName;
                residentialListingRequest.CompanyId = this.listingDetailResponse.SaleProperty.SalePropertyInfo.CompanyId;
            }

            void FillAddressInfo(AddressInfoResponse addressInfo)
            {
                if (addressInfo is null)
                {
                    throw new ArgumentNullException(nameof(addressInfo));
                }

                residentialListingRequest.StreetNum = addressInfo.StreetNumber;
                residentialListingRequest.StreetName = addressInfo.StreetName;
                residentialListingRequest.StreetDir = addressInfo.StreetDirection?.ToStringFromEnumMember();
                residentialListingRequest.StreetType = addressInfo.StreetType?.ToStringFromEnumMember();
                residentialListingRequest.UnitNum = addressInfo.UnitNumber;
                residentialListingRequest.City = addressInfo.City.GetEnumDescription();
                residentialListingRequest.CityCode = addressInfo.City.ToStringFromEnumMember();
                residentialListingRequest.State = addressInfo.State.GetEnumDescription();
                residentialListingRequest.StateCode = addressInfo.State.ToStringFromEnumMember();
                residentialListingRequest.Zip = addressInfo.ZipCode;
                residentialListingRequest.County = addressInfo.County?.ToStringFromEnumMember();
                residentialListingRequest.LotNum = addressInfo.LotNum;
                residentialListingRequest.Block = addressInfo.Block;
                residentialListingRequest.Subdivision = addressInfo.Subdivision;
            }

            void FillPropertyInfo(PropertyInfoResponse propertyInfo)
            {
                if (propertyInfo is null)
                {
                    throw new ArgumentNullException(nameof(propertyInfo));
                }

                residentialListingRequest.BuildCompletionDate = propertyInfo.ConstructionCompletionDate;
                residentialListingRequest.YearBuiltDesc = propertyInfo.ConstructionStage?.ToStringFromEnumMember();
                residentialListingRequest.YearBuilt = propertyInfo.ConstructionStartYear;
                residentialListingRequest.YearBuiltSrc = propertyInfo.YearBuiltSource?.ToStringFromEnumMember();
                residentialListingRequest.Legal = propertyInfo.LegalDescription;
                residentialListingRequest.TaxID = propertyInfo.PropertyId;
                residentialListingRequest.Category = propertyInfo.TypeCategory?.ToStringFromEnumMember();
                residentialListingRequest.MLSArea = propertyInfo.MlsArea;
                residentialListingRequest.ListType = propertyInfo.ListingType?.ToStringFromEnumMember();
                residentialListingRequest.Occupancy = propertyInfo.Occupancy;
                residentialListingRequest.UpdateGeocodes = propertyInfo.UpdateGeocodes;
                residentialListingRequest.FacesDesc = propertyInfo.FrontFaces?.ToStringFromEnumMember();
                residentialListingRequest.SqFtTotal = propertyInfo.SqFtTotal;
                residentialListingRequest.SqFtSource = propertyInfo.SqFtSource?.ToStringFromEnumMember();
                residentialListingRequest.AvailableDocumentsDesc = propertyInfo.DocumentsAvailable.ToStringFromEnumMembers();
            }

            void FillSpacesDimensionsInfo(SpacesDimensionsResponse spacesDimensionsInfo)
            {
                if (spacesDimensionsInfo is null)
                {
                    throw new ArgumentNullException(nameof(spacesDimensionsInfo));
                }

                residentialListingRequest.NumberFireplaces = spacesDimensionsInfo.Fireplaces.IntegerToString();
                residentialListingRequest.NumDiningAreas = spacesDimensionsInfo.DiningAreas;
                residentialListingRequest.NumLivingAreas = spacesDimensionsInfo.LivingAreas;
                residentialListingRequest.BathsFull = spacesDimensionsInfo.BathsFull;
                residentialListingRequest.BathsHalf = spacesDimensionsInfo.BathsHalf;
                residentialListingRequest.Beds = spacesDimensionsInfo.NumBedrooms;
            }

            void FillFeaturesInfo(FeaturesResponse featuresInfo)
            {
                if (featuresInfo is null)
                {
                    throw new ArgumentNullException(nameof(featuresInfo));
                }

                residentialListingRequest.HousingStyleDesc = featuresInfo.HousingStyle.ToStringFromEnumMembers();
                residentialListingRequest.FoundationDesc = featuresInfo.Foundation.ToStringFromEnumMembers();
                residentialListingRequest.NumStories = featuresInfo.Stories?.ToStringFromEnumMember();
                residentialListingRequest.AtticRoom = featuresInfo.SpecialtyRooms.ToStringFromEnumMembers();
                residentialListingRequest.RoofDesc = featuresInfo.RoofDescription.ToStringFromEnumMembers();
                residentialListingRequest.ExteriorDesc = featuresInfo.ConstructionExterior.ToStringFromEnumMembers();
                residentialListingRequest.FireplaceDesc = featuresInfo.FireplaceDescription.ToStringFromEnumMembers();
                residentialListingRequest.FloorsDesc = featuresInfo.Floors.ToStringFromEnumMembers();
                residentialListingRequest.KitchenDesc = featuresInfo.Kitchen.ToStringFromEnumMembers();
                residentialListingRequest.LaundryFacilityDesc = featuresInfo.Laundry.ToStringFromEnumMembers();
                residentialListingRequest.Bed1Desc = featuresInfo.MasterBedroom.ToStringFromEnumMembers();
                residentialListingRequest.GarageCarportDesc = featuresInfo.Garage?.ToStringFromEnumMember();
                residentialListingRequest.CarportDesc = featuresInfo.Carport?.ToStringFromCarport();
                residentialListingRequest.GarageDesc = featuresInfo.GarageDescription.ToStringFromEnumMembers();
                residentialListingRequest.InclusionsDesc = featuresInfo.Inclusions.ToStringFromEnumMembers();
                residentialListingRequest.AppliancesDesc = featuresInfo.AppliancesEquipment.ToStringFromEnumMembers();
                residentialListingRequest.LotDim = featuresInfo.LotDimension;
                residentialListingRequest.LotSize = featuresInfo.LotSize;
                residentialListingRequest.FenceDesc = featuresInfo.Fencing.ToStringFromEnumMembers();
                residentialListingRequest.HasWaterAccess = featuresInfo.WaterAccess.BoolToNumericBool();
                residentialListingRequest.WaterAccessDesc = featuresInfo.WaterAccessType.ToStringFromEnumMembers();
                residentialListingRequest.WaterfrontYN = featuresInfo.WaterFront.BoolToNumericBool();
                residentialListingRequest.WaterfrontFeatures = featuresInfo.WaterFeatures.ToStringFromEnumMembers();
                residentialListingRequest.IsGatedCommunity = featuresInfo.GatedCommunity.BoolToNumericBool();
                residentialListingRequest.HasSprinklerSys = featuresInfo.SprinklerSystem.BoolToNumericBool();
                residentialListingRequest.SprinklerSysDesc = featuresInfo.SprinklerSystemDescription.ToStringFromEnumMembers();
                residentialListingRequest.PoolDesc = featuresInfo.Pool.ToStringFromEnumMembers();
                residentialListingRequest.RestrictionsDesc = featuresInfo.RestrictionsType.ToStringFromEnumMembers();
                residentialListingRequest.ExteriorFeatures = featuresInfo.ExteriorFeatures.ToStringFromEnumMembers();
                residentialListingRequest.TopoLandDescription = featuresInfo.TopoLandDescription.ToStringFromEnumMembers();
                residentialListingRequest.CommonFeatures = featuresInfo.NeighborhoodAmenities.ToStringFromEnumMembers();
                residentialListingRequest.RoadFrontageDesc = featuresInfo.AccessRoadSurface.ToStringFromEnumMembers();
                residentialListingRequest.UpgradedEnergyFeatures = featuresInfo.UpgradedEnergyFeatures.BoolToNumericBool();
                residentialListingRequest.EES = featuresInfo.EESFeatures.BoolToNumericBool();
                residentialListingRequest.GreenCerts = featuresInfo.GreenBuildingVerification.ToStringFromEnumMembers();
                residentialListingRequest.EESFeatures = featuresInfo.EnergyFeatures.ToStringFromEnumMembers();
                residentialListingRequest.GreenIndoorAirQuality = featuresInfo.AirQuality.ToStringFromEnumMembers();
                residentialListingRequest.GreenWaterConservation = featuresInfo.WaterConservation.ToStringFromEnumMembers();
                residentialListingRequest.EnergyDesc = featuresInfo.GreenVerificationSource.ToStringFromEnumMembers();
                residentialListingRequest.HeatSystemDesc = featuresInfo.HeatSystem.ToStringFromEnumMembers();
                residentialListingRequest.CoolSystemDesc = featuresInfo.CoolingSystem.ToStringFromEnumMembers();
                residentialListingRequest.WaterDesc = featuresInfo.WaterSewer.ToStringFromEnumMembers();
                residentialListingRequest.SupOther = featuresInfo.SupplierOther.ToStringFromEnumMembers();
                residentialListingRequest.Bed1Desc = featuresInfo.MasterBedroom.ToStringFromEnumMembers();
            }

            void FillFinancialInfo(FinancialResponse financialInfo)
            {
                if (financialInfo is null)
                {
                    throw new ArgumentNullException(nameof(financialInfo));
                }

                residentialListingRequest.ProposedTerms = financialInfo.ProposedTerms.ToStringFromEnumMembers();
                residentialListingRequest.Exemptions = financialInfo.Exemptions.ToStringFromEnumMembers();
                residentialListingRequest.TaxRate = financialInfo.TaxRate.DecimalToString();
                residentialListingRequest.TaxYear = financialInfo.TaxYear.IntegerToString();
                residentialListingRequest.TitleCo = financialInfo.TitleCompany;
                residentialListingRequest.HOA = financialInfo.HoaRequirement?.ToStringFromHOARequirementCTX();
                residentialListingRequest.AssocName = financialInfo.HoaName;
                residentialListingRequest.AssocTransferFee = (int?)financialInfo.HoaTransferFeeAmount;
                residentialListingRequest.HoaWebsite = financialInfo.HoaWebsite;
                residentialListingRequest.AssocPhone = financialInfo.HoaPhone.PhoneFormat(true);
                residentialListingRequest.ManagementCompany = financialInfo.HoaMgmtCo;
                residentialListingRequest.AssocFeeFrequency = financialInfo.HoaTerm?.ToStringFromEnumMember();
                residentialListingRequest.AssocFeeIncludes = financialInfo.HoaFeeIncludes.ToStringFromEnumMembers();
                residentialListingRequest.AssocFee = (int?)financialInfo.HoaAmount;
            }

            void FillShowingInfo(ShowingResponse showingInfo)
            {
                if (showingInfo is null)
                {
                    throw new ArgumentNullException(nameof(showingInfo));
                }

                residentialListingRequest.AgentListApptPhone = showingInfo.ShowingPhone;
                residentialListingRequest.OtherPhone = showingInfo.SecondShowingPhone;
                residentialListingRequest.BuyerIncentive = showingInfo.BuyersAgentCommission.DecimalToString();
                residentialListingRequest.BuyerIncentiveDesc = showingInfo.BuyersAgentCommissionType.ToStringFromEnumMember();
                residentialListingRequest.LockboxTypeDesc = showingInfo.LockboxType?.ToStringFromEnumMember();
                residentialListingRequest.LockboxLocDesc = showingInfo.LockboxLocation.ToStringFromEnumMembers();
                residentialListingRequest.Showing = showingInfo.Showing.ToStringFromEnumMembers();
                residentialListingRequest.Directions = showingInfo.Directions;
                residentialListingRequest.AgentPrivateRemarks = showingInfo.AgentPrivateRemarks;
                residentialListingRequest.AgentPrivateRemarksAdditional = showingInfo.AgentPrivateRemarksAdditional;
                residentialListingRequest.PublicRemarks = showingInfo.PublicRemarks;
                residentialListingRequest.HasAgentBonus = showingInfo.HasAgentBonus;
                residentialListingRequest.HasBonusWithAmount = showingInfo.HasBonusWithAmount;
                residentialListingRequest.AgentBonusAmount = showingInfo.AgentBonusAmount.DecimalToString();
                residentialListingRequest.AgentBonusAmountType = showingInfo.AgentBonusAmountType.ToStringFromEnumMember();
                residentialListingRequest.CompBuyBonusExpireDate = showingInfo.BonusExpirationDate;
                residentialListingRequest.EnableOpenHouse = showingInfo.EnableOpenHouses;
            }

            void FillSchoolsInfo(SchoolsResponse schoolsInfo)
            {
                if (schoolsInfo is null)
                {
                    throw new ArgumentNullException(nameof(schoolsInfo));
                }

                residentialListingRequest.SchoolDistrict = this.listingDetailResponse.SaleProperty.SchoolsInfo.SchoolDistrict?.GetEnumDescription();
                residentialListingRequest.SchoolName1 = this.listingDetailResponse.SaleProperty.SchoolsInfo.ElementarySchool?.ToStringFromEnumMember();
                residentialListingRequest.SchoolName2 = this.listingDetailResponse.SaleProperty.SchoolsInfo.MiddleSchool?.ToStringFromEnumMember();
                residentialListingRequest.HighSchool = this.listingDetailResponse.SaleProperty.SchoolsInfo.HighSchool?.ToStringFromEnumMember();
            }

            void FillStatusInfo(ListingSaleStatusFieldsResponse statusInfo)
            {
                if (statusInfo is null)
                {
                    throw new ArgumentNullException(nameof(statusInfo));
                }

                residentialListingRequest.SoldPrice = statusInfo.ClosePrice;
                residentialListingRequest.ClosedDate = statusInfo.ClosedDate;
                residentialListingRequest.Financing = statusInfo.SellerConcessionDescription?.ToStringFromEnumMember();
                residentialListingRequest.SellConcess = statusInfo.SellConcess;
                residentialListingRequest.ContractDate = statusInfo.ContractDate;
                residentialListingRequest.AgentMarketUniqueId = statusInfo.AgentMarketUniqueId;
                residentialListingRequest.SecondAgentMarketUniqueId = statusInfo.SecondAgentMarketUniqueId;
                residentialListingRequest.EstClosedDate = statusInfo.EstimatedClosedDate;
                residentialListingRequest.IsWithdrawalListingAgreement = statusInfo.WithdrawalActiveListingAgreement.BoolToNumericBool();
                residentialListingRequest.WithdrawalReason = statusInfo.WithdrawalReason;
                residentialListingRequest.WithdrawnDate = statusInfo.WithdrawalDate;
                residentialListingRequest.OffMarketDate = statusInfo.OffMarketDate;
            }

            void FillRoomsInfo(IEnumerable<RoomResponse> rooms)
            {
                if (rooms == null || !rooms.Any())
                {
                    return;
                }

                foreach (var room in rooms)
                {
                    this.Rooms.Add(new()
                    {
                        Width = room.Width ?? 0,
                        Length = room.Length ?? 0,
                        Level = room.Level.ToStringFromEnumMember(),
                        RoomType = room.RoomType.ToStringFromEnumMember(),
                    });
                }

                residentialListingRequest.Rooms = this.Rooms;
            }

            void FillOpenHouseInfo(IEnumerable<OpenHouseResponse> openHouses)
            {
                if (openHouses == null || !openHouses.Any())
                {
                    return;
                }

                foreach (var openHouse in openHouses)
                {
                    var today = (short)DateTime.Now.DayOfWeek;
                    var day = (short)Enum.Parse<DayOfWeek>(openHouse.Type.ToStringFromEnumMember());
                    var diff = day - today;
                    this.OpenHouse.Add(new()
                    {
                        Date = OpenHouseExtensions.GetNextWeekday(DateTime.Today, Enum.Parse<DayOfWeek>(openHouse.Type.ToString(), ignoreCase: true)),
                        StartTime = openHouse.StartTime,
                        EndTime = openHouse.EndTime,
                        Active = true,
                        Comments = OpenHouseExtensions.GetComments(openHouse.Refreshments, openHouse.Lunch),
                        Type = OpenHouseType.Public,
                        Order = diff >= 0 ? diff : 7 + diff,
                    });
                }

                residentialListingRequest.OpenHouse = this.OpenHouse;
            }
        }

        public override string GetAgentRemarksMessage()
        {
            var privateRemarks = "LIMITED SERVICE LISTING: Buyer verifies dimensions & ISD info. Use Bldr contract.";
            var saleOfficeInfo = this.GetSalesAssociateRemarksMessage();
            var bonusMessage = this.GetAgentBonusRemarksMessage();

            if (!string.IsNullOrWhiteSpace(saleOfficeInfo))
            {
                privateRemarks = $"{saleOfficeInfo} {privateRemarks}";
            }

            if (!string.IsNullOrWhiteSpace(bonusMessage))
            {
                privateRemarks += $"{bonusMessage}";
            }

            if (!string.IsNullOrWhiteSpace(this.RealtorContactEmail))
            {
                privateRemarks += $" Email contact: {this.RealtorContactEmail}.";
            }

            if (!string.IsNullOrWhiteSpace(this.PlanProfileName))
            {
                privateRemarks += $" Plan - {this.PlanProfileName}.";
            }

            const string homeUnderConstruction = "Home is under construction. For your safety, call appt number for showings.";

            return this.BuiltStatus != BuiltStatus.ReadyNow ? $"{homeUnderConstruction} {privateRemarks}" : privateRemarks;
        }
    }
}
