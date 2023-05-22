namespace Husa.Uploader.Views
{
    using System;
    using System.Windows;
    using Husa.Uploader.ViewModels;

    public partial class MlsIssueReportView : Window
    {
        public MlsIssueReportView(MlsIssueReportViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
