namespace Husa.Uploader.Desktop.Views
{
    using System;
    using System.Windows;
    using Husa.Uploader.Desktop.ViewModels;

    /// <summary>
    /// Interaction logic for LatLonInputView.xaml.
    /// </summary>
    public partial class LatLonInputView : Window
    {
        public LatLonInputView(LatLonInputViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
