using Husa.Uploader.ViewModels;
using System;
using System.Windows;

namespace Husa.Uploader.Views
{
    public partial class MlsIssueReportView : Window
    {
        public MlsIssueReportView(MlsIssueReportViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
