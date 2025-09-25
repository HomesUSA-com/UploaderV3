namespace Husa.Uploader.Crosscutting.Options
{
    using System.ComponentModel.DataAnnotations;
    using Husa.Extensions.ServiceBus.Attributes;

    public class ServiceBusSettings
    {
        public const string Section = "ServiceBus";

        [Required(AllowEmptyStrings = false, ErrorMessage = "A Service Bus connection string must be provided.")]
        public string ConnectionString { get; set; }

        public ServiceBusOptions UploaderService { get; set; }
    }
}
