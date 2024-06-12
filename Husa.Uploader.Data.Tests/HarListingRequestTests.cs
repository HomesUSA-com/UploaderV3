namespace Husa.Uploader.Data.Tests
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Har.Api.Contracts.Response;
    using Husa.Quicklister.Har.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Quicklister.Har.Domain.Enums;
    using Husa.Quicklister.Har.Domain.Enums.Domain;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Xunit;

    public class HarListingRequestTests
    {
        public object Market { get; private set; }

        [Fact]
        public void CreateHarListingRequest_ThrowsOnNullListingSaleQueryResponse()
        {
            // Arrange
            // Act
            var sut = () => new HarListingRequest(listingResponse: null);

            // Assert
            Assert.Throws<ArgumentNullException>(sut);
        }

        [Fact]
        public void CreateHarListingRequest_ThrowsOnNullListingSaleRequestDetailResponse()
        {
            // Arrange
            // Act
            var sut = () => new HarListingRequest(listingDetailResponse: null);

            // Assert
            Assert.Throws<ArgumentNullException>(sut);
        }

        [Fact]
        public void CreateFromApiResponse_ReturnsResidentialListingRequest()
        {
            // Arrange
            var apiDate = new DateTime(2023, 05, 29, 0, 0, 0, DateTimeKind.Utc);
            var requestId = new Guid("7b6eb659-da6a-436d-a9c1-22d8fcbdfaae");
            var userId = new Guid("7b6eb659-a9c1-da6a-436d-22d8fcbdfaae");
            var listingResponse = new ListingSaleRequestQueryResponse
            {
                Id = requestId,
                OwnerName = "John Doe",
                MlsNumber = "MLS123",
                Market = MarketCode.Houston.ToString(),
                City = Cities.Austin,
                Subdivision = "Subdivision 1",
                ZipCode = "78123",
                Address = "123 Main St",
                ListPrice = 123456,
                SysCreatedOn = apiDate,
                SysCreatedBy = userId,
            };
            var sut = new HarListingRequest(listingResponse);

            // Act
            var result = sut.CreateFromApiResponse();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HarListingRequest>(result);
            Assert.Equal(listingResponse.Id, result.ResidentialListingRequestID);
        }

        [Fact]
        public void CreateFromApiResponseDetail_ReturnsResidentialListingRequest()
        {
            // Arrange
            var apiDate = new DateTime(2023, 05, 29, 0, 0, 0, DateTimeKind.Utc);
            var listingId = new Guid("7b6eb659-436d-da6a-a9c1-22d8fcbdfaae");
            var requestId = new Guid("7b6eb659-da6a-436d-a9c1-22d8fcbdfaae");
            var userId = new Guid("7b6eb659-a9c1-da6a-436d-22d8fcbdfaae");

            var listingResponse = new ListingSaleRequestDetailResponse
            {
                Id = requestId,
                ListingSaleId = listingId,
                ListPrice = 1234567,
                MlsNumber = "1231231",
                MlsStatus = MarketStatuses.Active,
                SysCreatedOn = apiDate.AddMonths(-1),
                SysCreatedBy = userId,
                SysModifiedOn = apiDate,
                SysModifiedBy = userId,
                StatusFieldsInfo = new(),
                SaleProperty = new()
                {
                    SalePropertyInfo = new(),
                    AddressInfo = new(),
                    PropertyInfo = new(),
                    SpacesDimensionsInfo = new(),
                    FeaturesInfo = new(),
                    FinancialInfo = new(),
                    ShowingInfo = new(),
                    SchoolsInfo = new(),
                    Rooms = new List<RoomResponse>(),
                    OpenHouses = new List<OpenHouseResponse>(),
                },
            };

            var sut = new HarListingRequest(listingResponse);

            // Act
            var result = sut.CreateFromApiResponseDetail();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HarListingRequest>(result);
            Assert.Equal(listingResponse.Id, result.ResidentialListingRequestID);
        }

        [Theory]
        [InlineData(false, true, true, true, true, true, true, true)]
        [InlineData(true, false, true, true, true, true, true, true)]
        [InlineData(true, true, false, true, true, true, true, true)]
        [InlineData(true, true, true, false, true, true, true, true)]
        [InlineData(true, true, true, true, false, true, true, true)]
        [InlineData(true, true, true, true, true, false, true, true)]
        [InlineData(true, true, true, true, true, true, false, true)]
        [InlineData(true, true, true, true, true, true, true, false)]
        public void CreateFromApiResponseDetail_ThrowsOnNullChildren(
            bool includeSaleProperty,
            bool includeAddress,
            bool includeProperty,
            bool includeSpaces,
            bool includeFeatures,
            bool includeFinancial,
            bool includeShowing,
            bool includeSchools)
        {
            // Arrange
            ListingSaleRequestDetailResponse listingResponse = GetDetailResponse(
                includeSaleProperty,
                includeAddress,
                includeProperty,
                includeSpaces,
                includeFeatures,
                includeFinancial,
                includeShowing,
                includeSchools);

            var sut = new HarListingRequest(listingResponse);

            // Act
            var act = () => sut.CreateFromApiResponseDetail();

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }

        [Fact]
        public void GetAgentBonusRemarksMessage_AgentBonusOn()
        {
            // Arrange
            var sut = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                HasAgentBonus = true,
                BuyerCheckBox = false,
            };

            // Act
            var result = sut.GetAgentBonusRemarksMessage();

            // Assert
            Assert.Contains("Contact Builder for Bonus Information", result);
        }

        [Fact]
        public void GetAgentBonusRemarksMessage_AgentBonusAndBuyerIncentiveOn()
        {
            // Arrange
            var sut = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                HasAgentBonus = true,
                BuyerCheckBox = true,
            };

            // Act
            var result = sut.GetAgentBonusRemarksMessage();

            // Assert
            Assert.Contains("Contact Builder for Bonus & Buyer Incentive Information", result);
        }

        [Fact]
        public void GetAgentBonusRemarksMessage_BuyerIncentiveOn()
        {
            // Arrange
            var sut = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                HasAgentBonus = false,
                BuyerCheckBox = true,
            };

            // Act
            var result = sut.GetAgentBonusRemarksMessage();

            // Assert
            Assert.Contains("Contact Builder for Buyer Incentive Information", result);
        }

        [Fact]
        public void GetAgentBonusRemarksMessage_AgentBonusWithAmountAndBuyerIncentiveOn()
        {
            // Arrange
            var sut = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                HasAgentBonus = false,
                BuyerCheckBox = true,
                HasBonusWithAmount = true,
                AgentBonusAmount = "1",
                AgentBonusAmountType = "%",
            };

            // Act
            var result = sut.GetAgentBonusRemarksMessage();

            // Assert
            Assert.Contains("1% Bonus. Contact Builder for Bonus & Buyer Incentive Information. ", result);
        }

        [Fact]
        public void GetAgentBonusRemarksMessage_AgentBonusWithAmount()
        {
            // Arrange
            var sut = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                HasAgentBonus = false,
                BuyerCheckBox = false,
                HasBonusWithAmount = true,
                AgentBonusAmount = "1344.7811",
                AgentBonusAmountType = "$",
            };

            // Act
            var result = sut.GetAgentBonusRemarksMessage();

            // Assert
            Assert.Contains("$1,344.78 Bonus", result);
        }

        [Fact]
        public void GetSalesAssociateRemarksMessage()
        {
            // Arrange
            var sut = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                AgentListApptPhone = "9999999999",
                OtherPhone = "8888888888",
                CommunityProfileSalesOfficeStreetNum = 11,
                CommunityProfileSalesOfficeStreetName = "Sales Off St",
            };

            // Act
            var result = sut.GetSalesAssociateRemarksMessage();

            // Assert
            Assert.Contains("For more information call (999) 999-9999 or (888) 888-8888. Sales Office at 11 Sales Off St.", result);
        }

        [Fact]
        public void UploadListingItem_SetMlsNumber()
        {
            // Arrange
            var mlsNumber = "454455";
            var harRequest = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                MLSNum = null,
            };

            var sut = new UploadListingItem()
            {
                FullListing = harRequest.CreateFromApiResponse(),
            };

            // act
            sut.SetMlsNumber(mlsNumber);

            // Assert
            Assert.Equal(mlsNumber, sut.MlsNumber);
            Assert.Equal(mlsNumber, sut.FullListing.MLSNum);
        }

        [Fact]
        public void UploadListingItem_SetFullListing()
        {
            // Arrange
            var mlsNumber = "454455";
            var harRequest = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                MLSNum = null,
            };

            var sut = new UploadListingItem()
            {
                FullListing = harRequest.CreateFromApiResponse(),
            };

            var newHarRequest = new HarListingRequest(new ListingSaleRequestQueryResponse())
            {
                MLSNum = mlsNumber,
            };

            // act
            sut.SetFullListing(newHarRequest);

            // Assert
            Assert.True(sut.FullListingConfigured);
            Assert.Equal(mlsNumber, sut.FullListing.MLSNum);
            Assert.False(sut.IsNewListing);
        }

        private static ListingSaleRequestDetailResponse GetDetailResponse(
            bool includeSaleProperty = true,
            bool includeAddress = true,
            bool includeProperty = true,
            bool includeSpaces = true,
            bool includeFeatures = true,
            bool includeFinancial = true,
            bool includeShowing = true,
            bool includeSchools = true,
            bool includeRooms = true,
            bool includeOpenHouse = true)
        {
            var apiDate = new DateTime(2023, 05, 29, 0, 0, 0, DateTimeKind.Utc);
            var listingId = new Guid("7b6eb659-436d-da6a-a9c1-22d8fcbdfaae");
            var requestId = new Guid("7b6eb659-da6a-436d-a9c1-22d8fcbdfaae");
            var userId = new Guid("7b6eb659-a9c1-da6a-436d-22d8fcbdfaae");

            var listingResponse = new ListingSaleRequestDetailResponse
            {
                Id = requestId,
                ListingSaleId = listingId,
                ListPrice = 1234567,
                MlsNumber = "1231231",
                MlsStatus = MarketStatuses.Active,
                SysCreatedOn = apiDate.AddMonths(-1),
                SysCreatedBy = userId,
                SysModifiedOn = apiDate,
                SysModifiedBy = userId,
                SaleProperty = new(),
            };

            if (includeSaleProperty)
            {
                listingResponse.SaleProperty.SalePropertyInfo = new();
            }

            if (includeAddress)
            {
                listingResponse.SaleProperty.AddressInfo = new();
            }

            if (includeProperty)
            {
                listingResponse.SaleProperty.PropertyInfo = new();
            }

            if (includeSpaces)
            {
                listingResponse.SaleProperty.SpacesDimensionsInfo = new();
            }

            if (includeFeatures)
            {
                listingResponse.SaleProperty.FeaturesInfo = new();
            }

            if (includeFinancial)
            {
                listingResponse.SaleProperty.FinancialInfo = new();
            }

            if (includeShowing)
            {
                listingResponse.SaleProperty.ShowingInfo = new();
            }

            if (includeSchools)
            {
                listingResponse.SaleProperty.SchoolsInfo = new();
            }

            if (includeRooms)
            {
                listingResponse.SaleProperty.Rooms = new List<RoomResponse>();
            }

            if (includeOpenHouse)
            {
                listingResponse.SaleProperty.OpenHouses = new List<OpenHouseResponse>();
            }

            return listingResponse;
        }
    }
}
