namespace Husa.Uploader.Views;
using System;
using Husa.Uploader.ViewModels;
using MahApps.Metro.Controls;

public partial class ShellView : MetroWindow
{
    public ShellView(ShellViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
