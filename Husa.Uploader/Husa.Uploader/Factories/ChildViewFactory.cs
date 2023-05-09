using Husa.Uploader.Data.Entities;
using Husa.Uploader.ViewModels;
using Husa.Uploader.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Husa.Uploader.Factories
{
    public class ChildViewFactory : IChildViewFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ChildViewFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public MlsIssueReportView Create(UploadListingItem uploadListingItem, bool isFailure)
        {
            var childViewModel = this.serviceProvider.GetRequiredService<MlsIssueReportViewModel>();
            childViewModel.Configure(uploadListingItem, isFailure);

            var childWindow = this.serviceProvider.GetRequiredService<MlsIssueReportView>();
            childWindow.DataContext = childViewModel;

            return childWindow;
        }
    }
}
