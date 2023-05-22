namespace Husa.Uploader.Factories
{
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Views;

    public interface IChildViewFactory
    {
        MlsIssueReportView Create(UploadListingItem uploadListingItem, bool isFailure);
    }
}
