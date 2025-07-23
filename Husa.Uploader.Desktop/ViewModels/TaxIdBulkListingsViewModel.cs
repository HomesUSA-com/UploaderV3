namespace Husa.Uploader.Desktop.ViewModels.BulkUpload
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Models.BulkUpload;

    public class TaxIdBulkListingsViewModel : ViewModel
    {
        private List<TaxIdBulkUploadListingItem> selectedListings;
        private string totalRowsSeleted;
        private UiState state;
        private ICommand continueCommand;
        private ICommand cancelCommand;

        public TaxIdBulkListingsViewModel(IEnumerable<TaxIdBulkUploadListingItem> listings, MarketCode market)
        {
            this.ListingsBulkFiltered = new List<TaxIdBulkUploadResidentialListingFiltered>();
            this.SelectedListings = new List<TaxIdBulkUploadListingItem>();
            var marketListings = listings.Where(rq => GetMarketCodeFromString(rq.Market) == market);

            foreach (var rl in marketListings)
            {
                var rlf = new TaxIdBulkUploadResidentialListingFiltered();
                rlf.Selected = true;
                rlf.ResidentialListing = rl;
                this.ListingsBulkFiltered.Add(rlf);
                this.SelectedListings.Add(rl);
            }

            this.totalRowsSeleted = this.ListingsBulkFiltered.Count.ToString();
        }

        public List<TaxIdBulkUploadResidentialListingFiltered> ListingsBulkFiltered { get; set; }

        public bool ShowError => this.State == UiState.Error;

        public List<TaxIdBulkUploadListingItem> SelectedListings
        {
            get
            {
                return this.selectedListings;
            }
            set
            {
                if (value == this.selectedListings)
                {
                    return;
                }

                this.selectedListings = value;
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
            "AMARILLO" => MarketCode.Amarillo,
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
            if (currentWindow == null)
            {
                this.State = UiState.Error;
                this.OnPropertyChanged(name: nameof(this.ShowError));
                return;
            }

            currentWindow.DialogResult = true;

            if (!this.ListingsBulkFiltered.Exists(x => x.Selected))
            {
                this.state = UiState.Error;
                this.OnPropertyChanged(name: nameof(this.ShowError));
                return;
            }

            var selectedListingsFromWindow = this.ListingsBulkFiltered.Where(x => x.Selected).Select(x => x.ResidentialListing);

            if (selectedListingsFromWindow.Any())
            {
                this.SelectedListings = selectedListingsFromWindow.ToList();
            }
        }

        public List<TaxIdBulkUploadListingItem> GetBulkUploadResidentialListingFiltered() => this.SelectedListings.Count != 0 ?
            new(this.SelectedListings) :
            new();
    }
}
