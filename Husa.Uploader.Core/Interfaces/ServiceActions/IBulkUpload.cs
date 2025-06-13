namespace Husa.Uploader.Core.Interfaces.ServiceActions
{
    using System.Threading.Tasks;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Models;

    public interface IBulkUpload
    {
        MarketCode CurrentMarket { get; }
        Task<UploaderResponse> Upload(CancellationToken cancellationToken = default);
        void CancelOperation();
        UploaderResponse Logout();
    }
}
