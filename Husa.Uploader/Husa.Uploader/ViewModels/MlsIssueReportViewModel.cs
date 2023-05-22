namespace Husa.Uploader.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Husa.Uploader.Commands;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Microsoft.Extensions.Logging;

    public class MlsIssueReportViewModel : ViewModel
    {
        private readonly ILogger<MlsIssueReportViewModel> logger;

        private UiState state;
        private string url;
        private bool isFailure;

        private ICommand cancelCommand;
        private ICommand finishCommand;
        private ICommand closeCommand;
        private ICommand reportCommand;

        public MlsIssueReportViewModel(ILogger<MlsIssueReportViewModel> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.State = UiState.Fields;
            this.Url = string.Empty;
        }

        public ICommand CancelCommand
        {
            get
            {
                this.cancelCommand ??= new RelayCommand(param => this.Cancel(), param => true);
                return this.cancelCommand;
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                this.closeCommand ??= new RelayCommand(param => this.TryClose(), param => true);
                return this.closeCommand;
            }
        }

        public ICommand FinishCommand
        {
            get
            {
                this.finishCommand ??= new RelayCommand(param => this.TryClose(), param => true);
                return this.finishCommand;
            }
        }

        public ICommand ReportCommand
        {
            get
            {
                this.reportCommand ??= new RelayCommand(param => this.Report(), param => true);
                return this.reportCommand;
            }
        }

        public bool ShowCreating => this.state == UiState.Creating;

        public bool ShowIssue => this.state == UiState.Issue;

        public bool ShowFields => this.state == UiState.Fields;

        public bool ShowError => this.state == UiState.Error;

        public string Url
        {
            get => this.url;
            set
            {
                if (this.url == value)
                {
                    return;
                }

                this.url = value;
                this.OnPropertyChanged();
            }
        }

        public string IssueDescription { get; set; }

        private UiState State
        {
            get => this.state;
            set
            {
                this.state = value;
                this.OnPropertyChanged(name: nameof(this.ShowIssue));
                this.OnPropertyChanged(name: nameof(this.ShowFields));
                this.OnPropertyChanged(name: nameof(this.ShowError));
                this.OnPropertyChanged(name: nameof(this.ShowCreating));
            }
        }

        private UploadListingItem SelectedListingRequest { get; set; }

        public void Cancel()
        {
            if (this.isFailure)
            {
                this.logger.LogError("User decided not to report an Uploader Failure with listing: {ResidentialListingRequestId}", this.SelectedListingRequest.RequestId);
            }

            this.TryClose();
        }

        public void Report()
        {
            // Method intentionally left empty.
        }

        public void Navigate()
        {
            // Method intentionally left empty.
        }

        public void Configure(UploadListingItem selectedListingRequest, bool isFailure)
        {
            this.SelectedListingRequest = selectedListingRequest;
            this.isFailure = isFailure;
        }

        private void TryClose()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(window => window.DataContext == this);
            if (currentWindow != null)
            {
                currentWindow.DialogResult = false;
            }
        }
    }
}
