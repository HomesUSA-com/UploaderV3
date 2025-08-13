namespace Husa.Uploader.Desktop.Views
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using Husa.Uploader.Desktop.ViewModels;

    public partial class MlsIssueReportView : Window
    {
        public MlsIssueReportView(MlsIssueReportViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
