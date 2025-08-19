namespace Husa.Uploader.Data.Interfaces.Common
{
    public interface IShowingInformation
    {
        bool? HasAgentBonus { get; }
        string AgentBonusAmountType { get; }
        string AgentBonusAmount { get; }
        string ProspectsExempt { get; }
        string TitleCo { get; }
        string EarnestMoney { get; }
        string LockboxTypeDesc { get; }
        string AgentListApptPhone { get; }
        string OtherPhone { get; }
        string LockboxLocDesc { get; }
        string Showing { get; }
    }
}
