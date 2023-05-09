using Husa.Uploader.Data.Entities;
using Husa.Uploader.Views;

namespace Husa.Uploader.Factories
{
    public interface IChildViewFactory
    {
        MlsIssueReportView Create(UploadListingItem uploadListingItem, bool isFailure);
    }
}