using Husa.Extensions.Common.Enums;
using Husa.Uploader.Core.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Husa.Uploader.Factories
{
    public class UploaderFactory : IUploadFactory
    {
        private readonly IServiceProvider serviceProvider;
        private IUploadListing uploader;

        public UploaderFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IUploadListing Uploader
        {
            private set => this.uploader = value;
            get
            {
                return this.uploader ?? throw new ArgumentNullException(nameof(uploader));
            }
        }

        public T Create<T>(MarketCode marketCode)
        {
            switch(marketCode)
            {
                case MarketCode.SanAntonio:
                    this.Uploader = this.serviceProvider.GetRequiredService<ISaborUploadService>();
                    return (T)this.Uploader;
                case MarketCode.CTX:
                    this.Uploader = this.serviceProvider.GetRequiredService<ICtxUploadService>();
                    return (T)this.Uploader;
                default:
                    throw new NotSupportedException($"The market {marketCode} is not supported");
            };
        }

        public static bool IsActionSupported<T>(MarketCode marketCode)
        {
            return marketCode switch
            {
                MarketCode.SanAntonio => IsAssignableFrom<ISaborUploadService, T>(),
                MarketCode.CTX => IsAssignableFrom<ICtxUploadService, T>(),
                _ => false,
            };
        }

        private static bool IsAssignableFrom<TService, T>()
            where TService : IUploadListing
        {
            var isAssignable = typeof(T).IsAssignableFrom(typeof(TService));

            return isAssignable;
        }
    }
}
