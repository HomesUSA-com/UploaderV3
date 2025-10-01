namespace Husa.Uploader.Data.Tests.Entities.MarketRequests.LotRequest
{
    using System;
    using Husa.Extensions.Common;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.CTX.Api.Contracts.Response;
    using Husa.Quicklister.CTX.Api.Contracts.Response.ListingRequest.LotRequest;
    using Husa.Quicklister.CTX.Api.Contracts.Response.LotListing;
    using Husa.Quicklister.CTX.Api.Contracts.Response.SalePropertyDetail;
    using Husa.Quicklister.CTX.Domain.Enums;
    using Husa.Quicklister.CTX.Domain.Enums.Entities;
    using Husa.Uploader.Crosscutting.Converters;
    using Husa.Uploader.Data.Entities.MarketRequests.LotRequest;
    using Xunit;

    public class CtxLotListingRequestTests
    {
        [Fact]
        public void Constructor_WithListingLotRequestQueryResponse_ThrowsOnNull()
        {
            // Arrange & Act
            var act = () => new CtxLotListingRequest(listingResponse: null);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Constructor_WithLotListingRequestDetailResponse_ThrowsOnNull()
        {
            // Arrange & Act
            var act = () => new CtxLotListingRequest(listingDetailResponse: null);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void MarketCode_ShouldReturnCtx()
        {
            // Arrange
            var sut = new CtxLotListingRequest(new ListingLotRequestQueryResponse());

            // Act
            var result = sut.MarketCode;

            // Assert
            Assert.Equal(MarketCode.CTX, result);
        }

        [Fact]
        public void CreateFromApiResponse_ReturnsLotListingRequest()
        {
            // Arrange
            var apiDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var listingResponse = new ListingLotRequestQueryResponse
            {
                Id = requestId,
                OwnerName = "Test Owner",
                MlsNumber = "CTX123",
                Market = MarketCode.CTX.ToString(),
                City = Cities.Austin,
                Subdivision = "Test Subdivision",
                ZipCode = "73301",
                Address = "456 Test Ave",
                ListPrice = 200000,
                MlsStatus = MarketStatuses.Active,
                SysCreatedOn = apiDate,
                SysCreatedBy = userId,
                UpdateGeocodes = true,
            };

            var sut = new CtxLotListingRequest(listingResponse);

            // Act
            var result = sut.CreateFromApiResponse();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CtxLotListingRequest>(result);
            Assert.Equal(listingResponse.Id, result.LotListingRequestID);
        }

        [Fact]
        public void CreateFromApiResponse_MapsAllPropertiesCorrectly()
        {
            // Arrange
            var apiDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var price = 210000;

            var listingResponse = new ListingLotRequestQueryResponse
            {
                Id = requestId,
                OwnerName = "Test Owner",
                MlsNumber = "CTX456",
                Market = MarketCode.CTX.ToString(),
                City = Cities.Austin,
                Subdivision = "Test Subdivision",
                ZipCode = "73301",
                Address = "456 Test Ave",
                ListPrice = price,
                MlsStatus = MarketStatuses.Pending,
                SysCreatedOn = apiDate,
                SysCreatedBy = userId,
                UpdateGeocodes = true,
            };

            var sut = new CtxLotListingRequest(listingResponse);

            // Act
            var result = sut.CreateFromApiResponse();

            // Assert
            Assert.Equal(requestId, result.LotListingRequestID);
            Assert.Equal("Test Owner", result.CompanyName);
            Assert.Equal("CTX456", result.MLSNum);
            Assert.Equal(MarketCode.CTX.ToString(), result.MarketName);
            Assert.Equal(Cities.Austin.ToStringFromEnumMember(), result.CityCode);
            Assert.Equal("Test Subdivision", result.Subdivision);
            Assert.Equal("73301", result.Zip);
            Assert.Equal("456 Test Ave", result.Address);
            Assert.Equal(price, result.ListPrice);
            Assert.Equal(apiDate, result.SysCreatedOn);
            Assert.Equal(userId, result.SysCreatedBy);
            Assert.True(result.UpdateGeocodes);
        }

        [Fact]
        public void CreateFromApiResponseDetail_ReturnsLotListingRequest()
        {
            // Arrange
            var apiDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var companyId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var listingDetailResponse = new LotListingRequestDetailResponse
            {
                Id = requestId,
                MlsNumber = "CTX789",
                MlsStatus = MarketStatuses.Active,
                ListPrice = 220000,
                SysCreatedOn = apiDate.AddMonths(-1),
                SysCreatedBy = userId,
                SysModifiedOn = apiDate,
                SysModifiedBy = userId,
                ExpirationDate = apiDate.AddMonths(6),
                OwnerName = "Detail Owner",
                CompanyId = companyId,
                AddressInfo = new AddressInfoResponse(),
                PropertyInfo = new LotPropertyResponse(),
                ShowingInfo = new LotShowingResponse(),
                FinancialInfo = new LotFinancialResponse(),
                FeaturesInfo = new LotFeaturesResponse(),
                SchoolsInfo = new SchoolsResponse(),
                StatusFieldsInfo = new ListingSaleStatusFieldsResponse(),
            };

            var sut = new CtxLotListingRequest(listingDetailResponse);

            // Act
            var result = sut.CreateFromApiResponseDetail();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CtxLotListingRequest>(result);
            Assert.Equal(listingDetailResponse.Id, result.LotListingRequestID);
        }

        [Fact]
        public void CreateFromApiResponseDetail_MapsAllPropertiesCorrectly()
        {
            // Arrange
            var apiDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var companyId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var price = 225000;

            var addressInfo = new AddressInfoResponse
            {
                StreetNumber = "100",
                StreetName = "Main St",
                City = Cities.Austin,
                State = States.Texas,
                ZipCode = "73301",
                County = Counties.Travis,
                StreetType = StreetType.Street,
                LotNum = "5",
                Subdivision = "Central Sub",
                StreetDirection = StreetDirectionType.North,
            };

            var propertyInfo = new LotPropertyResponse
            {
                LegalDescription = "Lot 5, Block A",
                UpdateGeocodes = true,
                TaxId = "123456789",
                Latitude = 30.2672m,
                Longitude = -97.7431m,
                ListingType = ListingType.LimitedServiceListing,
                TypeCategory = LotPropertySubType.ResidentialLots,
                FemaFloodPlain = FemaFloodPlain.Unknown,
            };

            var featuresInfo = new LotFeaturesResponse
            {
                LotDimension = "50x100",
                LotSize = "5000",
                ExteriorFeatures = new[] { ExteriorFeaturesDescription.BalconyCovered },
                Fencing = new[] { FencingDescription.Wood },
                WaterFeatures = new[] { WaterDescription.None },
                MineralRights = new[] { MineralRights.No },
                RestrictionsType = new[] { RestrictionsDescription.None },
                NeighborhoodAmenities = new[] { NeighborhoodAmenitiesDescription.WalkingJoggingBikeTrails },
                WaterSewer = new[] { WaterSewerDescription.CityWater },
            };

            var financialInfo = new LotFinancialResponse
            {
                TaxRate = 2.1m,
                TaxYear = 2023,
                HoaRequirement = HOARequirement.Mandatory,
                HoaName = "Central HOA",
            };

            var showingInfo = new LotShowingResponse
            {
                BuyersAgentCommission = 3,
                Showing = new[] { ShowingInstructionsDescription.CallFirstGo },
                Directions = "Go north on Main St.",
                PublicRemarks = "Great lot in central Austin.",
            };

            var schoolsInfo = new SchoolsResponse
            {
                SchoolDistrict = SchoolDistrict.Andrews,
                HighSchool = HighSchool.AkinsHighSchool,
            };

            var listingDetailResponse = new LotListingRequestDetailResponse
            {
                Id = requestId,
                MlsNumber = "CTX101",
                MlsStatus = MarketStatuses.Active,
                ListPrice = price,
                SysCreatedOn = apiDate.AddMonths(-1),
                SysCreatedBy = userId,
                SysModifiedOn = apiDate,
                SysModifiedBy = userId,
                ExpirationDate = apiDate.AddMonths(6),
                OwnerName = "Detail Owner",
                CompanyId = companyId,
                AddressInfo = addressInfo,
                PropertyInfo = propertyInfo,
                ShowingInfo = showingInfo,
                FinancialInfo = financialInfo,
                FeaturesInfo = featuresInfo,
                SchoolsInfo = schoolsInfo,
                StatusFieldsInfo = new ListingSaleStatusFieldsResponse(),
            };

            var sut = new CtxLotListingRequest(listingDetailResponse);

            // Act
            var result = sut.CreateFromApiResponseDetail();

            // Assert
            Assert.Equal(requestId, result.LotListingRequestID);
            Assert.Equal("CTX101", result.MLSNum);
            Assert.Equal(price, result.ListPrice);
            Assert.Equal(apiDate.AddMonths(-1), result.SysCreatedOn);
            Assert.Equal(userId, result.SysCreatedBy);
            Assert.Equal(apiDate, result.SysModifiedOn);
            Assert.Equal(userId, result.SysModifiedBy);
            Assert.Equal("Detail Owner", result.BuilderName);
            Assert.Equal("Detail Owner", result.CompanyName);
            Assert.Equal("Detail Owner", result.OwnerName);
            Assert.Equal(companyId, result.CompanyId);
            Assert.Equal(listingDetailResponse.ExpirationDate, result.ExpiredDate);

            // AddressInfo
            Assert.Equal("100", result.StreetNum);
            Assert.Equal("Main St", result.StreetName);
            Assert.Equal(Cities.Austin.ToStringFromEnumMember(), result.CityCode);
            Assert.Equal(5, result.LotNumber);
            Assert.Equal("Central Sub", result.Subdivision);

            // PropertyInfo
            Assert.Equal("Lot 5, Block A", result.LegalDescription);
            Assert.True(result.UpdateGeocodes);
            Assert.Equal("123456789", result.TaxId);
            Assert.Equal(30.2672m, result.Latitude);
            Assert.Equal(-97.7431m, result.Longitude);

            // FeaturesInfo
            Assert.Equal("50x100", result.LotDimension);
            Assert.Equal("5000", result.LotSize);

            // FinancialInfo
            Assert.Equal("2", result.TaxRate);
            Assert.Equal("2023", result.TaxYear);
            Assert.Equal(HOARequirement.Mandatory.ToStringFromHOARequirementCTX(), result.HOA);
            Assert.Equal("Central HOA", result.AssocName);

            // ShowingInfo
            Assert.Equal(3, result.BuyersAgentCommission);
            Assert.Equal("Go north on Main St.", result.Directions);
            Assert.Equal("Great lot in central Austin.", result.PublicRemarks);

            // SchoolsInfo
            Assert.Equal(SchoolDistrict.Andrews.ToStringFromEnumMember(), result.SchoolDistrict);
            Assert.Equal(HighSchool.AkinsHighSchool.ToStringFromEnumMember(), result.HighSchool);
        }
    }
}
