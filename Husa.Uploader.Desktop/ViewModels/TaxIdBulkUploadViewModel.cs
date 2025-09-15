namespace Husa.Uploader.Desktop.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Models.BulkUpload;

    public class TaxIdBulkUploadViewModel : ViewModel
    {
        private UiState state;

        private MarketCode? market;

        private ICommand searchCommand;
        private ICommand closeCommand;

        public static ObservableCollection<dynamic> Markets =>
        [
            new { Value = MarketCode.Austin, Text = "Austin" },
            new { Value = MarketCode.DFW, Text = "Dfw" },
            new { Value = MarketCode.Houston, Text = "Houston" },
        ];

        public MarketCode? Market
        {
            get => this.market;
            set
            {
                if (value == this.market)
                {
                    return;
                }

                this.market = value;
                this.OnPropertyChanged();
            }
        }

        public bool ShowError => this.State == UiState.Error;

        public ICommand SearchCommand
        {
            get
            {
                this.searchCommand ??= new RelayCommand(param => this.Continue(), param => this.CanContinue);
                return this.searchCommand;
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                this.closeCommand ??= new RelayCommand(param => this.Close(), param => true);
                return this.closeCommand;
            }
        }

        private bool CanContinue => this.Market.HasValue;

        private UiState State
        {
            get => this.state;
            set
            {
                this.state = value;
                this.OnPropertyChanged(name: nameof(this.ShowError));
            }
        }

        public void Close()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.DataContext == this);
            if (currentWindow != null)
            {
                currentWindow.DialogResult = false;
            }
        }

        public void Continue()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.DataContext == this);
            if (currentWindow != null)
            {
                currentWindow.DialogResult = true;
            }
            else
            {
                this.State = UiState.Error;
                this.OnPropertyChanged(name: nameof(this.ShowError));
            }
        }

        public TaxIdBulkUploadInfo GetBulkUploadInfo() => this.market.HasValue ?
            new(this.market.Value) :
            new();
    }
}
