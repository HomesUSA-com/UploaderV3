namespace Husa.Uploader.Desktop.Factories
{
    using System;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Desktop.ViewModels;
    using Husa.Uploader.Desktop.Views;
    using Microsoft.Extensions.DependencyInjection;

    public class ChildViewFactory : IChildViewFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ChildViewFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public MlsIssueReportView Create(UploadListingItem uploadListingItem, bool isFailure, JiraServiceSettings jiraSettings, List<UploaderError> uploaderErrors)
        {
            var childViewModel = this.serviceProvider.GetRequiredService<MlsIssueReportViewModel>();
            childViewModel.Configure(uploadListingItem, isFailure, jiraSettings, uploaderErrors);

            var childWindow = this.serviceProvider.GetRequiredService<MlsIssueReportView>();
            childWindow.DataContext = childViewModel;

            return childWindow;
        }
    }
}
