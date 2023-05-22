namespace Husa.Uploader.Core.Interfaces
{
    using Husa.Uploader.Core.Interfaces.ServiceActions;

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
