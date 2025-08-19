namespace Husa.Uploader.Data.Interfaces.Common
{
    public interface IListingRemarks
    {
        string PublicRemarks { get; }
        string AgentPrivateRemarks { get; }
        string AgentPrivateRemarksAdditional { get; }
        string Directions { get; }
        bool? IncludeRemarks { get; }
        string RemarksFormatFromCompany { get; }
    }
}
