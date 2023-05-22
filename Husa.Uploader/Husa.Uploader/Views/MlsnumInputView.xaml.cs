namespace Husa.Uploader.Views
{
    using System;
    using System.Windows;
    using Husa.Uploader.ViewModels;

    public partial class MlsnumInputView : Window
    {
        public MlsnumInputView(MlsnumInputViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }
    }
}
