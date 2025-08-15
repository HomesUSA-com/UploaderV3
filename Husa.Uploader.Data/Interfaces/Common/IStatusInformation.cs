namespace Husa.Uploader.Data.Interfaces.Common
{
    public interface IStatusInformation
    {
        string ListStatus { get; }
        string ExpectedActiveDate { get; }
        decimal? SoldPrice { get; }
        DateTime? ClosedDate { get; }
        string Financing { get; }
        string SellConcess { get; }
        DateTime? ContractDate { get; }
        string AgentMarketUniqueId { get; }
        string SecondAgentMarketUniqueId { get; }
        DateTime? EstClosedDate { get; }
        DateTime? WithdrawnDate { get; }
        string WithdrawalReason { get; }
        string IsWithdrawalListingAgreement { get; }
        DateTime? OffMarketDate { get; }
    }
}
