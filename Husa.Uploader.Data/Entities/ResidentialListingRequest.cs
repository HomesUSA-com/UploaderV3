namespace Husa.Uploader.Data.Entities
{
    using System.Collections.Generic;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using BuiltStatus = Husa.Uploader.Crosscutting.Enums.BuiltStatus;

    public abstract class ResidentialListingRequest
    {
        public const string DollarSign = "$";
        private string agentListApptPhone;

        // Never remove this property
        public string WorkingBy { get; set; }
        public bool IsNewListing => string.IsNullOrWhiteSpace(this.MLSNum);
        public string CommunityName { get; set; }
        public string CompanyName { get; set; }
        public Guid CompanyId { get; set; }
        public Guid ResidentialListingRequestID { get; set; }
        public Guid ResidentialListingID { get; set; }
        public short? SysStatusID { get; set; }
        public string SysState { get; set; }
        public Guid? SysCreatedBy { get; set; }
        public DateTime? SysCreatedOn { get; set; }
        public Guid? SysModifiedBy { get; set; }
        public DateTime? SysModifiedOn { get; set; }
        public string Zip { get; set; }
        public string YearBuiltDesc { get; set; }
        public int? YearBuilt { get; set; }
        public string WaterDesc { get; set; }
        public string ViewDesc { get; set; }
        public string UtilitiesDesc { get; set; }
        public string UID { get; set; }
        public string TitleCo { get; set; }
        public string TaxYear { get; set; }
        public string TaxRate { get; set; }
        public string TaxID { get; set; }
        public string Subdivision { get; set; }
        public string StreetType { get; set; }
        public string StreetNumDisplay { get; set; }
        public string StreetNum { get; set; }
        public string StreetName { get; set; }
        public string Address { get; set; }
        public DateTime? StatusChangeDate { get; set; }
        public string State { get; set; }
        public int? SqFtTotal { get; set; }
        public string SqFtSource { get; set; }
        public string ShowingInstructions { get; set; }
        public string SewerDesc { get; set; }
        public string SchoolName6 { get; set; }
        public string SchoolName5 { get; set; }
        public string SchoolName4 { get; set; }
        public string SchoolName2 { get; set; }
        public string SchoolName1 { get; set; }
        public string HighSchool { get; set; }
        public string SchoolDistrict { get; set; }
        public string RoofDesc { get; set; }
        public string Restrictions { get; set; }
        public string RestrictionsDesc { get; set; }
        public string PublicRemarks { get; set; }
        public string PropType { get; set; }
        public string PropSubType { get; set; }
        public string PrivateRemarks { get; set; }
        public string ParkingDesc { get; set; }
        public string OwnerPhone { get; set; }
        public string OwnerName { get; set; }
        public string OtherRoomDesc { get; set; }
        public string OtherPhone { get; set; }
        public string OfficeList { get; set; }
        public string Occupancy { get; set; }
        public string NumStories { get; set; }
        public int? NumLivingAreas { get; set; }
        public int? NumDiningAreas { get; set; }
        public int? NumBedsOtherLevels { get; set; }
        public int? NumBedsMainLevel { get; set; }
        public DateTime? Modified { get; set; }
        public string MLSNum { get; set; }
        public string MLSArea { get; set; }
        public string MapscoMapPage { get; set; }
        public string MapscoMapCoord { get; set; }
        public string LotSize { get; set; }
        public string LotDesc { get; set; }
        public string LockboxTypeDesc { get; set; }
        public string ListStatus { get; set; }
        public string ListStatusName { get; set; }
        public string OldListStatus { get; set; }
        public int ListPrice { get; set; }
        public string ListOninternetDesc { get; set; }
        public DateTime? ListDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string ExpectedActiveDate { get; set; }
        public string Legal { get; set; }
        public string LaundryLocDesc { get; set; }
        public string KitchenDesc { get; set; }
        public string IsWaterFront { get; set; }
        public string IsGatedCommunity { get; set; }
        public string InteriorDesc { get; set; }
        public string HOA { get; set; }
        public string HoaWebsite { get; set; }
        public string AssocName { get; set; }
        public int? AssocFee { get; set; }
        public string AssocFeeFrequency { get; set; }
        public int? AssocTransferFee { get; set; }
        public string AssocFeePaid { get; set; }
        public string AssocFeeIncludes { get; set; }
        public string AssocPhone { get; set; }
        public IList<HoaRequest> HOAs { get; set; }
        public string NumHoas { get; set; }
        public string HeatSystemDesc { get; set; }
        public bool HasPool { get; set; }
        public string HasHandicapAmenities { get; set; }
        public string GarageDesc { get; set; }
        public string GarageCarpotDesc { get; set; }
        public int? GarageCapacity { get; set; }
        public string FoundationDesc { get; set; }
        public string FloorsDesc { get; set; }
        public string FenceDesc { get; set; }
        public string FacesDesc { get; set; }
        public string ExemptionsDesc { get; set; }
        public int? DOM { get; set; }
        public string Directions { get; set; }
        public string DiningRoomDesc { get; set; }
        public string County { get; set; }
        public string CoolSystemDesc { get; set; }
        public string ConstructionDesc { get; set; }
        public string CompBuyType { get; set; }
        public string CompBuy { get; set; }
        public string CommonFeatures { get; set; }
        public string City { get; set; }
        public int? CDOM { get; set; }
        public string BuilderName { get; set; }
        public int? Beds { get; set; }
        public string Bed1Desc { get; set; }
        public decimal BathsTotal { get; set; }
        public int? BathsHalf { get; set; }
        public int? BathsFull { get; set; }
        public string AppliancesDesc { get; set; }
        public string AgentList { get; set; }
        public string AddressOninternetAllowed { get; set; }
        public string AccessInstructionsDesc { get; set; }
        public DateTime? BuildCompletionDate { get; set; }
        public DateTime? SysTimestamp { get; set; }
        public Guid? CommunityProfileID { get; set; }
        public string AvailableDocumentsDesc { get; set; }
        public string HasWaterAccess { get; set; }
        public string HasSprinklerSys { get; set; }
        public string SprinklerSysDesc { get; set; }
        public string UnitStyleDesc { get; set; }
        public Guid ResidentialListingRequestGUID { get; set; }
        public string MlsStatus { get; set; }
        public string MarketName { get; set; }

        public abstract MarketCode MarketCode { get; }

        public string WaterfrontDesc { get; set; }
        public string UnitNum { get; set; }
        public string StreetDir { get; set; }
        public string PoolDesc { get; set; }
        public string NumLakes { get; set; }
        public string HandicapDesc { get; set; }
        public string FireplaceDesc { get; set; }
        public string ExteriorDesc { get; set; }
        public decimal? Latitude { get; set; }
        public string WaterAccessDesc { get; set; }
        public string LaundryFacilityDesc { get; set; }
        public decimal? Longitude { get; set; }
        public string OpenHouseDays { get; set; }
        public string OpenHouseWeekdayStart { get; set; }
        public string OpenHouseWeekdayEnd { get; set; }
        public string OpenHouseWeekendStart { get; set; }
        public string OpenHouseWeekendEnd { get; set; }
        public decimal LandSQFT { get; set; }
        public string BodyofWater { get; set; }
        public bool? HasAgentBonus { get; set; }

        public bool HasContingencyInfo { get; set; }
        public string UtilityRoomDesc { get; set; }
        public bool IsPlannedDevelopment { get; set; }
        public string PlannedDevelopment { get; set; }
        public string ListType { get; set; }
        public int? ListPriceOrig { get; set; }
        public string IsForLease { get; set; }
        public string HousingStyleDesc { get; set; }
        public string EnergyDesc { get; set; }
        public string BedBathDesc { get; set; }
        public string AgentListApptPhone { get => this.agentListApptPhone.PhoneFormat(); set => this.agentListApptPhone = value; }
        public string LotSizeSrc { get; set; }
        public string OvenDesc { get; set; }
        public bool IsNewConstruction { get; set; }
        public string Disclosures { get; set; }
        public string Bed1Dim { get; set; }
        public string Bed2Dim { get; set; }
        public string DiningRoomDim { get; set; }
        public string DefectsDesc { get; set; }
        public string OtherFees { get; set; }
        public string Bed3Dim { get; set; }
        public string YearBuiltSrc { get; set; }
        public string Bed4Dim { get; set; }
        public string BreakfastDim { get; set; }
        public string KitchenDim { get; set; }
        public string RangeDesc { get; set; }

        public string LivingRoom1Dim { get; set; }
        public string GreenCerts { get; set; }
        public string SectionNum { get; set; }
        public string StateCode { get; set; }
        public string GolfCourseName { get; set; }
        public string Bed5Dim { get; set; }
        public string CountertopsDesc { get; set; }
        public string LotDim { get; set; }
        public string UpgradedEEFeatures { get; set; }
        public string StudyDim { get; set; }
        public string Showing { get; set; }
        public string SellerType { get; set; }
        public string SecurityDesc { get; set; }
        public string PossessionDesc { get; set; }
        public string MUDDistrict { get; set; }
        public string MLSSubArea { get; set; }
        public string LoanType { get; set; }
        public int? LivingRoom1Width { get; set; }
        public string LivingRoom1Level { get; set; }
        public int? LivingRoom1Length { get; set; }
        public int? KitchenWidth { get; set; }
        public string KitchenLevel { get; set; }
        public int? KitchenLength { get; set; }
        public string KitchenEquipmentDesc { get; set; }
        public string ParcelId { get; set; }
        public string IsMultiParcel { get; set; }
        public string HousingTypeDesc { get; set; }
        public string HasSecuritySys { get; set; }
        public int? GarageWidth { get; set; }
        public int? GarageLength { get; set; }
        public string FinancingProposed { get; set; }
        public int? CoveredSpacesTotal { get; set; }
        public int? CarportCapacity { get; set; }
        public int? Bed4Width { get; set; }
        public string Bed4Level { get; set; }
        public int? Bed4Length { get; set; }
        public int? Bed3Width { get; set; }
        public string Bed3Level { get; set; }
        public int? Bed3Length { get; set; }
        public int? Bed2Width { get; set; }
        public string Bed2Level { get; set; }
        public int? Bed2Length { get; set; }
        public int? Bed1Width { get; set; }
        public string Bed1Level { get; set; }
        public int? Bed1Length { get; set; }
        public string AgentListEmail { get; set; }
        public string CityCode { get; set; }
        public int? StudyWidth { get; set; }
        public string StudyLevel { get; set; }
        public int? StudyLength { get; set; }
        public int? BreakfastWidth { get; set; }
        public string BreakfastLevel { get; set; }
        public int? BreakfastLength { get; set; }
        public DateTime? PendingDate { get; set; }
        public DateTime? EstClosedDate { get; set; }
        public int? UtilityRoomWidth { get; set; }
        public string UtilityRoomLevel { get; set; }
        public int? UtilityRoomLength { get; set; }
        public int? DiningRoomWidth { get; set; }
        public string DiningRoomLevel { get; set; }
        public int? DiningRoomLength { get; set; }
        public int? BathsFullLevel1 { get; set; }
        public string GreenFeatures { get; set; }
        public DateTime? SysCompletedOn { get; set; }
        public int? SysCompletedBy { get; set; }
        public string TitleCoPhone { get; set; }
        public string TitleCoLocation { get; set; }
        public string LotNum { get; set; }
        public string Block { get; set; }
        public int? BathsHalfLevel2 { get; set; }
        public int? BathsHalfLevel1 { get; set; }
        public int? BathsFullLevel2 { get; set; }
        public string OwnerVideo { get; set; }
        public int? LivingRoom2Width { get; set; }
        public string LivingRoom2Level { get; set; }
        public int? LivingRoom2Length { get; set; }
        public string HasAerialPhoto { get; set; }
        public string AgentMarketUniqueId { get; set; }
        public string AgentLoginName { get; set; }
        public string CoopSale { get; set; }
        public bool? SellingAgentPresent { get; set; }
        public string SoilType { get; set; }
        public string SellConcess { get; set; }
        public string SellConcessDescription { get; set; }
        public decimal? SoldPrice { get; set; }
        public decimal? SellerBuyerCost { get; set; }
        public int? OtherRoom1Width { get; set; }
        public string OtherRoom1Level { get; set; }
        public int? OtherRoom1Length { get; set; }
        public string MortgageCoSold { get; set; }
        public string MFinancing { get; set; }
        public int? LivingRoom3Width { get; set; }
        public string LivingRoom3Level { get; set; }
        public int? LivingRoom3Length { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? Bed5Width { get; set; }
        public string Bed5Level { get; set; }
        public int? Bed5Length { get; set; }
        public int? BathsHalfLevel3 { get; set; }
        public int? BathsHalfBasement { get; set; }
        public int? BathsFullLevel3 { get; set; }
        public int? BathsFullBasement { get; set; }
        public DateTime? OffMarketDate { get; set; }
        public string SchoolName7 { get; set; }
        public string AltPhoneCommunity { get; set; }
        public int? OtherRoom2Width { get; set; }
        public string OtherRoom2Level { get; set; }
        public int? OtherRoom2Length { get; set; }
        public int? Bath1Length { get; set; }
        public int? Bath1Width { get; set; }
        public string HasMultipleHOA { get; set; }
        public string HeatingFuel { get; set; }
        public string InclusionsDesc { get; set; }
        public string SupElectricity { get; set; }
        public string SupGarbage { get; set; }
        public string SupGas { get; set; }
        public string SupSewer { get; set; }
        public string SupWater { get; set; }
        public string WindowCoverings { get; set; }
        public string OtherRoom2Desc { get; set; }
        public string AccessibilityDesc { get; set; }
        public string NumberFireplaces { get; set; }
        public string MediaRoomDim { get; set; }
        public string OtherRoom2Dim { get; set; }
        public string GarageCarportDesc { get; set; }
        public string SupOther { get; set; }
        public string CBNCB { get; set; }
        public string MiscellaneousDesc { get; set; }
        public string UtilityRoomDim { get; set; }
        public string GuestAccommodationsDesc { get; set; }
        public DateTime? BackOnMarketDate { get; set; }
        public int? NumGuestBeds { get; set; }
        public int? NumGuestFullBaths { get; set; }
        public int? NumGuestHalfBaths { get; set; }
        public string MapscoMapBook { get; set; }
        public string LivingRoom2Dim { get; set; }
        public string CarportDesc { get; set; }
        public string SoldTerms { get; set; }
        public string HowSold { get; set; }
        public string LockboxLocDesc { get; set; }
        public string BuyerIncentiveDesc { get; set; }
        public string BuyerIncentive { get; set; }
        public string EES { get; set; }
        public string EESFeatures { get; set; }
        public string Bus3 { get; set; }
        public string Bus2 { get; set; }
        public string Bus1 { get; set; }
        public string SchoolType4 { get; set; }
        public string Bus4 { get; set; }
        public string OtherRoom1Dim { get; set; }
        public string Bath1Level { get; set; }
        public string Bath1Desc { get; set; }
        public string SpecialNotes { get; set; }
        public DateTime? ExpiredDateOption { get; set; }
        public string CompBuyBonus { get; set; }
        public DateTime? CompBuyBonusExpireDate { get; set; }
        public string CompBuyBonusDesc { get; set; }
        public string VirtualTourURL { get; set; }
        public string Excludes { get; set; }
        public int? NumBlocksToCollegeShuttle { get; set; }
        public int? AppraisalAmount { get; set; }
        public string LoanPaymentType { get; set; }
        public string LoanPayment { get; set; }
        public string LoanBalance { get; set; }
        public string PlanProfileName { get; set; }
        public int? CommunityProfileSalesOfficeStreetNum { get; set; }
        public string CommunityProfileSalesOfficeStreetName { get; set; }
        public string CommunityProfileSalesOfficeCity { get; set; }
        public string CommunityProfileSalesOfficeZip { get; set; }
        public string CommunityProfilePhone { get; set; }
        public string SellingAgentLicenseNum { get; set; }
        public DateTime? ContingencyDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string SellingAgentLastName { get; set; }
        public string SellingAgentFristName { get; set; }
        public string BrokerName { get; set; }
        public string BrokerOffice { get; set; }
        public string RealtorContactEmail { get; set; }

        public string StreetSuffixFQ { get; set; }

        public string OtherParking { get; set; }

        public int? ClosetWidth { get; set; }

        public int? ClosetLength { get; set; }

        public string LegalSubdivision { get; set; }

        public string Category { get; set; }

        public bool? IncludeRemarks { get; set; }

        public string RemarksFormatFromCompany { get; set; }

        public string OtherFeesInclude { get; set; }
        public string VirtualTourLink { get; set; }

        public string SchoolDistrictLongName { get; set; }

        public DateTime? ContractDate { get; set; }

        public string ContingencyInfo { get; set; }

        public DateTime? WithdrawnDate { get; set; }

        public string OHStartTimeSun { get; set; }

        public string OHStartTimeMon { get; set; }

        public string OHStartTimeTue { get; set; }

        public string OHStartTimeWed { get; set; }

        public string OHStartTimeThu { get; set; }

        public string OHStartTimeFri { get; set; }

        public string OHStartTimeSat { get; set; }

        public string OHEndTimeSun { get; set; }

        public string OHEndTimeMon { get; set; }

        public string OHEndTimeTue { get; set; }

        public string OHEndTimeWed { get; set; }

        public string OHEndTimeThu { get; set; }

        public string OHEndTimeFri { get; set; }

        public string OHEndTimeSat { get; set; }

        public string OHTypeSun { get; set; }

        public string OHTypeMon { get; set; }

        public string OHTypeTue { get; set; }

        public string OHTypeWed { get; set; }

        public string OHTypeThu { get; set; }

        public string OHTypeFri { get; set; }

        public string OHTypeSat { get; set; }

        public string OHRefreshmentsSun { get; set; }

        public string OHRefreshmentsMon { get; set; }

        public string OHRefreshmentsTue { get; set; }

        public string OHRefreshmentsWed { get; set; }

        public string OHRefreshmentsThu { get; set; }

        public string OHRefreshmentsFri { get; set; }

        public string OHRefreshmentsSat { get; set; }

        public string OHLunchSun { get; set; }

        public string OHLunchMon { get; set; }

        public string OHLunchTue { get; set; }

        public string OHLunchWed { get; set; }

        public string OHLunchThu { get; set; }

        public string OHLunchFri { get; set; }

        public string OHLunchSat { get; set; }

        public string OHCommentsSun { get; set; }

        public string OHCommentsMon { get; set; }

        public string OHCommentsTue { get; set; }

        public string OHCommentsWed { get; set; }

        public string OHCommentsThu { get; set; }

        public string OHCommentsFri { get; set; }

        public string OHCommentsSat { get; set; }

        public string OHStartTimeSunOH { get; set; }

        public string OHStartTimeMonOH { get; set; }

        public string OHStartTimeTueOH { get; set; }

        public string OHStartTimeWedOH { get; set; }

        public string OHStartTimeThuOH { get; set; }

        public string OHStartTimeFriOH { get; set; }

        public string OHStartTimeSatOH { get; set; }

        public string OHEndTimeSunOH { get; set; }

        public string OHEndTimeMonOH { get; set; }

        public string OHEndTimeTueOH { get; set; }

        public string OHEndTimeWedOH { get; set; }

        public string OHEndTimeThuOH { get; set; }

        public string OHEndTimeFriOH { get; set; }

        public string OHEndTimeSatOH { get; set; }

        public string OHTypeSunOH { get; set; }

        public string OHTypeMonOH { get; set; }

        public string OHTypeTueOH { get; set; }

        public string OHTypeWedOH { get; set; }

        public string OHTypeThuOH { get; set; }

        public string OHTypeFriOH { get; set; }

        public string OHTypeSatOH { get; set; }

        public string OHRefreshmentsSunOH { get; set; }

        public string OHRefreshmentsMonOH { get; set; }

        public string OHRefreshmentsTueOH { get; set; }

        public string OHRefreshmentsWedOH { get; set; }

        public string OHRefreshmentsThuOH { get; set; }

        public string OHRefreshmentsFriOH { get; set; }

        public string OHRefreshmentsSatOH { get; set; }

        public string OHLunchSunOH { get; set; }

        public string OHLunchMonOH { get; set; }

        public string OHLunchTueOH { get; set; }

        public string OHLunchWedOH { get; set; }

        public string OHLunchThuOH { get; set; }

        public string OHLunchFriOH { get; set; }

        public string OHLunchSatOH { get; set; }

        public string OHCommentsSunOH { get; set; }

        public string OHCommentsMonOH { get; set; }

        public string OHCommentsTueOH { get; set; }

        public string OHCommentsWedOH { get; set; }

        public string OHCommentsThuOH { get; set; }

        public string OHCommentsFriOH { get; set; }

        public string OHCommentsSatOH { get; set; }

        public string Bed6Dim { get; set; }

        public string Bonus { get; set; }

        public DateTime? BonusEndDate { get; set; }

        public string Bed1Location { get; set; }

        public string Bed2Location { get; set; }

        public string Bed3Location { get; set; }

        public string Bed4Location { get; set; }

        public string Bed5Location { get; set; }

        public string BreakfastLocation { get; set; }

        public string DenLocation { get; set; }

        public string DiningLocation { get; set; }

        public string ExtraRoomLocation { get; set; }

        public string GameroomLocation { get; set; }

        public string KitchenLocation { get; set; }

        public string LivingRoomLocation { get; set; }

        public string MediaRoomLocation { get; set; }

        public string StudyLocation { get; set; }

        public string UtilityLocation { get; set; }

        public bool HasBonusWithAmount { get; set; }

        public string AgentBonusAmount { get; set; }

        public string AgentBonusAmountType { get; set; }

        public string AgentPrivateRemarks { get; set; }

        public string SMARTFEATURESAPP { get; set; }

        public string ProposedTerms { get; set; }

        public string Exemptions { get; set; }

        public string DistanceToWaterAccess { get; set; }

        public string Bath1Dim { get; set; }

        public string Bath1Location { get; set; }

        public string RoomDescription { get; set; }

        public string BedroomDescription { get; set; }

        public string KitchenDescription { get; set; }

        public string SellingAgent2ID { get; set; }

        public string SellTeamID { get; set; }

        public string SellingAgentSupervisor { get; set; }

        public int? ResidentialLeaseID { get; set; }

        public Guid ResidentialLeaseRequestID { get; set; }

        public string AppFee { get; set; }

        public string LotSizeAcres { get; set; }

        public string NonRefunPetFee { get; set; }

        public string PetPolicy { get; set; }

        public string Date { get; set; }

        public DateTime? MoveInDate { get; set; }

        public string ApplicationFeePay { get; set; }

        public string WillSubdivide { get; set; }

        public string NumberGuestAllowed { get; set; }

        public string LeaseTerms { get; set; }

        public string NumberOfVehicles { get; set; }

        public string AppFeeAmount { get; set; }

        public string NumberOfPetsAllowed { get; set; }

        public string DepositPet { get; set; }

        public DateTime? LeasedDate { get; set; }

        public decimal LeasedPrice { get; set; }

        public int? Furnished { get; set; }

        public string FloorLocationNumber { get; set; }

        public string DepositAmount { get; set; }

        public string LeaseConditions { get; set; }

        public string TenantPays { get; set; }

        public string MoniesRequired { get; set; }

        public string LeaseType { get; set; }

        public string MonthlyPetFee { get; set; }

        public int AppliancesYN { get; set; }

        public string CompensationPaid { get; set; }

        public string CancelledOptionLease { get; set; }

        public string IndividualOH { get; set; }

        public bool AllowPendingList { get; set; }

        public bool? FencedYard { get; set; }

        public string TypeFence { get; set; }

        public string CommissionLease { get; set; }

        public bool? AgentCommissionPercentYN { get; set; }

        public bool? AgentCommissionDollarsYN { get; set; }

        public string YearBuiltLease { get; set; }

        public string LeaseStatus { get; set; }

        public bool? ChangeOpenHouseHours { get; set; }

        public string CommunityCSS { get; set; }

        public int InternalLotRequestID { get; set; }

        public Guid InternalLotRequestGUID { get; set; }

        public int? InternalLotID { get; set; }

        public string PresentUse { get; set; }

        public string RoadFrontageDesc { get; set; }

        public string LotFeatures { get; set; }

        public string ProposedUse { get; set; }

        public string Documents { get; set; }

        public string Development { get; set; }

        public string Topography { get; set; }

        public string WaterfrontFeatures { get; set; }

        public string Easements { get; set; }

        public string ZoningLot { get; set; }

        public string Utilities { get; set; }

        public string UtilitiesOther { get; set; }

        public string WaterfrontYN { get; set; }

        public bool? RoadAssessmentYN { get; set; }

        public string ParcelNumber { get; set; }

        public string KeyboxNumber { get; set; }

        public int? NumberOfLakes { get; set; }

        public int? NumberOfWaterMeters { get; set; }

        public string CancelledOption { get; set; }

        public string KickOutInfo { get; set; }

        public string MLSNumLot { get; set; }

        public string RateYear { get; set; }

        public string ETJDesc { get; set; }

        public bool? HasGas { get; set; }

        public string WaterExtras { get; set; }

        public string WaterView { get; set; }

        public string BedRoom2Level { get; set; }

        public string BedRoom3Level { get; set; }

        public string BedRoom4Level { get; set; }

        public string BedRoom2Dim { get; set; }

        public string BedRoom3Dim { get; set; }

        public string BedRoom4Dim { get; set; }

        public string RentIncludes { get; set; }

        public string ExteriorFeatures { get; set; }

        public string MinNumMonths { get; set; }

        public string MaxNumMonths { get; set; }

        public string DepositSecurity { get; set; }

        public string DepositClean { get; set; }

        public string PetDepositRefund { get; set; }

        public string PetsAllowed { get; set; }

        public string ApplyAt { get; set; }

        public string ApplForm { get; set; }

        public string PersonalChecksAccepted { get; set; }

        public string CashAccepted { get; set; }

        public string CountyTax { get; set; }

        public string CityTax { get; set; }

        public string SchoolTax { get; set; }

        public string OtherTax { get; set; }

        public bool EnableOpenHouse { get; set; }

        public string Financing { get; set; }

        public int? Mbr2Len { get; set; }

        public int? Mbr2Wid { get; set; }

        public string MBR2LEVEL { get; set; }

        public string ManagementCompany { get; set; }

        public string SecondAgentMarketUniqueId { get; set; }

        public string AgentPrivateRemarks2 { get; set; }

        public string Bed6Location { get; set; }

        public bool InExtraTerritorialJurisdiction { get; set; }

        public bool IsManufacturedAllowed { get; set; }

        public string GeographicID { get; set; }

        public string EarnestMoney { get; set; }

        public string ProspectsExempt { get; set; }

        public bool UpdateGeocodes { get; set; }

        public string GreenIndoorAirQuality { get; set; }

        public string TopoLandDescription { get; set; }

        public string UpgradedEnergyFeatures { get; set; }

        public string GreenWaterConservation { get; set; }

        public string AtticRoom { get; set; }

        public bool HasBuyerAgent { get; set; }

        public bool HasSecondBuyerAgent { get; set; }

        public string TitlePaidBy { get; set; }

        public string RepairsPaidBySeller { get; set; }
        public string FemaFloodPlain { get; set; }
        public List<ResidentialListingRequestRoom> Rooms { get; set; }

        public List<OpenHouseRequest> OpenHouse { get; set; }

        public abstract BuiltStatus BuiltStatus { get; }

        public abstract ResidentialListingRequest CreateFromApiResponse();

        public abstract ResidentialListingRequest CreateFromApiResponseDetail();

        public UploadListingItem AsUploadItem(
            string builderName,
            string brokerOffice,
            string isLeasing,
            string isLot,
            Entity currentEntity,
            string worker,
            string workingStatus,
            string workingSourceAction) => new()
            {
                RequestId = this.ResidentialListingRequestID,
                MlsNumber = this.IsNewListing ? $"New {currentEntity}" : this.MLSNum,
                Address = this.Address,
                Status = !string.IsNullOrEmpty(this.ListStatus) ? this.ListStatus : string.Empty,
                Market = this.MarketName,
                CompanyName = this.CompanyName,
                BuilderName = builderName,
                BrokerOffice = brokerOffice,
                FullListing = this,
                UnitNumber = this.UnitNum,
                IsLeasing = isLeasing,
                IsLot = isLot,
                InternalLotRequestId = this.InternalLotRequestID,
                WorkingBy = worker,
                WorkingStatus = workingStatus,
                WorkingSourceAction = workingSourceAction,
            };

        public virtual string GetAgentRemarksMessage()
        {
            var privateRemarks = "LIMITED SERVICE LISTING: Buyer verifies dimensions & ISD info. Use Bldr contract.";

            var bonusMessage = this.GetAgentBonusRemarksMessage();
            if (!string.IsNullOrWhiteSpace(bonusMessage))
            {
                privateRemarks = $"{bonusMessage} {privateRemarks}";
            }

            var saleOfficeInfo = this.GetSalesAssociateRemarksMessage();
            if (!string.IsNullOrWhiteSpace(saleOfficeInfo))
            {
                privateRemarks += $" {saleOfficeInfo}";
            }

            if (!string.IsNullOrWhiteSpace(this.PlanProfileName))
            {
                privateRemarks += $" Plan: {this.PlanProfileName}.";
            }

            if (!string.IsNullOrWhiteSpace(this.RealtorContactEmail))
            {
                privateRemarks += $" Email contact: {this.RealtorContactEmail}.";
            }

            const string homeUnderConstruction = "Home is under construction. For your safety, call appt number for showings.";

            return this.BuiltStatus != BuiltStatus.ReadyNow ? $"{homeUnderConstruction} {privateRemarks}" : privateRemarks;
        }

        public virtual string GetSalesOfficeAddressRemarkMessage()
        {
            if (!this.CommunityProfileSalesOfficeStreetNum.HasValue || string.IsNullOrWhiteSpace(this.CommunityProfileSalesOfficeStreetName))
            {
                return string.Empty;
            }

            string soCity = this.CommunityProfileSalesOfficeCity;
            string soZip = this.CommunityProfileSalesOfficeZip;
            return $"Sales Office at {this.CommunityProfileSalesOfficeStreetNum.Value} {this.CommunityProfileSalesOfficeStreetName}"
                               + (!string.IsNullOrWhiteSpace(soCity) ? ", " + soCity : string.Empty)
                               + (!string.IsNullOrWhiteSpace(soZip) ? ", " + soZip : string.Empty)
                               + ".";
        }

        public virtual string GetSalesAssociateRemarksMessage()
        {
            var salesOfficeAddr = this.GetSalesOfficeAddressRemarkMessage();
            var phones = new List<string>();

            if (!string.IsNullOrWhiteSpace(this.AgentListApptPhone))
            {
                phones.Add(this.AgentListApptPhone.PhoneFormat());
            }

            if (!string.IsNullOrWhiteSpace(this.OtherPhone))
            {
                phones.Add(this.OtherPhone.PhoneFormat());
            }

            return phones.Any()
                ? string.Format("For more information call {0}. {1}.", string.Join(" or ", phones), salesOfficeAddr)
                : salesOfficeAddr;
        }

        public virtual string GetAgentBonusAmount()
        {
            if (this.HasBonusWithAmount && !string.IsNullOrWhiteSpace(this.AgentBonusAmountType) && decimal.TryParse(this.AgentBonusAmount, out decimal agentBonusAmount))
            {
                return this.AgentBonusAmountType == DollarSign ? string.Format("${0:n2}", agentBonusAmount) : string.Format("{0}%", agentBonusAmount);
            }

            return string.Empty;
        }

        public virtual string GetAgentBonusRemarksMessage()
        {
            var hasBuyerIncentive = this.BuyerCheckBox.HasValue && this.BuyerCheckBox.Value;

            if (this.HasAgentBonus.HasValue && this.HasAgentBonus.Value)
            {
                return hasBuyerIncentive
                    ? "Contact Builder for Seller Concession & Buyer Incentive Information. "
                    : "Contact Builder for Seller Concession Information. ";
            }

            if (hasBuyerIncentive)
            {
                return "Contact Builder for Buyer Incentive Information. ";
            }

            return string.Empty;
        }

        public virtual string GetPublicRemarks()
        {
            var builtNote = !string.IsNullOrWhiteSpace(this.MLSNum) ? $"MLS# {this.MLSNum}" : "MLS# ";

            if (!string.IsNullOrWhiteSpace(this.CompanyName))
            {
                builtNote += !string.IsNullOrWhiteSpace(builtNote) ? " - " : string.Empty;
                builtNote += "Built by " + this.CompanyName + " - ";
            }

            switch (this.BuiltStatus)
            {
                case BuiltStatus.ToBeBuilt:
                    if (this.BuildCompletionDate.HasValue)
                    {
                        builtNote += this.BuildCompletionDate.Value.ToString("MMMM") + " completion ~ ";
                    }

                    break;

                case BuiltStatus.ReadyNow:
                    builtNote += "Ready Now! ~ ";

                    break;

                case BuiltStatus.WithCompletion:
                    if (this.BuildCompletionDate.HasValue)
                    {
                        builtNote += this.BuildCompletionDate.Value.ToString("MMMM") + " completion! ~ ";
                    }

                    break;

                default:
                    break;
            }

            return GetRemarks();

            string GetRemarks()
            {
                string remark;

                if (this.IncludeRemarks != null && this.IncludeRemarks == false)
                {
                    builtNote = string.Empty;
                }

                if (string.IsNullOrWhiteSpace(this.PublicRemarks) || !this.PublicRemarks.Contains('~'))
                {
                    remark = (builtNote + (this.PublicRemarks ?? string.Empty)).RemoveSlash();
                }
                else
                {
                    var tempIndex = this.PublicRemarks.IndexOf("~", StringComparison.Ordinal) + 1;
                    var temp = this.PublicRemarks[tempIndex..].Trim();
                    remark = (builtNote + temp).RemoveSlash();
                }

                return remark.Replace("\t", string.Empty).Replace("\n", " ");
            }
        }
    }
}
