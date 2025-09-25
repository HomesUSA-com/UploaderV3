namespace Husa.Uploader.Core.Interfaces
{
    using Husa.Extensions.Common.Enums;

    public interface IUploaderServiceFactory
    {
        IMarketUploadService GetService(MarketCode marketCode);
    }
}
