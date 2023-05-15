using Husa.Extensions.Common.Enums;
using Husa.Uploader.Core.Interfaces.Services;

namespace Husa.Uploader.Factories
{
    public interface IUploadFactory
    {
        public IUploadListing Uploader { get; }

        T Create<T>(MarketCode marketCode);
    }
}