namespace Husa.Uploader.Data.Interfaces
{
    public interface IListingMedia
    {
        Guid Id { get; set; }

        public Uri MediaUri { get; set; }

        string Caption { get; set; }
    }
}
