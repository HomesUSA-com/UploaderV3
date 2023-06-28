namespace Husa.Uploader.Data.Tests
{
    using Husa.Quicklister.Sabor.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Xunit;
    public class SaborListingRequestTests
    {
        [Fact]
        public void GetPrivateRemarks_GetAppointmentPhone()
        {
            // Arrange
            var sut = new SaborListingRequest(new ListingSaleRequestQueryResponse())
            {
                AgentListApptPhoneFromCommunityProfile = "123-456-7890",
                OwnerPhone = "987-654-3210",
            };

            // Act
            var result = sut.GetPrivateRemarks(true);

            // Assert
            Assert.Contains("(123) 456-7890", result);
        }

        [Fact]
        public void GetPrivateRemarks_GetAppointmentPhoneAndAlternatePhone()
        {
            // Arrange
            var sut = new SaborListingRequest(new ListingSaleRequestQueryResponse())
            {
                AgentListApptPhone = "9876543210",
                AlternatePhoneFromCompany = "111-222-3333",
                OtherPhoneFromCommunityProfile = "444-555-6666",
            };

            // Act
            var result = sut.GetPrivateRemarks(true);

            // Assert
            Assert.Contains("(987) 654-3210", result);
            Assert.Contains("(111) 222-3333", result);
        }
    }
}
