namespace Husa.Uploader.Factories
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;

    public interface IUploadFactory
    {
        public IUploadListing Uploader { get; }

        T Create<T>(MarketCode marketCode);

        void CloseDriver();
    }
}
