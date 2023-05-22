namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Uploader.Crosscutting.Enums.Ctx;

    public record SalesOfficeRecord
    {
        public string StreetNumber { get; set; }

        public string StreetName { get; set; }

        public string StreetSuffix { get; set; }

        public Cities? SalesOfficeCity { get; set; }

        public string SalesOfficeZip { get; set; }
    }
}
