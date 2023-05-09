namespace Husa.Uploader.Core.Interfaces
{
    public interface ICTXUploader : IUploader,
        IEditor,
        IImageUploader,
        IPriceUploader,
        IStatusUploader,
        ICompletionDateUploader,
        IUploadVirtualTourUploader
    {
    }
}