using Husa.Uploader.ViewModels;
using MahApps.Metro.Controls;
using System;

namespace Husa.Uploader.Views;

public partial class ShellView : MetroWindow
{
    public ShellView(ShellViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
