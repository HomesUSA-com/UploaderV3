namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Uploader.Crosscutting.Enums.Ctx;
    using System;

    public record OpenHouseRecord
    {
        public Guid Id { get; set; }

        public Guid SalePropertyId { get; set; }

        public OpenHouseType Type { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool Refreshments { get; set; }

        public bool Lunch { get; set; }

        public bool IsDeleted { get; set; }
    }
}
