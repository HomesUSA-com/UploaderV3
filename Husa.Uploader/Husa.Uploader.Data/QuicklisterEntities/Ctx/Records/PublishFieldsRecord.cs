namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using System;
    using Husa.Uploader.Crosscutting.Enums.Ctx;

    public record PublishFieldsRecord
    {
        public ActionType? PublishType { get; set; }

        public Guid? PublishUser { get; set; }

        public MarketStatuses? PublishStatus { get; set; }

        public DateTime? PublishDate { get; set; }
    }
}
