namespace Husa.Uploader.Core.Interfaces
{
    public interface ISanAntonioUploader: IUploader,
        IEditor, 
        IImageUploader, 
        IPriceUploader, 
        IStatusUploader, 
        ICompletionDateUploader, 
        IUpdateOpenHouseUploader,
        IUploadVirtualTourUploader
    {
    }
}