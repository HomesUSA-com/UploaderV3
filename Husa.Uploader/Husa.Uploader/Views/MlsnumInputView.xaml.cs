using Husa.Uploader.ViewModels;
using System;
using System.Windows;

namespace Husa.Uploader.Views
{
    public partial class MlsnumInputView : Window
    {
        public MlsnumInputView(MlsnumInputViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
