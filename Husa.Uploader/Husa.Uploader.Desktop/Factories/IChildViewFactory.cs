namespace Husa.Uploader.Desktop.Factories
{
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Desktop.Views;

    public interface IChildViewFactory
    {
        MlsIssueReportView Create(UploadListingItem uploadListingItem, bool isFailure);
    }
}
