namespace Husa.Uploader.Core.Interfaces.Services
{
    public interface ISaborUploadService :
        IUploadListing,
        IEditListing,
        IUpdateImages,
        IUpdatePrice,
        IUpdateStatus,
        IUpdateCompletionDate,
        IUpdateOpenHouse,
        IUpdateVirtualTour
    {
    }
}