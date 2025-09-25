namespace Husa.Uploader.ServiceBus.Contracts
{
    using Husa.Extensions.ServiceBus.Interfaces;

    public abstract class BaseMessage : IProvideBusEvent
    {
        protected BaseMessage()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
    }
}
