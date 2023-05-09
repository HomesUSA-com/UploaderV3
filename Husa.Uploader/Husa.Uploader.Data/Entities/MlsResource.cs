namespace Husa.Uploader.Data.Entities
{
    public class MlsResource
    {
        public Guid? ResourceID { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }
        public bool? IsPrimaryPic { get; set; }
        public string VirtualTourAddress { get; set; }
        public bool? isRepresentative { get; set; }
        public string ExternalUrl { get; set; }
    }
}