namespace Husa.Uploader.Desktop.Factories
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.ServiceActions;

    public interface IUploadFactory
    {
        public IUploadListing Uploader { get; }

        T Create<T>(MarketCode marketCode);

        IShowingTimeUploadService ShowingTimeUploaderFactory();

        void CloseDriver();
    }
}
