namespace Husa.Uploader.Desktop.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Core.Services;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Crosscutting.Models;
    using Husa.Uploader.Crosscutting.Options;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Data.Interfaces;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Factories;
    using Husa.Uploader.Desktop.Models;
    using Husa.Uploader.Desktop.Views;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class ShellViewModel : ViewModel
    {
        public static readonly IEnumerable<UploaderState> NoUploadInProgressStatuses = new[]
        {
            UploaderState.UploadInProgress,
            UploaderState.UploadSucceeded,
            UploaderState.UploadSucceededWithErrors,
            UploaderState.UploadFailed,
        };

        private const int MaxSignalRReconnectAttempts = 50;

        private readonly IOptions<ApplicationOptions> options;
        private readonly IListingRequestRepository sqlDataLoader;
        private readonly IAuthenticationService authenticationClient;
        private readonly IVersionManagerService versionManagerService;
        private readonly IChildViewFactory mlsIssueReportFactory;
        private readonly IAbstractFactory<LatLonInputView> locationViewFactory;
        private readonly IAbstractFactory<MlsnumInputView> mlsNumberInputFactory;
        private readonly IUploadFactory uploadFactory;
        private readonly HubConnection hubConnection;
        private readonly ILogger<ShellView> logger;

        private CancellationTokenSource cancellationTokenSource;

        private Entity currentEntity;
        private int signalRConnectionTriesError = 0;
        private DataBaseStatus databaseOnline;
        private SignalRStatus signalROnline;
        private UploaderState state;

        private string userName;
        private string password;
        private string userFullName;
        private string errorMessage;
        private bool isErrorVisible;
        private bool isListingButtonActive;
        private bool isLotButtonActive;
        private int selectedTabItem;
        private string correlationIdBox;
        private string lastUpdated;
        private bool loadFailed;
        private bool updateAvailable;

        private UploadListingItem selectedListingRequest;
        private ObservableCollection<UploadListingItem> listingRequests;

        private ICommand loginCommand;
        private ICommand searchListingCommand;
        private ICommand markCompletedCommand;
        private ICommand cancelProcessCommand;
        private ICommand reportProblemCommand;
        private ICommand reportFailureCommand;

        private ICommand startEditCommand;
        private ICommand startUploadCommand;
        private ICommand startStatusUpdateCommand;
        private ICommand startPriceUpdateCommand;
        private ICommand startCompletionDateUpdateCommand;
        private ICommand startImageUpdateCommand;
        private ICommand startOHUpdateCommand;
        private ICommand startVTUploadCommand;

        public ShellViewModel(
            IOptions<ApplicationOptions> options,
            IListingRequestRepository sqlDataLoader,
            IAuthenticationService authenticationClient,
            IVersionManagerService versionManagerService,
            IChildViewFactory mlsIssueReportFactory,
            IAbstractFactory<LatLonInputView> locationViewFactory,
            IAbstractFactory<MlsnumInputView> mlsNumberInputFactory,
            IUploadFactory uploadFactory,
            HubConnection hubConnection,
            ILogger<ShellView> logger)
            : this()
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.sqlDataLoader = sqlDataLoader ?? throw new ArgumentNullException(nameof(sqlDataLoader));
            this.authenticationClient = authenticationClient ?? throw new ArgumentNullException(nameof(authenticationClient));
            this.versionManagerService = versionManagerService ?? throw new ArgumentNullException(nameof(versionManagerService));
            this.mlsIssueReportFactory = mlsIssueReportFactory ?? throw new ArgumentNullException(nameof(mlsIssueReportFactory));
            this.locationViewFactory = locationViewFactory ?? throw new ArgumentNullException(nameof(locationViewFactory));
            this.mlsNumberInputFactory = mlsNumberInputFactory ?? throw new ArgumentNullException(nameof(mlsNumberInputFactory));
            this.uploadFactory = uploadFactory ?? throw new ArgumentNullException(nameof(uploadFactory));
            this.hubConnection = hubConnection ?? throw new ArgumentNullException(nameof(hubConnection));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this.ConfigureVersionCheck();
        }

        private ShellViewModel()
        {
            this.SelectedTabItem = 0;
            this.DatabaseOnline = DataBaseStatus.Unknow;
            this.SignalROnline = SignalRStatus.Unknow;
            this.HasPermission = false;
            this.CurrentEntity = Entity.Empty;
            this.IsErrorVisible = false;
            this.ErrorMessage = "Username or password incorrect.";
            this.CorrelationIdBox = string.Empty;
            this.LastUpdated = "Last Updated: 12/12/12 22:22:22";
            this.State = UploaderState.Ready;
        }

        public int SelectedTabItem
        {
            get => this.selectedTabItem;
            set
            {
                this.selectedTabItem = value;
                this.OnPropertyChanged();
            }
        }

        public string CorrelationIdBox
        {
            get => this.correlationIdBox;
            set
            {
                if (value == this.correlationIdBox)
                {
                    return;
                }

                this.correlationIdBox = value;
                this.OnPropertyChanged();
            }
        }

        public string ApplicationBuildDate => $"{VersionManagerService.ApplicationBuildDate} - {VersionManagerService.ApplicationBuildVersion}";

        public bool UpdateAvailable
        {
            get => this.updateAvailable;
            set
            {
                if (value == this.updateAvailable)
                {
                    return;
                }

                this.updateAvailable = value;
                this.OnPropertyChanged();
            }
        }

        public string LastUpdated
        {
            get => this.lastUpdated;
            set
            {
                if (value == this.lastUpdated)
                {
                    return;
                }

                this.lastUpdated = value;
                this.OnPropertyChanged();
            }
        }

        public bool HasPermission { get; set; }

        public bool ShowDataBaseOnline { get; set; }

        public bool ShowDataBaseError { get; set; }

        public bool ShowSignalROnline { get; set; }

        public bool ShowSignalRError { get; set; }

        public bool ShowCancelButton { get; set; }

        public bool LoadFailed
        {
            get => this.loadFailed;
            set
            {
                this.loadFailed = value;
                this.OnPropertyChanged();
            }
        }

        public bool UploadSucceeded => this.State == UploaderState.UploadSucceeded || this.State == UploaderState.UploadSucceededWithErrors;

        public bool UploadFailed => this.State == UploaderState.UploadFailed;

        public Dictionary<string, Item> Workers { get; set; }

        public UploaderState State
        {
            get => this.state;
            set
            {
                this.state = value;
                this.Refresh();
            }
        }

        public bool NoUploadInProgress => !NoUploadInProgressStatuses.Contains(this.State) && !this.LoadFailed;

        public bool IsReadyListing => this.CurrentEntity == Entity.Listing && this.State == UploaderState.Ready && this.SelectedListingRequest != null;

        public bool ShowPanelAction => this.IsReadyListing || this.UploadSucceeded || this.UploadFailed || this.ShowCancelButton;

        public string UserName
        {
            get => this.userName;
            set
            {
                if (value == this.userName)
                {
                    return;
                }

                this.userName = value;
                this.OnPropertyChanged();
            }
        }

        public string Password
        {
            get => this.password;
            set
            {
                if (value == this.password)
                {
                    return;
                }

                this.password = value;
                this.OnPropertyChanged();
            }
        }

        public string UserFullName
        {
            get => this.userFullName;
            set
            {
                if (value == this.userFullName)
                {
                    return;
                }

                this.userFullName = value;
                this.OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => this.errorMessage;
            set
            {
                if (value == this.errorMessage)
                {
                    return;
                }

                this.errorMessage = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsErrorVisible
        {
            get => this.isErrorVisible;
            set
            {
                this.isErrorVisible = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsListingButtonActive
        {
            get => this.isListingButtonActive;
            set
            {
                this.isListingButtonActive = value;
                this.isLotButtonActive = !this.isLotButtonActive;
                this.OnPropertyChanged();
            }
        }

        public Entity CurrentEntity
        {
            get => this.currentEntity;
            set
            {
                if (value == this.currentEntity)
                {
                    return;
                }

                this.currentEntity = value;
                this.SelectedListingRequest = null;
                if (this.currentEntity != Entity.Empty)
                {
                    this.Workers = null;
                    this.signalRConnectionTriesError = 0;
                    Task.Run(() => this.LoadData(this.currentEntity));
                }
            }
        }

        public DataBaseStatus DatabaseOnline
        {
            get => this.databaseOnline;
            set
            {
                if (this.databaseOnline == value)
                {
                    return;
                }

                this.databaseOnline = value;
                if (this.databaseOnline != DataBaseStatus.Online)
                {
                    this.ShowDataBaseOnline = false;
                    this.ShowDataBaseError = true;
                }
                else
                {
                    this.ShowDataBaseOnline = true;
                    this.ShowDataBaseError = false;
                }

                this.OnPropertyChanged(name: nameof(this.ShowDataBaseOnline));
                this.OnPropertyChanged(name: nameof(this.ShowDataBaseError));
            }
        }

        public SignalRStatus SignalROnline
        {
            get => this.signalROnline;
            set
            {
                if (this.signalROnline == value)
                {
                    return;
                }

                this.signalROnline = value;
                if (this.signalROnline != SignalRStatus.Online)
                {
                    this.ShowSignalROnline = false;
                    this.ShowSignalRError = true;
                }
                else
                {
                    this.ShowSignalROnline = true;
                    this.ShowSignalRError = false;
                }

                this.OnPropertyChanged(name: nameof(this.ShowSignalROnline));
                this.OnPropertyChanged(name: nameof(this.ShowSignalRError));
            }
        }

        public UploadListingItem SelectedListingRequest
        {
            get => this.selectedListingRequest;
            set
            {
                this.selectedListingRequest = value;
                this.Refresh();
            }
        }

        public ObservableCollection<UploadListingItem> ListingRequests
        {
            get => this.listingRequests;
            set
            {
                if (Equals(value, this.listingRequests))
                {
                    return;
                }

                this.listingRequests = value;
                this.OnPropertyChanged();
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                this.loginCommand ??= new RelayAsyncCommand(param => this.LogIn(), canExecute: param => true);
                return this.loginCommand;
            }
        }

        public ICommand SearchListingCommand
        {
            get
            {
                this.searchListingCommand ??= new RelayAsyncCommand(param => this.SearchCorrelationId(), canExecute: param => true);
                return this.searchListingCommand;
            }
        }

        public ICommand MarkCompletedCommand
        {
            get
            {
                this.markCompletedCommand ??= new RelayAsyncCommand(param => this.MarkCompleted(), param => true);
                return this.markCompletedCommand;
            }
        }

        public ICommand ReportProblemCommand
        {
            get
            {
                this.reportProblemCommand ??= new RelayAsyncCommand(param => this.OpenMlsIssueReportView(isFailure: false), param => true);
                return this.reportProblemCommand;
            }
        }

        public ICommand ReportFailureCommand
        {
            get
            {
                this.reportFailureCommand ??= new RelayAsyncCommand(param => this.OpenMlsIssueReportView(isFailure: true), param => true);
                return this.reportFailureCommand;
            }
        }

        public ICommand StartEditCommand
        {
            get
            {
                this.startEditCommand ??= new RelayAsyncCommand(param => this.StartEdit(), param => this.CanStartEdit);
                return this.startEditCommand;
            }
        }

        public ICommand StartUploadCommand
        {
            get
            {
                this.startUploadCommand ??= new RelayAsyncCommand(param => this.StartUpload(), param => this.CanStartUpload);
                return this.startUploadCommand;
            }
        }

        public ICommand StartStatusUpdateCommand
        {
            get
            {
                this.startStatusUpdateCommand ??= new RelayAsyncCommand(param => this.StartStatusUpdate(), param => this.CanStartStatusUpdate);
                return this.startStatusUpdateCommand;
            }
        }

        public ICommand StartPriceUpdateCommand
        {
            get
            {
                this.startPriceUpdateCommand ??= new RelayAsyncCommand(param => this.StartPriceUpdate(), param => this.CanStartPriceUpdate);
                return this.startPriceUpdateCommand;
            }
        }

        public ICommand StartCompletionDateUpdateCommand
        {
            get
            {
                this.startCompletionDateUpdateCommand ??= new RelayAsyncCommand(param => this.StartCompletionDateUpdate(), param => this.CanStartCompletionDateUpdate);
                return this.startCompletionDateUpdateCommand;
            }
        }

        public ICommand StartImageUpdateCommand
        {
            get
            {
                this.startImageUpdateCommand ??= new RelayAsyncCommand(param => this.StartImageUpdate(), param => this.CanStartImageUpdate);
                return this.startImageUpdateCommand;
            }
        }

        public ICommand StartOHUpdateCommand
        {
            get
            {
                this.startOHUpdateCommand ??= new RelayAsyncCommand(param => this.StartOHUpdate(), param => this.CanStartOHUpdate);
                return this.startOHUpdateCommand;
            }
        }

        public ICommand StartVTUploadCommand
        {
            get
            {
                this.startVTUploadCommand ??= new RelayAsyncCommand(param => this.StartVTUpload(), param => this.CanStartUploadVirtualTour);
                return this.startVTUploadCommand;
            }
        }

        public ICommand CancelProcessCommand
        {
            get
            {
                this.cancelProcessCommand ??= new RelayAsyncCommand(param => this.CancelProcess(), param => true);
                return this.cancelProcessCommand;
            }
        }

        public bool CanStartEdit => this.SelectedListingRequest != null && !this.SelectedListingRequest.FullListing.IsNewListing && UploaderFactory.IsActionSupported<IEditListing>(this.SelectedListingRequest.FullListing.MarketCode);

        public bool CanStartUpload => this.SelectedListingRequest != null && this.SelectedListingRequest.FullListing.IsNewListing && UploaderFactory.IsActionSupported<IUploadListing>(this.SelectedListingRequest.FullListing.MarketCode);

        public bool CanStartImageUpdate => this.SelectedListingRequest != null && UploaderFactory.IsActionSupported<IUpdateImages>(this.SelectedListingRequest.FullListing.MarketCode);

        public bool CanStartStatusUpdate => this.SelectedListingRequest != null && !this.SelectedListingRequest.FullListing.IsNewListing && UploaderFactory.IsActionSupported<IUpdateStatus>(this.SelectedListingRequest.FullListing.MarketCode);

        public bool CanStartPriceUpdate => this.SelectedListingRequest != null && !this.SelectedListingRequest.FullListing.IsNewListing && UploaderFactory.IsActionSupported<IUpdatePrice>(this.SelectedListingRequest.FullListing.MarketCode);

        public bool CanStartCompletionDateUpdate => this.SelectedListingRequest != null && !this.SelectedListingRequest.FullListing.IsNewListing && UploaderFactory.IsActionSupported<IUpdateCompletionDate>(this.SelectedListingRequest.FullListing.MarketCode);

        public bool CanStartUploadVirtualTour => this.SelectedListingRequest != null && UploaderFactory.IsActionSupported<IUpdateImages>(this.SelectedListingRequest.FullListing.MarketCode);

        public bool CanStartOHUpdate
        {
            get
            {
                if (this.SelectedListingRequest == null || !UploaderFactory.IsActionSupported<IUpdateOpenHouse>(this.SelectedListingRequest.FullListing.MarketCode))
                {
                    return false;
                }

                var isPending = this.SelectedListingRequest.FullListing.ListStatus == "Pending" || this.SelectedListingRequest.FullListing.ListStatus == "PND";
                var enableInPending = isPending && this.SelectedListingRequest.FullListing.AllowPendingList;
                var isActive = this.SelectedListingRequest.FullListing.ListStatus == "Active" || this.SelectedListingRequest.FullListing.ListStatus == "ACT";
                var isPCH = this.SelectedListingRequest.FullListing.ListStatus == "PCH";
                var isBOM = this.SelectedListingRequest.FullListing.ListStatus == "BOM";
                var isSanAntonio = this.SelectedListingRequest.FullListing.MarketCode == MarketCode.SanAntonio;
                return !this.SelectedListingRequest.FullListing.IsNewListing
                    && this.SelectedListingRequest.FullListing.EnableOpenHouse
                    && (isActive || enableInPending || (isSanAntonio && (isPCH || isBOM)));
            }
        }

        private UploadResult UploadResult { get; set; }

        private void Refresh() => this.OnPropertyChanged(name: string.Empty);

        private async Task LogIn()
        {
            await this.DoLoginAction();

            if (this.HasPermission)
            {
                this.CurrentEntity = (int)Entity.Listing;
                this.SelectedTabItem = 1;
                this.ConfigureReload();
                this.ConfigureSignalR();
            }
        }

        private async Task DoLoginAction()
        {
            try
            {
                var userResponse = await this.authenticationClient.LoginAsync(this.UserName, this.Password);
                if (userResponse != null)
                {
                    this.HasPermission = true;
                    this.UserFullName = userResponse.FullName;
                }

                if (!this.HasPermission)
                {
                    this.logger.LogWarning("Username {username} or password incorrect. Please, try again.", this.UserName);
                    this.ShowError(friendlyMessage: "Username or password incorrect. Please, try again.");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error connecting to the authentication service {serviceUrl}", this.options.Value.Services.MigrationService);
                this.ShowError(friendlyMessage: "Error connecting to authentication server.");
            }
        }

        private void ShowError(string friendlyMessage)
        {
            this.IsErrorVisible = true;
            this.ErrorMessage = friendlyMessage;
        }

        private void ConfigureSignalR()
        {
            if (!this.options.Value.FeatureFlags.EnableSignalR)
            {
                this.logger.LogInformation("Skipping SignalR refresh dispatcher registration to {refreshInterval} seconds because it's disabled", this.options.Value.SignalRRefreshIntervalSeconds);
                return;
            }

            var dispatcherTimerSignalR = new DispatcherTimer();
            dispatcherTimerSignalR.Tick += this.ReloadSignalR;
            dispatcherTimerSignalR.Interval = TimeSpan.FromSeconds(this.options.Value.SignalRRefreshIntervalSeconds);
            dispatcherTimerSignalR.Start();
        }

        private void ConfigureReload()
        {
            var reloadDispatcher = new DispatcherTimer();
            reloadDispatcher.Tick += this.ReloadTick;
            reloadDispatcher.Interval = TimeSpan.FromSeconds(this.options.Value.DataRefreshIntervalInSeconds);
            reloadDispatcher.Start();
        }

        private void ConfigureVersionCheck()
        {
            if (!this.options.Value.FeatureFlags.IsVersionCheckEnabled)
            {
                this.logger.LogInformation("Skipping version check dispatcher registration to {refreshInterval} seconds because it's disabled", this.options.Value.VersionCheckIntervalInSeconds);
                return;
            }

            var versionCheckDispatcher = new DispatcherTimer();
            versionCheckDispatcher.Tick += this.VersionCheck;
            versionCheckDispatcher.Interval = TimeSpan.FromSeconds(this.options.Value.VersionCheckIntervalInSeconds);
            versionCheckDispatcher.Start();

            this.VersionCheck(sender: null, e: null);
        }

        private void VersionCheck(object sender, EventArgs e)
        {
            if (!this.UpdateAvailable)
            {
                Task.Run(async () => this.UpdateAvailable = await this.versionManagerService.CheckForUpdateAsync());
            }
        }

        private async void ReloadTick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.CorrelationIdBox) && this.DatabaseOnline != DataBaseStatus.Failed)
            {
                await this.LoadData(this.CurrentEntity);
            }
        }

        private async void ReloadSignalR(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.CorrelationIdBox))
            {
                this.logger.LogWarning("No correlation Id has been set for user {userName}, stopping signalR retry", this.UserName);
                return;
            }

            if (this.State == UploaderState.UploadInProgress)
            {
                this.logger.LogWarning("Upload in progress for user {userName}, stopping signalR retry", this.UserName);
                return;
            }

            if (this.signalRConnectionTriesError > MaxSignalRReconnectAttempts)
            {
                this.logger.LogWarning("Too many retries {retryCount} to connect with signalr, stopping retry", this.signalRConnectionTriesError);
                return;
            }

            try
            {
                await this.ReceiveWorkerList();
                this.signalRConnectionTriesError = 0;
            }
            catch
            {
                this.signalRConnectionTriesError++;
            }
        }

        private async Task SearchCorrelationId()
        {
            this.LastUpdated = "Updating Listings...";
            int recordsCount = 0;

            IEnumerable<ResidentialListingRequest> fullListings;
            try
            {
                switch (this.CurrentEntity)
                {
                    case Entity.Listing:
                        var requestId = new Guid(this.CorrelationIdBox);
                        var pendingRequest = await this.sqlDataLoader.GetListingRequest(
                            requestId,
                            marketCode: MarketCode.SanAntonio,
                            this.cancellationTokenSource.Token);
                        if (pendingRequest != null)
                        {
                            fullListings = new List<ResidentialListingRequest> { pendingRequest };
                            recordsCount = 1;
                            this.ProcessListingData(fullListings);
                        }

                        break;
                }

                this.LastUpdated = $"Total {this.CurrentEntity} records: [{recordsCount}]. Last Updated: {DateTime.Now:MM/dd/yyyy h:mm:ss tt}";
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to Connect to Server.");
                this.LoadFailed = true;
                this.LastUpdated = "Failed to Connect to Server";
                this.Refresh();
            }
        }

        private async Task LoadData(Entity entity)
        {
            this.LastUpdated = $"Updating {entity} data...";
            switch (entity)
            {
                case Entity.Listing:
                    try
                    {
                        var fullListings = await this.sqlDataLoader.GetListingData();
                        this.DatabaseOnline = DataBaseStatus.Online;
                        this.ProcessListingData(fullListings);
                        this.LastUpdated = $"Total {entity} records: [{fullListings.Count()}]. Last Updated: {DateTime.Now:MM/dd/yyyy h:mm:ss tt}";
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Failed to Connect to Server.");
                        this.LastUpdated = "Failed to Connect to Database Server.";
                        this.LoadFailed = true;
                        this.DatabaseOnline = DataBaseStatus.Failed;
                        this.Refresh();
                    }

                    break;
            }
        }

        private async Task ReceiveWorkerList()
        {
            if (!this.options.Value.FeatureFlags.EnableSignalR)
            {
                this.logger.LogInformation("Skipping SignalR worker list refresh because it's disabled");
                return;
            }

            if (this.signalRConnectionTriesError > MaxSignalRReconnectAttempts)
            {
                this.SignalROnline = SignalRStatus.Failed;
                this.logger.LogWarning("Skipping the retrieval action of the workers. The maximum of attempts {maxAttempts} to reconnect with SignalR has been reached setting signalR status to {status}", MaxSignalRReconnectAttempts, this.SignalROnline);
                return;
            }

            try
            {
                // receiving data from other users
                this.hubConnection.On<Dictionary<string, Item>>("updateWorkerList", this.HandleWorkerItems);

                // Start signalR service
                if (this.SignalROnline != SignalRStatus.Online)
                {
                    await this.hubConnection.StartAsync();
                    this.SignalROnline = SignalRStatus.Online;
                }

                // send message
                await this.hubConnection.InvokeAsync("GetWorkerItems");
                this.signalRConnectionTriesError = 0;
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Error connecting to SignalR service GetWorkerItems.");
                this.SignalROnline = SignalRStatus.Failed;
            }
        }

        private void HandleWorkerItems(Dictionary<string, Item> response)
        {
            try
            {
                var hasWorkerListChanged = false;
                if (this.Workers == null)
                {
                    this.Workers = response;
                    hasWorkerListChanged = true;
                }
                else
                {
                    var diff = response
                        .Where(entry =>
                            this.Workers.ContainsKey(entry.Key) &&
                            (this.Workers[entry.Key].SelectedItemID != entry.Value.SelectedItemID ||
                             this.Workers[entry.Key].Status != entry.Value.Status))
                        .ToDictionary(entry => entry.Key, entry => entry.Value);
                    if (diff.Any())
                    {
                        this.Workers = response;
                        hasWorkerListChanged = true;
                    }
                }

                if (!hasWorkerListChanged || this.ListingRequests == null)
                {
                    this.logger.LogWarning("Skipping table refresh because the list of workers hasn't changed {changeStatus} or it's not yet available {listingRequests}.", hasWorkerListChanged, this.ListingRequests);
                    return;
                }

                var updateTable = false;
                // Updating table data
                var uploadListItems = this.ListingRequests.ToList();
                foreach (var worker in this.Workers)
                {
                    UploadListingItem contains = null;
                    switch (this.CurrentEntity)
                    {
                        case Entity.Listing:
                        case Entity.Leasing:
                            contains = uploadListItems.Find(uploadItem => uploadItem.RequestId.ToString() == worker.Value.SelectedItemID);
                            break;
                        case Entity.Lot:
                            contains = uploadListItems.Find(uploadItem => uploadItem.InternalLotRequestId.ToString() == worker.Value.SelectedItemID);
                            break;
                    }

                    if (contains != null)
                    {
                        updateTable = true;

                        var itemIndex = uploadListItems.IndexOf(contains);
                        uploadListItems[itemIndex].WorkingBy = worker.Key;
                        uploadListItems[itemIndex].WorkingStatus = worker.Value.Status;
                    }
                }

                if (!updateTable)
                {
                    return;
                }

                this.ListingRequests = new ObservableCollection<UploadListingItem>(uploadListItems);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Error updating data (from other users) after running SignalR service.");
            }
        }

        private void ProcessListingData(IEnumerable<ResidentialListingRequest> fullListings)
        {
            try
            {
                this.LoadFailed = false;
                var uploadItems = fullListings.Select(listingRequest =>
                {
                    var worker = string.Empty;
                    var workingStatus = string.Empty;
                    if (this.Workers != null)
                    {
                        foreach (var workerItem in this.Workers)
                        {
                            if (workerItem.Value.SelectedItemID == listingRequest.ResidentialListingRequestID.ToString())
                            {
                                worker = workerItem.Key;
                                workingStatus = workerItem.Value.Status;
                            }
                        }
                    }

                    var uploadItem = listingRequest.AsUploadItem(
                        builderName: "Ben Caballero",
                        brokerOffice: "HHRE00",
                        isLeasing: string.Empty,
                        isLot: "No",
                        this.CurrentEntity,
                        worker,
                        workingStatus);

                    return uploadItem;
                }).ToList();

                // Verify if the user has any listing requested and if so, keep it in the interface (replacing anything coming from the DB, or adding it)
                if (this.SelectedListingRequest != null)
                {
                    var uploadItem = uploadItems.Find(c => c.RequestId == this.SelectedListingRequest.RequestId);
                    if (uploadItem != null)
                    {
                        var uploadItemIndex = uploadItems.IndexOf(uploadItem);
                        uploadItems.Remove(uploadItem);
                        uploadItems.Insert(uploadItemIndex, this.SelectedListingRequest);
                    }
                    else
                    {
                        if (!this.IsReadyListing)
                        {
                            uploadItems.Add(this.SelectedListingRequest);
                        }
                        else
                        {
                            this.SelectedListingRequest = null;
                        }
                    }
                }

                this.ListingRequests = new ObservableCollection<UploadListingItem>(uploadItems);
            }
            catch
            {
                this.ListingRequests = new ObservableCollection<UploadListingItem>();
            }
        }

        private async Task BroadcastSelectedList(Guid? selectedId = null)
        {
            if (!this.options.Value.FeatureFlags.EnableSignalR)
            {
                this.logger.LogInformation("Skipping SignalR broadcast of selected list because it's disabled");
                return;
            }

            if (this.signalRConnectionTriesError > MaxSignalRReconnectAttempts)
            {
                this.SignalROnline = SignalRStatus.Failed;
                this.logger.LogWarning("Skipping the broadcast action of the selected listing. The maximum of attempts {maxAttempts} to reconnect with SignalR has been reached setting signalR status to {status}", MaxSignalRReconnectAttempts, this.SignalROnline);
                return;
            }

            try
            {
                if (this.SignalROnline != SignalRStatus.Online)
                {
                    // Start signalr service
                    await this.hubConnection.StartAsync();
                    this.SignalROnline = SignalRStatus.Online;
                }

                // send message
                var item = new Item(selectedId, uploaderStatus: this.State);
                await this.hubConnection.InvokeAsync("SendSelectedItem", this.UserFullName, item);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Error connecting to SignalR service SendSelectedItem {selectedItemId}", selectedId);
                this.SignalROnline = SignalRStatus.Failed;
            }
        }

        private async Task RefreshWorkersOnTable(string responseUserName, Item responseItem)
        {
            UploadListingItem selectedListingItem = null;
            List<UploadListingItem> availableListingsToUpload = this.ListingRequests.ToList();
            switch (this.CurrentEntity)
            {
                case Entity.Listing:
                case Entity.Leasing:
                    selectedListingItem = availableListingsToUpload.Find(c => c.RequestId.ToString() == responseItem.SelectedItemID);
                    break;
                case Entity.Lot:
                    selectedListingItem = availableListingsToUpload.Find(c => c.InternalLotRequestId.ToString() == responseItem.SelectedItemID);
                    break;
            }

            if (selectedListingItem == null)
            {
                this.logger.LogWarning("Skipping workers update since there isn't a selected listing item with id {selectedItemId}", responseItem.SelectedItemID);
                return;
            }

            var selectedIndex = availableListingsToUpload.IndexOf(selectedListingItem);
            availableListingsToUpload[selectedIndex].WorkingBy = responseUserName;
            availableListingsToUpload[selectedIndex].WorkingStatus = responseItem.Status;
            try
            {
                this.ListingRequests = await Task.Run(() => new ObservableCollection<UploadListingItem>(availableListingsToUpload));
            }
            catch
            {
                // Intentionally left empty to disregard any failures running this task
            }
        }

        private async Task Start(UploadType opType, Func<ResidentialListingRequest, CancellationToken, Task<UploadResult>> action)
        {
            var listing = this.SelectedListingRequest.FullListing;
            this.ShowCancelButton = true;
            var entityID = Guid.Empty;
            switch (this.CurrentEntity)
            {
                case Entity.Listing:
                case Entity.Leasing:
                    entityID = listing.ResidentialListingRequestID;
                    break;
                case Entity.Lot:
                    entityID = listing.InternalLotRequestGUID;
                    break;
            }

            try
            {
                this.signalRConnectionTriesError = 0;
                this.State = UploaderState.UploadInProgress;

                // 1. Broadcast the current seleted request ID to other users
                await this.BroadcastSelectedList(selectedId: entityID);

                // 2. Refresh the table
                await this.RefreshWorkersOnTable(this.UserFullName, responseItem: new(listing.ResidentialListingRequestID, this.State));

                // 2. Execute de action
                var response = await this.RunAction(action);
                this.HandleUploadExecutionResult(response);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed when processing the user request.");
                this.HandleUploadExecutionResult(response: UploadResult.Failure);
            }
            finally
            {
                switch (this.UploadResult)
                {
                    case UploadResult.Success:
                        this.State = UploaderState.UploadSucceeded;
                        break;
                    case UploadResult.SuccessWithErrors:
                        this.State = UploaderState.UploadSucceededWithErrors;
                        this.logger.LogWarning("[{uploadType}] upload for [{marketName}] listing with [{ResidentialListingRequestId}] succeeded WITH ERRORS", opType, listing.MarketName, listing.ResidentialListingRequestID);
                        break;
                    case UploadResult.Failure:
                        this.logger.LogError("[{uploadType}] upload for [{marketName}] listing with [{ResidentialListingRequestId}] succeeded WITH ERRORS", opType, listing.MarketName, listing.ResidentialListingRequestID);
                        this.State = UploaderState.UploadFailed;
                        break;
                }

                // 1. roadcast message to other users
                await this.BroadcastSelectedList(selectedId: entityID);

                // 2. Refresh the table
                await this.RefreshWorkersOnTable(this.UserFullName, responseItem: new(listing.ResidentialListingRequestID, this.State));

                try
                {
                    Application.Current.MainWindow.Activate();
                }
                catch (Exception exception)
                {
                    this.logger.LogWarning(exception, "Failed to Activate windows when processing listing with {ResidentialListingRequestId}", listing.ResidentialListingRequestID);
                }
            }
        }

        private async Task<UploadResult> RunAction(
            Func<ResidentialListingRequest, CancellationToken, Task<UploadResult>> action)
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            try
            {
                this.logger.LogInformation("Starting the requested upload operation");
                var listing = this.SelectedListingRequest.FullListing;
                var token = this.cancellationTokenSource.Token;
                return await Task.Run(() => action(listing, token));
            }
            catch (OperationCanceledException)
            {
                this.State = UploaderState.Cancelled;
                this.uploadFactory.Uploader.Logout();
                this.uploadFactory.CloseDriver();
                return UploadResult.Failure;
            }
            finally
            {
                this.cancellationTokenSource = null;
            }
        }

        private async Task StartEdit()
        {
            var uploader = this.uploadFactory.Create<IEditListing>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.Edit, action: uploader.Edit);
        }

        private async Task StartUpload()
        {
            if (string.IsNullOrEmpty(this.SelectedListingRequest.FullListing.MLSNum))
            {
                var locationInfo = this.RequestLocationInfo();
                if (locationInfo.IsValidLocation)
                {
                    this.SelectedListingRequest.FullListing.Latitude = locationInfo.Latitude;
                    this.SelectedListingRequest.FullListing.Longitude = locationInfo.Longitude;
                }
                else
                {
                    await this.FinishUpload();
                    return;
                }
            }

            this.ShowCancelButton = true;
            var uploader = this.uploadFactory.Create<IUploadListing>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.InserOrUpdate, action: uploader.Upload);
        }

        private async Task StartStatusUpdate()
        {
            var uploader = this.uploadFactory.Create<IUpdateStatus>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.Status, action: uploader.UpdateStatus);
        }

        private async Task StartPriceUpdate()
        {
            var uploader = this.uploadFactory.Create<IUpdatePrice>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.Price, action: uploader.UpdatePrice);
        }

        private async Task StartCompletionDateUpdate()
        {
            var uploader = this.uploadFactory.Create<IUpdateCompletionDate>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.CompletionDate, action: uploader.UpdateCompletionDate);
        }

        private async Task StartOHUpdate()
        {
            var uploader = this.uploadFactory.Create<IUpdateOpenHouse>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.OpenHouse, action: uploader.UpdateOpenHouse);
        }

        private async Task StartImageUpdate()
        {
            if (!this.MediaUpload())
            {
                return;
            }

            var uploader = this.uploadFactory.Create<IUpdateImages>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.Image, action: uploader.UpdateImages);
        }

        private async Task StartVTUpload()
        {
            if (!this.MediaUpload())
            {
                return;
            }

            var uploader = this.uploadFactory.Create<IUpdateVirtualTour>(this.SelectedListingRequest.FullListing.MarketCode);
            await this.Start(opType: UploadType.VirtualTour, action: uploader.UploadVirtualTour);
        }

        private bool MediaUpload()
        {
            return !(this.SelectedListingRequest.IsNewListing || string.IsNullOrEmpty(this.SelectedListingRequest.FullListing.MLSNum));
        }

        private async Task FinishUpload()
        {
            if (this.State == UploaderState.UploadInProgress)
            {
                try
                {
                    this.uploadFactory.Uploader.Logout();
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, "Failed to logout of listing {ResidentialListingRequestId} with {uploaderType}", this.SelectedListingRequest.FullListing.ResidentialListingRequestID, this.uploadFactory.Uploader.GetType().ToString());
                }

                this.uploadFactory.Uploader.CancelOperation();
            }

            this.ShowCancelButton = false;
            this.State = UploaderState.Ready;

            // 1. Broadcast to the other user the entity request is free
            await this.BroadcastSelectedList(selectedId: this.SelectedListingRequest.RequestId);

            // 2. Refresh the table
            await this.RefreshWorkersOnTable(this.UserFullName, responseItem: new Item(this.SelectedListingRequest.RequestId, this.State));
        }

        private async Task MarkCompleted()
        {
            await this.FinishUpload();
        }

        private async Task CancelProcess()
        {
            this.ShowCancelButton = false;
            try
            {
                this.cancellationTokenSource?.Cancel(throwOnFirstException: true);
                this.uploadFactory.Uploader.CancelOperation();
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed to kill the Chromedriver.exe proccess.");
            }

            this.State = UploaderState.Ready;
            // 1. Broadcast to the other user the entity request is free
            await this.BroadcastSelectedList(selectedId: null);

            // 2. Refresh the table
            await this.RefreshWorkersOnTable(this.UserFullName, responseItem: new(this.SelectedListingRequest.RequestId, uploaderStatus: UploaderState.Cancelled));
        }

        private string RequestMlsNumber()
        {
            var childWindow = this.mlsNumberInputFactory.Create();
            var result = childWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var childViewModel = (MlsnumInputViewModel)childWindow.DataContext;
                return childViewModel.MlsNum;
            }

            return string.Empty;
        }

        private async Task OpenMlsIssueReportView(bool isFailure)
        {
            var childWindow = this.mlsIssueReportFactory.Create(this.SelectedListingRequest, isFailure);
            childWindow.ShowDialog();

            await this.FinishUpload();
        }

        private LocationInfo RequestLocationInfo()
        {
            var childWindow = this.locationViewFactory.Create();
            var result = childWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var childViewModel = (LatLonInputViewModel)childWindow.DataContext;
                return childViewModel.GetLocationInfo();
            }

            return new();
        }

        private void HandleUploadExecutionResult(UploadResult response)
        {
            this.UploadResult = response;
            if (this.UploadResult == UploadResult.Failure)
            {
                this.ShowCancelButton = false;
            }
        }
    }
}
