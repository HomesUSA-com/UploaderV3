using Husa.Uploader.ViewModels;
using System;
using System.Windows;

namespace Husa.Uploader.Views
{
    /// <summary>
    /// Interaction logic for LatLonInputView.xaml
    /// </summary>
    public partial class LatLonInputView : Window
    {
        public LatLonInputView(LatLonInputViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
