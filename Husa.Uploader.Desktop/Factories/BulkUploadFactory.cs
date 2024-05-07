namespace Husa.Uploader.Desktop.Factories
{
    using System;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Data.Entities;
    using Microsoft.Extensions.DependencyInjection;

    public class BulkUploadFactory : IBulkUploadFactory
    {
        private readonly IServiceProvider serviceProvider;
        private IBulkUploadListings uploader;

        public BulkUploadFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IBulkUploadListings Uploader
        {
            get => this.uploader ?? throw new InvalidOperationException($"The field '{nameof(this.uploader)}' must be initialized first");
            private set => this.uploader = value;
        }

        public static bool IsActionSupported<T>(MarketCode marketCode)
        {
            return marketCode switch
            {
                MarketCode.SanAntonio => IsAssignableFrom<ISaborUploadService, T>(),
                MarketCode.DFW => IsAssignableFrom<IDfwUploadService, T>(),
                MarketCode.CTX => IsAssignableFrom<ICtxUploadService, T>(),
                MarketCode.Austin => IsAssignableFrom<IAborUploadService, T>(),
                _ => false,
            };
        }

        public void CloseDriver()
        {
            if (this.uploader is null)
            {
                return;
            }

            this.Uploader.CancelOperation();
        }

        public T Create<T>(MarketCode marketCode, RequestFieldChange requestFieldChange, List<UploadListingItem> bulkListings)
        {
            switch (marketCode)
            {
                case MarketCode.SanAntonio:
                    this.Uploader = this.serviceProvider.GetRequiredService<ISaborBulkUploadService>();
                    break;
                case MarketCode.DFW:
                    this.Uploader = this.serviceProvider.GetRequiredService<IDfwBulkUploadService>();
                    break;
                case MarketCode.CTX:
                    this.Uploader = this.serviceProvider.GetRequiredService<ICtxBulkUploadService>();
                    break;
                case MarketCode.Austin:
                    this.Uploader = this.serviceProvider.GetRequiredService<IAborBulkUploadService>();
                    break;
                default:
                    throw new NotSupportedException($"The market {marketCode} is not supported");
            }

            if (this.uploader is not null)
            {
                this.uploader.SetRequestFieldChange(requestFieldChange);
                this.uploader.SetBulkListings(bulkListings);
            }

            return (T)this.Uploader;
        }

        private static bool IsAssignableFrom<TService, T>()
            where TService : IUploadListing
        {
            var isAssignable = typeof(T).IsAssignableFrom(typeof(TService));

            return isAssignable;
        }
    }
}
