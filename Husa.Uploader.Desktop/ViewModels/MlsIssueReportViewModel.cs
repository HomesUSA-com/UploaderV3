namespace Husa.Uploader.Desktop.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Factories;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class MlsIssueReportViewModel : ViewModel
    {
        private readonly ILogger<MlsIssueReportViewModel> logger;
        private readonly IJiraService jiraService;
        private UiState state;
        private string url;
        private string jiraIssueKey;
        private string issueDescription;
        private bool isFailure;
        private List<UploaderError> uploaderErrors;
        private JiraServiceSettings jiraSettings;

        private ICommand cancelCommand;
        private ICommand finishCommand;
        private ICommand closeCommand;
        private ICommand reportCommand;
        private ICommand showLogCommand;

        public MlsIssueReportViewModel(ILogger<MlsIssueReportViewModel> logger, IJiraService jiraService, IOptions<ApplicationOptions> options)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.jiraService = jiraService ?? throw new ArgumentNullException(nameof(jiraService));
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
                this.reportCommand ??= new RelayCommand(async param => await this.ReportAsync(), param => true);
                return this.reportCommand;
            }
        }

        public ICommand ShowLogCommand
        {
            get
            {
                this.showLogCommand ??= new RelayCommand(param => this.Report(), param => true);
                return this.showLogCommand;
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

        public string IssueDescription
        {
            get => this.issueDescription;
            set
            {
                if (this.issueDescription == value)
                {
                    return;
                }

                this.issueDescription = value;
                this.OnPropertyChanged();
            }
        }

        public UiState State
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

        public string JiraIssueKey
        {
            get => this.jiraIssueKey;
            set
            {
                if (this.jiraIssueKey == value)
                {
                    return;
                }

                this.jiraIssueKey = value;
                this.OnPropertyChanged();
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

        public async Task ReportAsync()
        {
            this.jiraService.InitializeBasicAuth(this.jiraSettings.Url, this.jiraSettings.Email, this.jiraSettings.ApiToken);
            string listingDescription = this.SelectedListingRequest.IsNewListing
                ? "New Listing"
                : $"MLS number {this.SelectedListingRequest.MlsNumber}";

            string summary =
                $"[{this.SelectedListingRequest.Market}] Uploader Issue - Error uploading {listingDescription}";
            string issueKey = await this.jiraService.CreateBugAsync(summary, this.issueDescription, this.jiraSettings.Project, this.uploaderErrors);
            this.State = UiState.Issue;
            this.JiraIssueKey = issueKey;
            string urlJiraIssue = $"{this.jiraSettings.Url}/browse/{this.jiraIssueKey}";
            this.Url = urlJiraIssue;
        }

        public void Navigate()
        {
            // Method intentionally left empty.
        }

        public void Configure(UploadListingItem selectedListingRequest, bool isFailure, JiraServiceSettings jiraSettings, List<UploaderError> uploaderErrors)
        {
            this.SelectedListingRequest = selectedListingRequest;
            this.isFailure = isFailure;
            this.jiraSettings = jiraSettings;
            this.uploaderErrors = uploaderErrors;
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
