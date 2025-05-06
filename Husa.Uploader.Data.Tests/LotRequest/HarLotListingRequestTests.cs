namespace Husa.Uploader.Data.Tests.Entities.MarketRequests.LotRequest
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Har.Api.Contracts.Response.ListingRequest.LotRequest;
    using Husa.Quicklister.Har.Domain.Enums;
    using Husa.Quicklister.Har.Domain.Enums.Domain;
    using Husa.Uploader.Data.Entities.MarketRequests.LotRequest;
    using Xunit;
    public class HarLotListingRequestTests
    {
        [Fact]
        public void Constructor_WithListingLotRequestQueryResponse_ThrowsOnNull()
        {
            // Arrange & Act
            var act = () => new HarLotListingRequest(listingResponse: null);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void Constructor_WithLotListingRequestDetailResponse_ThrowsOnNull()
        {
            // Arrange & Act
            var act = () => new HarLotListingRequest(listingDetailResponse: null);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void MarketCode_ShouldReturnHar()
        {
            // Arrange
            var sut = new HarLotListingRequest(new ListingLotRequestQueryResponse());

            // Act
            var result = sut.MarketCode;

            // Assert
            Assert.Equal(MarketCode.Houston, result);
        }

        [Fact]
        public void CreateFromApiResponse_ReturnsLotListingRequest()
        {
            // Arrange
            var apiDate = new DateTime(2023, 05, 29, 0, 0, 0, DateTimeKind.Utc);
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var listingResponse = new ListingLotRequestQueryResponse
            {
                Id = requestId,
                OwnerName = "Test Owner",
                MlsNumber = "MLS123",
                Market = MarketCode.Houston.ToString(),
                City = Cities.Houston,
                Subdivision = "Test Subdivision",
                ZipCode = "78701",
                Address = "123 Test St",
                ListPrice = 150000,
                MlsStatus = MarketStatuses.Active,
                SysCreatedOn = apiDate,
                SysCreatedBy = userId,
                UpdateGeocodes = true,
            };

            var sut = new HarLotListingRequest(listingResponse);

            // Act
            var result = sut.CreateFromApiResponse();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HarLotListingRequest>(result);
            Assert.Equal(listingResponse.Id, result.LotListingRequestID);
        }

        [Fact]
        public void CreateFromApiResponse_MapsAllPropertiesCorrectly()
        {
            // Arrange
            var apiDate = new DateTime(2023, 05, 29, 0, 0, 0, DateTimeKind.Utc);
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var price = 175000;

            var listingResponse = new ListingLotRequestQueryResponse
            {
                Id = requestId,
                OwnerName = "Test Owner",
                MlsNumber = "MLS456",
                Market = MarketCode.Houston.ToString(),
                City = Cities.Houston,
                Subdivision = "Test Subdivision",
                ZipCode = "78701",
                Address = "123 Test St",
                ListPrice = price,
                MlsStatus = MarketStatuses.Pending,
                SysCreatedOn = apiDate,
                SysCreatedBy = userId,
                UpdateGeocodes = true,
            };

            var sut = new HarLotListingRequest(listingResponse);

            // Act
            var result = sut.CreateFromApiResponse();

            // Assert
            Assert.Equal(requestId, result.LotListingRequestID);
            Assert.Equal("Test Owner", result.CompanyName);
            Assert.Equal("MLS456", result.MLSNum);
            Assert.Equal(MarketCode.Houston.ToString(), result.MarketName);
            Assert.Equal(Cities.Houston.ToString(), result.CityCode);
            Assert.Equal("Test Subdivision", result.Subdivision);
            Assert.Equal("78701", result.Zip);
            Assert.Equal("123 Test St", result.Address);
            Assert.Equal(price, result.ListPrice);
            Assert.Equal(apiDate, result.SysCreatedOn);
            Assert.Equal(userId, result.SysCreatedBy);
            Assert.True(result.UpdateGeocodes);
        }

        [Fact]
        public void CreateFromApiResponseDetail_ReturnsLotListingRequest()
        {
            // Arrange
            var apiDate = new DateTime(2023, 05, 29, 0, 0, 0, DateTimeKind.Utc);
            var companyId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var listingDetailResponse = new LotListingRequestDetailResponse
            {
                Id = requestId,
                MlsNumber = "MLS789",
                MlsStatus = MarketStatuses.Active,
                ListPrice = 150000,
                SysCreatedOn = apiDate.AddMonths(-1),
                SysCreatedBy = userId,
                SysModifiedOn = apiDate,
                SysModifiedBy = userId,
                ExpirationDate = apiDate.AddMonths(6),
                OwnerName = "Detail Owner",
                CompanyId = companyId,
            };

            var sut = new HarLotListingRequest(listingDetailResponse);

            // Act
            var result = sut.CreateFromApiResponseDetail();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HarLotListingRequest>(result);
            Assert.Equal(listingDetailResponse.Id, result.LotListingRequestID);
        }

        [Fact]
        public void CreateFromApiResponseDetail_MapsAllPropertiesCorrectly()
        {
            // Arrange
            var apiDate = new DateTime(2023, 05, 29, 0, 0, 0, DateTimeKind.Utc);
            var companyId = Guid.NewGuid();
            var requestId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var price = 225000;

            var listingDetailResponse = new LotListingRequestDetailResponse
            {
                Id = requestId,
                MlsNumber = "MLS101",
                MlsStatus = MarketStatuses.Active,
                ListPrice = price,
                SysCreatedOn = apiDate.AddMonths(-1),
                SysCreatedBy = userId,
                SysModifiedOn = apiDate,
                SysModifiedBy = userId,
                ExpirationDate = apiDate.AddMonths(6),
                OwnerName = "Detail Owner",
                CompanyId = companyId,
            };

            var sut = new HarLotListingRequest(listingDetailResponse);

            // Act
            var result = sut.CreateFromApiResponseDetail();

            // Assert
            Assert.Equal(requestId, result.LotListingRequestID);
            Assert.Equal("MLS101", result.MLSNum);
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
        }
    }
}
