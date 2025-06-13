namespace Husa.Uploader.Desktop.Views
{
    using System;
    using System.Windows;
    using Husa.Uploader.Desktop.ViewModels.BulkUpload;

    public partial class TaxIdBulkListingsView : Window
    {
        public TaxIdBulkListingsView(TaxIdBulkListingsViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
