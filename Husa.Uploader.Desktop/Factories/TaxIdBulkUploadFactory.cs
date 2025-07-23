namespace Husa.Uploader.Desktop.Factories
{
    using System;
    using System.Collections.Generic;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.BulkUpload.TaxIdBulkUpload;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Microsoft.Extensions.DependencyInjection;

    public class TaxIdBulkUploadFactory : ITaxIdBulkUploadFactory
    {
        private readonly IServiceProvider serviceProvider;
        private ITaxIdBulkUploadListings uploader;

        public TaxIdBulkUploadFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public ITaxIdBulkUploadListings Uploader
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
                MarketCode.Houston => IsAssignableFrom<IHarUploadService, T>(),
                MarketCode.CTX => IsAssignableFrom<ICtxUploadService, T>(),
                MarketCode.Austin => IsAssignableFrom<IAborUploadService, T>(),
                MarketCode.Amarillo => IsAssignableFrom<IAmarilloUploadService, T>(),
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

        public T Create<T>(MarketCode marketCode, List<TaxIdBulkUploadListingItem> bulkListings)
        {
            this.Uploader = marketCode switch
            {
                MarketCode.DFW => this.serviceProvider.GetRequiredService<IDfwTaxIdBulkUploadService>(),
                _ => throw new NotSupportedException($"The market {marketCode} is not supported"),
            };

            this.uploader?.SetBulkListings(bulkListings);

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
