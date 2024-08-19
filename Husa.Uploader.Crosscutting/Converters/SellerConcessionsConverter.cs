namespace Husa.Uploader.Crosscutting.Converters
{
    public static class SellerConcessionsConverter
    {
        public static string ToFormatSellerConcessions(this decimal? sellerConcessions, string agentBonusAmountType)
        {
            if (sellerConcessions == null)
            {
                return null;
            }

            if (agentBonusAmountType == "$")
            {
                return agentBonusAmountType + sellerConcessions.Value.ToString("F0");
            }
            else
            {
                return sellerConcessions.Value.ToString("F2") + agentBonusAmountType;
            }
        }
    }
}
