namespace Husa.Uploader.Data.Entities.LotListing
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Extensions;

    public abstract class LotListingRequest
    {
        public const string DollarSign = "$";
        private string agentListApptPhone;

        // Never remove this property
        public string WorkingBy { get; set; }
        public bool IsNewListing => string.IsNullOrWhiteSpace(this.MLSNum);
        public string CompanyName { get; set; }
        public Guid CompanyId { get; set; }
        public Guid LotListingRequestID { get; set; }
        public DateTime? SysCreatedOn { get; set; }
        public Guid? SysCreatedBy { get; set; }
        public Guid? SysModifiedBy { get; set; }
        public DateTime? SysModifiedOn { get; set; }
        public string BuilderName { get; set; }
        public string Address { get; set; }
        public string MLSNum { get; set; }
        public string ListStatus { get; set; }
        public string MarketName { get; set; }
        public abstract MarketCode MarketCode { get; }
        public int InternalLotRequestID { get; set; }
        public Guid InternalLotRequestGUID { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string ExpectedActiveDate { get; set; }
        public string AgentListApptPhone { get => this.agentListApptPhone.PhoneFormat(); set => this.agentListApptPhone = value; }

        public string StreetNum { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Subdivision { get; set; }
        public string OtherFees { get; set; }

        public int ListPrice { get; set; }
        public DateTime? ListDate { get; set; }

        // Lot Schools
        public string SchoolDistrict { get; set; }
        public string SchoolName1 { get; set; }
        public string SchoolName2 { get; set; }
        public string HighSchool { get; set; }
        public string SchoolName4 { get; set; }
        public string SchoolName5 { get; set; }
        public string SchoolName6 { get; set; }

        // Lot Address
        public string UnitNumber { get; set; }

        // Lot Property
        public string MlsArea { get; set; }
        public string PropertyType { get; set; }
        public string FemaFloodPlain { get; set; }
        public string LotDescription { get; set; }
        public string PropCondition { get; set; }
        public string PropertySubType { get; set; }
        public string TypeOfHomeAllowed { get; set; }
        public string SoilType { get; set; }
        public bool SurfaceWater { get; set; }
        public int? NumberOfPonds { get; set; }
        public int? NumberOfWells { get; set; }
        public bool LiveStock { get; set; }
        public bool CommercialAllowed { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string LegalDescription { get; set; }
        public string TaxLot { get; set; }
        public string TaxId { get; set; }
        public string TaxBlock { get; set; }
        public string LotDimension { get; set; }
        public string LotSize { get; set; }
        public bool UpdateGeocodes { get; set; }
        public int? AlsoListedAs { get; set; }
        public bool BuilderRestrictions { get; set; }

        // Lot Features
        public string RestrictionsDescription { get; set; }
        public string WaterfrontFeatures { get; set; }
        public string View { get; set; }
        public string WaterSewer { get; set; }
        public string UtilitiesDescription { get; set; }
        public string WaterSource { get; set; }
        public string DistanceToWaterAccess { get; set; }
        public string Fencing { get; set; }
        public string ExteriorFeatures { get; set; }
        public bool GroundWaterConservDistric { get; set; }
        public string HorseAmenities { get; set; }
        public string MineralsFeatures { get; set; }
        public string RoadSurface { get; set; }
        public string OtherStructures { get; set; }
        public string NeighborhoodAmenities { get; set; }
        public string Disclosures { get; set; }
        public string DocumentsAvailable { get; set; }
        public string WaterBodyName { get; set; }

        // Lot Finantial
        public bool HasHoa { get; set; }
        public string HoaName { get; set; }
        public string HoaFee { get; set; }
        public string HOARequirement { get; set; }
        public string BillingFrequency { get; set; }
        public string HoaIncludes { get; set; }
        public string AcceptableFinancing { get; set; }
        public string EstimatedTax { get; set; }
        public string TaxYear { get; set; }
        public string TaxAssesedValue { get; set; }
        public string TaxRate { get; set; }
        public string TaxExemptions { get; set; }
        public string LandTitleEvidence { get; set; }
        public string PreferredTitleCompany { get; set; }
        public bool HasBuyerIncentive { get; set; }
        public decimal? BuyersAgentCommission { get; set; }
        public string BuyersAgentCommissionType { get; set; }
        public bool HasAgentBonus { get; set; }
        public bool HasBonusWithAmount { get; set; }
        public string AgentBonusAmount { get; set; }
        public string AgentBonusAmountType { get; set; }
        public DateTime? BonusExpirationDate { get; set; }

        // Lot Financial
        public string OwnerName { get; set; }
        public string ShowingRequirements { get; set; }
        public string ApptPhone { get; set; }
        public string ShowingServicePhone { get; set; }
        public string ShowingInstructions { get; set; }
        public string PublicRemarks { get; set; }
        public string Directions { get; set; }
        public string ShowingContactType { get; set; }
        public string ShowingContactName { get; set; }

        public abstract LotListingRequest CreateFromApiResponse();

        public abstract LotListingRequest CreateFromApiResponseDetail();

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
                RequestId = this.LotListingRequestID,
                MlsNumber = this.IsNewListing ? $"New {currentEntity}" : this.MLSNum,
                Address = this.Address,
                Status = !string.IsNullOrEmpty(this.ListStatus) ? this.ListStatus : string.Empty,
                Market = this.MarketName,
                CompanyName = this.CompanyName,
                BuilderName = builderName,
                BrokerOffice = brokerOffice,
                FullLotListing = this,
                UnitNumber = this.UnitNumber,
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

            return privateRemarks;
        }

        public virtual string GetSalesAssociateRemarksMessage()
        {
            var phones = new List<string>();

            if (!string.IsNullOrWhiteSpace(this.AgentListApptPhone))
            {
                phones.Add(this.AgentListApptPhone.PhoneFormat());
            }

            if (!string.IsNullOrWhiteSpace(this.ShowingServicePhone))
            {
                phones.Add(this.ShowingServicePhone.PhoneFormat());
            }

            return phones.Any()
                ? string.Format("For more information call {0}. {1}.", string.Join(" or ", phones), string.Empty)
                : string.Empty;
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
            var hasBuyerIncentive = this.HasBuyerIncentive;

            if (this.HasAgentBonus)
            {
                return hasBuyerIncentive
                    ? "Contact Builder for Bonus & Buyer Incentive Information. "
                    : "Contact Builder for Bonus Information. ";
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

            return GetRemarks();

            string GetRemarks()
            {
                string remark;

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
