namespace Husa.Uploader.Data.Tests
{
    using Husa.Uploader.Crosscutting.Converters;
    using Xunit;
    public class UtilitiesTests
    {
        [Fact]
        public void ToFormatSellerConcessions_ReturnsNull_WhenSellerConcessionsIsNull()
        {
            decimal? sellerConcessions = null;
            string agentBonusAmountType = "$";

            var result = sellerConcessions.ToFormatSellerConcessions(agentBonusAmountType);

            Assert.Null(result);
        }

        [Fact]
        public void ToFormatSellerConcessions_ReturnsFormattedString_WithNoDecimals_WhenAgentBonusAmountTypeIsDollar()
        {
            decimal? sellerConcessions = 1500.75m;
            string agentBonusAmountType = "$";

            var result = sellerConcessions.ToFormatSellerConcessions(agentBonusAmountType);

            Assert.Equal("$1501", result);
        }

        [Fact]
        public void ToFormatSellerConcessions_ReturnsFormattedString_WithTwoDecimals_WhenAgentBonusAmountTypeIsPercent()
        {
            decimal? sellerConcessions = 15.5m;
            string agentBonusAmountType = "%";

            var result = sellerConcessions.ToFormatSellerConcessions(agentBonusAmountType);

            Assert.Equal("15.50%", result);
        }

        [Fact]
        public void ToFormatSellerConcessions_ReturnsFormattedString_WithTwoDecimals_WhenAgentBonusAmountTypeIsOtherThanDollar()
        {
            decimal? sellerConcessions = 1234.5678m;
            string agentBonusAmountType = "€";

            var result = sellerConcessions.ToFormatSellerConcessions(agentBonusAmountType);

            Assert.Equal("1234.57€", result);
        }
    }
}
