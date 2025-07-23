namespace Husa.Uploader.Desktop.Views
{
    using System;
    using System.Windows;
    using Husa.Uploader.Desktop.ViewModels;

    public partial class TaxIdBulkUploadView : Window
    {
        public TaxIdBulkUploadView(TaxIdBulkUploadViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
