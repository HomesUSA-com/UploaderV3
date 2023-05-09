namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums.Ctx;
    using System.Linq;

    public record AddressRecord
    {
        public string FormalAddress { get; set; }

        public string ReadableCity { get; set; }

        public string StreetNumber { get; set; }

        public string StreetName { get; set; }

        public StreetDirectionType? StreetDirection { get; set; }

        public StreetType StreetType { get; set; }

        public string UnitNumber { get; set; }

        public Cities City { get; set; }

        public States State { get; set; }

        public string ZipCode { get; set; }

        public Counties County { get; set; }

        public string LotNum { get; set; }

        public string Block { get; set; }

        public string Subdivision { get; set; }
    }
}
