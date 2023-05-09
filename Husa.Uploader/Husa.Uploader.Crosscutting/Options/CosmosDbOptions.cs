namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;

    public class CosmosDbOptions
    {
        public const string Section = "CosmosDb";

        [Required(AllowEmptyStrings = false)]
        public string Endpoint { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string AuthToken { get; set; }

        [Required]
        public DatabaseSettings Databases { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string SaleCollectionName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LotCollectionName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LeaseCollectionName { get; set; }
    }
}
