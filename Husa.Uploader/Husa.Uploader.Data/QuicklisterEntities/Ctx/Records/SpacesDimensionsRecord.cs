namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    public record SpacesDimensionsRecord
    {
        public virtual int? NumBedrooms { get; set; }

        public virtual int? BathsFull { get; set; }

        public virtual int? BathsHalf { get; set; }

        public virtual int? DiningAreas { get; set; }

        public virtual int? LivingAreas { get; set; }

        public virtual int? Fireplaces { get; set; }
    }
}
