namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Uploader.Crosscutting.Enums.Ctx;

    public record SchoolRecord
    {
        public SchoolDistrict SchoolDistrict { get; set; }

        public MiddleSchool MiddleSchool { get; set; }

        public ElementarySchool ElementarySchool { get; set; }

        public HighSchool HighSchool { get; set; }
    }
}
