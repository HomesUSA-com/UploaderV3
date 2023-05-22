namespace Husa.Uploader.Views
{
    using System;
    using System.Windows;
    using Husa.Uploader.ViewModels;

    /// <summary>
    /// Interaction logic for LatLonInputView.xaml
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
