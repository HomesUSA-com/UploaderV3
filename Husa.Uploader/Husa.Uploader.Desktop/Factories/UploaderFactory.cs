namespace Husa.Uploader.Desktop.Factories
{
    using System;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Microsoft.Extensions.DependencyInjection;

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
            get => this.uploader ?? throw new InvalidOperationException($"The field '{nameof(this.uploader)}' must be initialized first");
            private set => this.uploader = value;
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

        public void CloseDriver()
        {
            if (this.uploader is null)
            {
                return;
            }

            this.Uploader.CancelOperation();
        }

        public T Create<T>(MarketCode marketCode)
        {
            switch (marketCode)
            {
                case MarketCode.SanAntonio:
                    this.Uploader = this.serviceProvider.GetRequiredService<ISaborUploadService>();
                    return (T)this.Uploader;
                case MarketCode.CTX:
                    this.Uploader = this.serviceProvider.GetRequiredService<ICtxUploadService>();
                    return (T)this.Uploader;
                default:
                    throw new NotSupportedException($"The market {marketCode} is not supported");
            }
        }

        private static bool IsAssignableFrom<TService, T>()
            where TService : IUploadListing
        {
            var isAssignable = typeof(T).IsAssignableFrom(typeof(TService));

            return isAssignable;
        }
    }
}
