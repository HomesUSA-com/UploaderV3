namespace Husa.Uploader.Core.Interfaces
{
    using Husa.Uploader.Core.Interfaces.ServiceActions;

    public interface IAborUploadService :
        IUploadListing,
        IEditListing,
        IUpdateImages,
        IUpdatePrice,
        IUpdateStatus,
        IUpdateCompletionDate,
        IUpdateVirtualTour,
        IUpdateOpenHouse
    {
    }
}
