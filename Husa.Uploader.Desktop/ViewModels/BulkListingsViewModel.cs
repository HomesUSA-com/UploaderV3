namespace Husa.Uploader.Desktop.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Models;

    public class BulkListingsViewModel : ViewModel
    {
        private List<UploadListingItem> selectedListingRequests;
        private string totalRowsSeleted;
        private UiState state;
        private ICommand continueCommand;
        private ICommand cancelCommand;

        public BulkListingsViewModel(ObservableCollection<UploadListingItem> requests, MarketCode market)
        {
            this.ListingRequestsBulkFiltered = new List<BulkUploadResidentialListingFiltered>();
            this.SelectedListingRequests = new List<UploadListingItem>();
            var marketRequests = requests.Where(rq => (GetMarketCodeFromString(rq.Market) == market) && (!rq.IsNewListing));

            foreach (var rl in marketRequests)
            {
                var rlf = new BulkUploadResidentialListingFiltered();
                rlf.Selected = true;
                rlf.ResidentialListingRequest = rl;
                this.ListingRequestsBulkFiltered.Add(rlf);
                this.SelectedListingRequests.Add(rl);
            }

            this.totalRowsSeleted = this.ListingRequestsBulkFiltered.Count.ToString();
        }

        public List<BulkUploadResidentialListingFiltered> ListingRequestsBulkFiltered { get; set; }

        public bool ShowError => this.State == UiState.Error;

        public List<UploadListingItem> SelectedListingRequests
        {
            get
            {
                return this.selectedListingRequests;
            }
            set
            {
                if (value == this.selectedListingRequests)
                {
                    return;
                }

                this.selectedListingRequests = value;
                this.OnPropertyChanged();
            }
        }

        public string TotalRowsSeleted
        {
            get => "Total rows: " + this.totalRowsSeleted;
            set
            {
                if (value == this.totalRowsSeleted)
                {
                    return;
                }

                this.totalRowsSeleted = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand ContinueCommand
        {
            get
            {
                this.continueCommand ??= new RelayCommand(param => this.Continue(), param => true);
                return this.continueCommand;
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                this.cancelCommand ??= new RelayCommand(param => this.Cancel(), param => true);
                return this.cancelCommand;
            }
        }

        private UiState State
        {
            get => this.state;
            set
            {
                if (value == this.state)
                {
                    return;
                }

                this.state = value;
                this.OnPropertyChanged(name: nameof(this.ShowError));
            }
        }

        public static MarketCode GetMarketCodeFromString(string market) => market switch
        {
            "SABOR" => MarketCode.SanAntonio,
            "CTX" => MarketCode.CTX,
            "ABOR" => MarketCode.Austin,
            "HAR" => MarketCode.Houston,
            "DFW" => MarketCode.DFW,
            _ => throw new NotSupportedException(nameof(market)),
        };

        public void Cancel()
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

            if (currentWindow != null && this.ListingRequestsBulkFiltered.Exists(x => x.Selected))
            {
                var selectedListings = new List<UploadListingItem>();
                this.ListingRequestsBulkFiltered.ForEach((x) =>
                {
                    if (x.Selected)
                    {
                        selectedListings.Add(x.ResidentialListingRequest);
                    }
                });

                if (selectedListings.Any())
                {
                    this.SelectedListingRequests = selectedListings;
                }

                ////currentWindow.DialogResult = true;
            }
            else
            {
                this.state = UiState.Error;
                this.OnPropertyChanged(name: nameof(this.ShowError));
            }
        }

        public List<UploadListingItem> GetBulkUploadResidentialListingFiltered() => this.SelectedListingRequests.Any() ?
            new(this.SelectedListingRequests) :
            new();
    }
}
