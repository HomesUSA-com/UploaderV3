namespace Husa.Uploader.Core.Interfaces.Services
{
    public interface ICtxUploadService :
        IUploadListing,
        IEditListing,
        IUpdateImages,
        IUpdatePrice,
        IUpdateStatus,
        IUpdateCompletionDate,
        IUpdateVirtualTour
    {
    }
}