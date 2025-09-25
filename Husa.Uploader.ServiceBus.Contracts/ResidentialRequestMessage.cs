namespace Husa.Uploader.ServiceBus.Contracts
{
    using Husa.Extensions.Common.Enums;

    public class ResidentialRequestMessage : BaseMessage
    {
        public virtual MarketCode MarketCode { get; set; }
        public virtual Guid RequestId { get; set; }
    }
}
