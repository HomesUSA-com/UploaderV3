namespace Husa.Uploader.Desktop.Factories
{
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Core.Interfaces.ShowingTime;

    public interface IUploadFactory
    {
        public IUploadListing Uploader { get; }

        T Create<T>(MarketCode marketCode);

        IShowingTimeUploadService ShowingTimeUploaderFactory(string marketCode);

        void CloseDriver();
    }
}
