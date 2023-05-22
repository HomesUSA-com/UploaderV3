namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using System;
    using Husa.Uploader.Crosscutting.Enums.Ctx;

    public record RoomRecord
    {
        public Guid Id { get; set; }

        public Guid SalePropertyId { get; set; }

        public int Length { get; set; }

        public int Width { get; set; }

        public RoomLevel Level { get; set; }

        public RoomType RoomType { get; set; }

        public string EntityOwnerType { get; set; }

        public string FieldType { get; set; }

        public DateTime SysCreatedOn { get; set; }

        public DateTime? SysModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? SysModifiedBy { get; set; }

        public Guid? SysCreatedBy { get; set; }

        public DateTime SysTimestamp { get; set; }

        public Guid CompanyId { get; set; }
    }
}
