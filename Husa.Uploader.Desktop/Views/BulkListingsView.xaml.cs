namespace Husa.Uploader.Desktop.Views
{
    using System.Windows;
    using Husa.Uploader.Desktop.ViewModels;

    public partial class BulkListingsView : Window
    {
        public BulkListingsView(BulkListingsViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
