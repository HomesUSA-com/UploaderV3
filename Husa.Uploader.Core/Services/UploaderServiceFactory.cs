namespace Husa.Uploader.Core.Services
{
    using System;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public class UploaderServiceFactory : IUploaderServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public UploaderServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IMarketUploadService GetService(MarketCode marketCode)
        {
            switch (marketCode)
            {
                case MarketCode.Austin:
                    return this.serviceProvider.GetRequiredService<AborUploadService>();
                case MarketCode.Amarillo:
                    return this.serviceProvider.GetRequiredService<AmarilloUploadService>();
                case MarketCode.SanAntonio:
                    return this.serviceProvider.GetRequiredService<SaborUploadService>();
                case MarketCode.Houston:
                    return this.serviceProvider.GetRequiredService<HarUploadService>();
                case MarketCode.DFW:
                    return this.serviceProvider.GetRequiredService<DfwUploadService>();
                case MarketCode.CTX:
                    return this.serviceProvider.GetRequiredService<CtxUploadService>();
                default:
                    throw new NotImplementedException("No service implemented for this market code.");
            }
        }
    }
}
