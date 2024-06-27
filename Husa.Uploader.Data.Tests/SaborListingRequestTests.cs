namespace Husa.Uploader.Data.Tests
{
    using Husa.Quicklister.Sabor.Api.Contracts.Response.ListingRequest.SaleRequest;
    using Husa.Uploader.Data.Entities.MarketRequests;
    using Xunit;
    public class SaborListingRequestTests
    {
        [Fact]
        public void GetAgentRemarksMessage_GetAppointmentPhone()
        {
            // Arrange
            var sut = new SaborListingRequest(new ListingSaleRequestQueryResponse())
            {
                AgentListApptPhone = "123-456-7890",
            };

            // Act
            var result = sut.GetAgentRemarksMessage();

            // Assert
            Assert.Contains("(123) 456-7890", result);
        }

        [Fact]
        public void GetAgentRemarksMessage_GetAppointmentPhoneAndAlternatePhone()
        {
            // Arrange
            var sut = new SaborListingRequest(new ListingSaleRequestQueryResponse())
            {
                AgentListApptPhone = "9876543210",
                OtherPhone = "111-222-3333",
            };

            // Act
            var result = sut.GetAgentRemarksMessage();

            // Assert
            Assert.Contains("(987) 654-3210", result);
            Assert.Contains("(111) 222-3333", result);
        }

        [Fact]
        public void GetAgentBonusRemarksMessage_AgentBonusWithAmountOn()
        {
            // Arrange
            var sut = new SaborListingRequest(new ListingSaleRequestQueryResponse())
            {
                HasAgentBonus = false,
                BuyerCheckBox = false,
                HasBonusWithAmount = true,
                AgentBonusAmount = "1",
                AgentBonusAmountType = "%",
            };

            // Act
            var result = sut.GetAgentBonusRemarksMessage();

            // Assert
            Assert.Equal("1% Bonus. Contact Builder for Bonus Information. ", result);
        }

        [Fact]
        public void GetAgentBonusAmount_Amount()
        {
            // Arrange
            var sut = new SaborListingRequest(new ListingSaleRequestQueryResponse())
            {
                AgentBonusAmountType = "$",
                AgentBonusAmount = "1000",
                HasBonusWithAmount = true,
            };

            // Act
            var result = sut.GetAgentBonusAmount();

            // Assert
            Assert.Equal("$1000", result);
        }
    }
}
