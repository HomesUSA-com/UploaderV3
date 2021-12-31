using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Husa.Uploader.Datasources;
using Husa.Uploader.Support;
using Husa.Core.UploaderBase;
using Husa.Uploader.EventLog;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using Microsoft.AspNet.SignalR.Client;
using System.Threading;
using System.Collections;
using System.Text;
using System.Deployment.Application;
using Newtonsoft.Json;
using System.Windows.Navigation;

namespace Husa.Uploader
{
    public interface IShell
    { }

    public class ShellViewModel : PropertyChangedBase, IShell
    {

        #region Attributes

        private Entity _currentEntity;

        private string _userName;
        private string _password;
        private string _userFullName;
        private bool HasPermission;
        private string _errorMessage;
        private bool _isErrorVisible;

        private int selectedTabItem = 0;

        private readonly SqlDataLoader _loader;
        private UploadListingItem _selectedListingRequest;

        private UploaderState _state;
        private DispatcherTimer _dispatcherTimer;
        private DispatcherTimer _dispatcherTimerSignalR;
        private string _lastUpdated;

        private IEnumerable<ResidentialListingRequest> CurrentResidentialListingRequest;
        private IEnumerable<ResidentialListingRequest> CurrentResidentialLeasing;
        private IEnumerable<ResidentialListingRequest> CurrentLots;


        private ObservableCollection<UploadListingItem> _listingRequests;
        private ObservableCollection<UploadListingItem> _lots;
        
        private bool isListingButtonActive;
        private bool isLotButtonActive;

        private int SignalRConnectionTriesError = 0;

        #endregion Attributes

        #region Properties
        
        public Dictionary<string, Item> Workers { get; set; }

        private bool _updateAvailable;
        private string _correlationIdBox;

        public bool LoadFailed { get; set; }

        public bool ShowDataBaseOnline;
        public bool ShowDataBaseError;
        private DataBaseStatus _databaseOnline;
        public DataBaseStatus DatabaseOnline 
        {
            get { return this._databaseOnline; }
            set
            {
                if (value == _databaseOnline) return;
                _databaseOnline = value;
                if(_databaseOnline != DataBaseStatus.Online)
                {
                    ShowDataBaseOnline = false;
                    ShowDataBaseError = true;
                } else
                {
                    ShowDataBaseOnline = true;
                    ShowDataBaseError = false;
                }
                NotifyOfPropertyChange(() => ShowDataBaseOnline);
                NotifyOfPropertyChange(() => ShowDataBaseError);
            }
        }

        public bool ShowSignalROnline;
        public bool ShowSignalRError;
        private SignalRStatus _signalROnline;
        public SignalRStatus SignalROnline
        {
            get { return this._signalROnline; }
            set
            {
                if (value == _signalROnline) return;
                _signalROnline = value;
                if (_signalROnline != SignalRStatus.Online)
                {
                    ShowSignalROnline = false;
                    ShowSignalRError = true;
                }
                else
                {
                    ShowSignalROnline = true;
                    ShowSignalRError = false;
                }
                NotifyOfPropertyChange(() => ShowSignalROnline);
                NotifyOfPropertyChange(() => ShowSignalRError);
            }
        }
        
        public bool ShowCancelButton { get; set; }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value == _userName) return;
                _userName = value;
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
            }
        }

        public string UserFullName
        {
            get { return _userFullName; }
            set
            {
                if (value == _userFullName) return;
                _userFullName = value;
                NotifyOfPropertyChange(() => UserFullName);
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (value == _errorMessage) return;
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool IsErrorVisible
        {
            get { return _isErrorVisible; }
            set
            {
                if (value == _isErrorVisible) return;
                _isErrorVisible = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }

        public bool IsListingButtonActive
        {
            get { return isListingButtonActive; }
            set
            {
                isListingButtonActive = value;
                isLotButtonActive = !isLotButtonActive;
                NotifyOfPropertyChange(() => IsListingButtonActive);
            }
        }

        public bool IsLotButtonActive
        {
            get { return isLotButtonActive; }
            set
            {
                isLotButtonActive = value;
                isListingButtonActive = !isListingButtonActive;
                NotifyOfPropertyChange(() => IsLotButtonActive);
            }
        }

        public Entity CurrentEntity
        {
            get { return _currentEntity; }
            set
            {
                if (value == _currentEntity) return;
                _currentEntity = value;
                SelectedListingRequest = null;
                Entity loadEntity = (Entity)_currentEntity;
                if(loadEntity != Entity.Empty)
                {
                    Workers = null;
                    SignalRConnectionTriesError = 0;
                    Task.Run(() => LoadData(loadEntity));
                }
            }
        }

        public int SelectedTabItem
        {
            get { return selectedTabItem; }
            set
            {
                selectedTabItem = value;
                NotifyOfPropertyChange(() => SelectedTabItem);
            }
        }

        public string CorrelationIdBox
        {
            get { return _correlationIdBox; }
            set
            {
                if (value == _correlationIdBox) return;
                _correlationIdBox = value;
                NotifyOfPropertyChange(() => CorrelationIdBox);
            }
        }


        public string ApplicationBuildDate
        {
            get
            {
                var version = VersionManager.ApplicationBuildDate + " - " + VersionManager.ApplicationBuildVersion;
                return version;
            }
        }

        public string ApplicationBuildDateLogin
        {
            get 
            { 
                var version = VersionManager.ApplicationBuildDate + " - " + VersionManager.ApplicationBuildVersion;
                return version;
            }
        }

        public bool UpdateAvailable
        {
            get { return _updateAvailable; }
            set
            {
                if (value == _updateAvailable) return;
                _updateAvailable = value;
                NotifyOfPropertyChange(() => UpdateAvailable);
            }
        }

        public ObservableCollection<UploadListingItem> ListingRequests
        {
            get { return _listingRequests; }
            set
            {
                if (Equals(value, _listingRequests)) return;
                _listingRequests = value;
                Refresh();
            }
        }

        public string LastUpdated
        {
            get { return _lastUpdated; }
            set
            {
                if (value == _lastUpdated) return;
                _lastUpdated = value;
                NotifyOfPropertyChange(() => LastUpdated);
            }
        }

        public UploadListingItem SelectedListingRequest
        {
            get
            {
                return _selectedListingRequest;
            }
            set
            {
                _selectedListingRequest = value;
                Refresh();
            }
        }

        public UploaderState State
        {
            get { return _state; }
            set
            {
                _state = value;
                Refresh();
            }
        }

        #endregion Properties

        #region Constructor

        public ShellViewModel()
        {
            var config = UploaderConfiguration.GetConfiguration();
            _loader = new SqlDataLoader(config.DatabaseConnectionString);
            InitialVariableState();
            ConfigureVersionCheck();
            State = UploaderState.Ready;
        }

        #endregion Constructor

        #region Methods
        private void InitialVariableState()
        {
            DatabaseOnline = DataBaseStatus.Unknow;
            SignalROnline = SignalRStatus.Unknow;

            IsErrorVisible = false;
            HasPermission = false;

            //UserFullName = "";
            ErrorMessage = "";

            CurrentEntity = Entity.Empty;
        }

        #endregion

        public async Task SearchCorrelationId()
        {
            LastUpdated = "Updating Listings...";
            int recordsCount = 0;

            IEnumerable<ResidentialListingRequest> fullListings;
            try
            {
                switch (CurrentEntity)
                {
                    case Entity.Listing:
                        fullListings = await _loader.GetListing(CorrelationIdBox);
                        CurrentResidentialListingRequest = fullListings;
                        recordsCount = fullListings.Count();
                        await ProcessListingData(fullListings);
                        break;
                    case Entity.Leasing:
                        fullListings = await _loader.GetLeasing(CorrelationIdBox);
                        CurrentResidentialLeasing = fullListings;
                        recordsCount = fullListings.Count();
                        await ProcessLeasingData(fullListings);

                        break;
                    case Entity.Lot:
                        fullListings = await _loader.GetLot(CorrelationIdBox);
                        CurrentLots = fullListings;
                        recordsCount = fullListings.Count();
                        await ProcessLotsData(fullListings);
                        break;
                }
                LastUpdated = "Total " + CurrentEntity.ToString() + " records: [" + recordsCount + "]. Last Updated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt");
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to Connect to Server.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                LoadFailed = true;
                LastUpdated = "Failed to Connect to Server";
                Refresh();
            }
        }

        private async Task LoadData(Entity entity)
        {
            /*if (State == UploaderState.UploadInProgress)
                return;*/

            int recordsCount = 0;
            LastUpdated = "Updating " + entity + " data...";

            switch(entity)
            {
                case Entity.Listing:
                    IEnumerable<ResidentialListingRequest> fullListings;
                    try
                    {
                        fullListings = await _loader.GetListingData();
                        CurrentResidentialListingRequest = fullListings;
                        DatabaseOnline = DataBaseStatus.Online;
                        await ProcessListingData(fullListings);
                        // TODO : Add refresh data with workers

                        recordsCount = fullListings.Count();
                        LastUpdated = "Total " + entity + " records: [" + recordsCount + "]. Last Updated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt");
                    }
                    catch (Exception ex)
                    {
                        LastUpdated = "Failed to Connect to Database Server.";
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to Connect to Server.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        LoadFailed = true;
                        DatabaseOnline = DataBaseStatus.Failed;
                        Refresh();                        
                    }
                    break;
                case Entity.Leasing:
                    IEnumerable<ResidentialListingRequest> fullLeasing;
                    try
                    {
                        fullLeasing = await _loader.GetLeasingData();
                        CurrentResidentialLeasing = fullLeasing;
                        DatabaseOnline = DataBaseStatus.Online;
                        await ProcessLeasingData(fullLeasing);
                        recordsCount = fullLeasing.Count();
                        LastUpdated = "Total " + entity + " records: [" + recordsCount + "]. Last Updated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt");
                    }
                    catch (Exception ex)
                    {
                        LastUpdated = "Failed to Connect to Database Server.";
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to Connect to Server.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        LoadFailed = true;
                        DatabaseOnline = DataBaseStatus.Failed;
                        Refresh();                        
                    }
                    break;
                case Entity.Lot:
                    IEnumerable<ResidentialListingRequest> fullLots;
                    try
                    {
                        fullLots = await _loader.GetLotsData();
                        CurrentLots = fullLots;
                        DatabaseOnline = DataBaseStatus.Online;
                        await ProcessLotsData(fullLots);
                        recordsCount = fullLots.Count();
                        LastUpdated = "Total " + entity + " records: [" + recordsCount + "]. Last Updated: " + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss tt");
                    }
                    catch (Exception ex)
                    {
                        LastUpdated = "Failed to Connect to Database Server.";
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to Connect to Server.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                        LoadFailed = true;
                        DatabaseOnline = DataBaseStatus.Failed;
                        Refresh();
                    }

                    break;
            }
        }

        private async Task ProcessListingData(IEnumerable<ResidentialListingRequest> fullListings)
        {
            LoadFailed = false;

            try
            {
                var uploadItems = fullListings.Select(c =>
                {
                    String worker = String.Empty;
                    String workingStatus = String.Empty;
                    if (Workers != null)
                    {
                        foreach (var y in Workers)
                        {
                            if (y.Value.SelectedItemID == c.ResidentialListingRequestID.ToString())
                            {
                                worker = y.Key;
                                workingStatus = y.Value.Status;
                            }
                        }
                    }
                    var ui = new UploadListingItem
                    {
                        RequestId = c.ResidentialListingRequestID,
                        MlsNumber = string.IsNullOrEmpty(c.MLSNum) ? "New " + CurrentEntity : c.MLSNum,
                        Address = c.StreetNum + " " + c.StreetName,
                        Status = !String.IsNullOrEmpty(c.ListStatusName) ? c.ListStatusName.ToString(): "",
                        Market = "San Antonio",
                        CompanyName = c.CompanyName,
                        BuilderName = "Ben Caballero",
                        BrokerOffice = "HHRE00",
                        FullListing = c,
                        UnitNumber = c.UnitNum,
                        IsLeasing = "",
                        IsLot = "No",
                        InternalLotRequestId = c.InternalLotRequestID,
                        WorkingBy = worker,
                        WorkingStatus = workingStatus
                    };
                    return ui;
                }).ToList();

                // Verify if the user has any listing requested and if so, keep it in the interface (replacing anything coming from the DB, or adding it)
                if (SelectedListingRequest != null)
                {
                    var contains = uploadItems.FirstOrDefault(c => c.RequestId == SelectedListingRequest.RequestId);
                    if (!IsReadyListing)
                    {
                        if (contains != null)
                        {
                            var i = uploadItems.IndexOf(contains);
                            uploadItems.Remove(contains);
                            uploadItems.Insert(i, SelectedListingRequest);
                        }
                        else
                        {
                            uploadItems.Add(SelectedListingRequest);
                        }
                    }
                    else
                    {
                        if (contains != null)
                        {
                            var i = uploadItems.IndexOf(contains);
                            uploadItems.Remove(contains);
                            uploadItems.Insert(i, SelectedListingRequest);
                        }
                        else
                        {
                            SelectedListingRequest = null;
                        }
                    }
                }

                ListingRequests = new ObservableCollection<UploadListingItem>(uploadItems);
            }
            catch 
            {
                ListingRequests = new ObservableCollection<UploadListingItem>();
            }
        }

        private async Task ProcessLeasingData(IEnumerable<ResidentialListingRequest> fullListings)
        {
            LoadFailed = false;

            try
            {
                var uploadItems = fullListings.Select(c =>
                {
                    String worker = String.Empty;
                    String workingStatus = String.Empty;
                    if (Workers != null)
                    {
                        foreach (var y in Workers)
                        {
                            if (y.Value.SelectedItemID == c.ResidentialLeaseRequestID.ToString())
                            {
                                worker = y.Key;
                                workingStatus = y.Value.Status;
                            }
                        }
                    }
                    var ui = new UploadListingItem
                    {
                        RequestId = c.ResidentialLeaseRequestID,
                        MlsNumber = string.IsNullOrEmpty(c.MLSNum) ? "New Lease" : c.MLSNum,
                        Address = c.StreetNum + " " + c.StreetName,
                        Status = !String.IsNullOrEmpty(c.ListStatusName) ? c.ListStatusName.ToString() : "",
                        Market = c.MarketName,
                        CompanyName = c.CompanyName,
                        BuilderName = c.BrokerName,
                        BrokerOffice = c.BrokerOffice,
                        FullListing = c,
                        UnitNumber = c.UnitNum,
                        IsLeasing = "Yes",
                        WorkingBy = worker,
                        WorkingStatus = workingStatus
                    };
                    return ui;
                }).ToList();

                // Verify if the user has any listing requested and if so, keep it in the interface (replacing anything coming from the DB, or adding it)
                if (SelectedListingRequest != null)
                {
                    var contains = uploadItems.FirstOrDefault(c => c.RequestId == SelectedListingRequest.RequestId);
                    if (!IsReadyListing)
                    {
                        if (contains != null)
                        {
                            var i = uploadItems.IndexOf(contains);
                            uploadItems.Remove(contains);
                            uploadItems.Insert(i, SelectedListingRequest);
                        }
                        else
                        {
                            uploadItems.Add(SelectedListingRequest);
                        }
                    }
                    else
                    {
                        if (contains != null)
                        {
                            var i = uploadItems.IndexOf(contains);
                            uploadItems.Remove(contains);
                            uploadItems.Insert(i, SelectedListingRequest);
                        }
                        else
                        {
                            SelectedListingRequest = null;
                        }
                    }
                }
                ListingRequests = new ObservableCollection<UploadListingItem>(uploadItems);
            }
            catch 
            {
                ListingRequests = new ObservableCollection<UploadListingItem>();
            }
        }

        private async Task ProcessLotsData(IEnumerable<ResidentialListingRequest> fullLots)
        {
            LoadFailed = false;

            try
            {
                var uploadLotItems = fullLots.Select(c =>
                {
                    String worker = String.Empty;
                    String workingStatus = String.Empty;
                    if (Workers != null)
                    {
                        foreach (var y in Workers)
                        {
                            if (y.Value.SelectedItemID == c.InternalLotRequestID.ToString())
                            {
                                worker = y.Key;
                                workingStatus = y.Value.Status;
                            }
                        }
                    }
                    var ui = new UploadListingItem
                    {
                        InternalLotRequestId = c.InternalLotRequestID,
                        MlsNumber = string.IsNullOrWhiteSpace(c.MLSNumLot) ? "New Lot" : c.MLSNumLot,
                        Address = c.StreetNum + " " + c.StreetName,
                        Status = c.ListStatusName.ToString(),
                        Market = c.MarketName,
                        CompanyName = c.CompanyName,
                        BuilderName = c.BrokerName,
                        BrokerOffice = c.BrokerOffice,
                        FullListing = c,
                        UnitNumber = c.UnitNum,
                        IsLot = c.InternalLotRequestID > 0 ? "Yes" : "",
                        WorkingBy = worker,
                        WorkingStatus = workingStatus
                    };
                    return ui;
                }).ToList();

                // Verify if the user has any lot and if so, keep it in the interface (replacing anything coming from the DB, or adding it)
                if (SelectedListingRequest != null)
                {
                    var contains = uploadLotItems.FirstOrDefault(c => c.InternalLotRequestId == SelectedListingRequest.InternalLotRequestId);
                    if (!IsReadyListing)
                    {
                        if (contains != null)
                        {
                            var i = uploadLotItems.IndexOf(contains);
                            uploadLotItems.Remove(contains);
                            uploadLotItems.Insert(i, SelectedListingRequest);
                        }
                        else
                        {
                            uploadLotItems.Add(SelectedListingRequest);
                        }
                    }
                    else
                    {
                        if (contains != null)
                        {
                            var i = uploadLotItems.IndexOf(contains);
                            uploadLotItems.Remove(contains);
                            uploadLotItems.Insert(i, SelectedListingRequest);
                        }
                        else
                        {
                            SelectedListingRequest = null;
                        }
                    }
                }
                ListingRequests = new ObservableCollection<UploadListingItem>(uploadLotItems);
            }
            catch
            {
                ListingRequests = new ObservableCollection<UploadListingItem>();
            }
        }

        #region Services Helper

        private void ConfigureSignalR(UploaderConfiguration config)
        {
            _dispatcherTimerSignalR = new DispatcherTimer();
            _dispatcherTimerSignalR.Tick += ReloadSignalR;
            _dispatcherTimerSignalR.Interval = TimeSpan.FromSeconds(config.SingalRRefreshIntervalSeconds);
            _dispatcherTimerSignalR.Start();
        }

        private void ConfigureReload(UploaderConfiguration config)
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += ReloadTick;
            _dispatcherTimer.Interval = TimeSpan.FromSeconds(config.DataRefreshIntervalInSeconds);
            _dispatcherTimer.Start();
        }

        private void ConfigureVersionCheck()
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += VersionCheck;
            _dispatcherTimer.Interval = TimeSpan.FromMinutes(15);
            _dispatcherTimer.Start();

            VersionCheck(null, null);
        }

        private void VersionCheck(object sender, EventArgs e)
        {
            if (!UpdateAvailable)
            {
                Task.Run(() => UpdateAvailable = VersionManager.InstallUpdateSyncWithInfo());
            }
        }

        private async void ReloadTick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CorrelationIdBox) && DatabaseOnline != DataBaseStatus.Failed)
            {
                await LoadData((Entity)CurrentEntity);
            }
        }

        private async void ReloadSignalR(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CorrelationIdBox) || State == UploaderState.UploadInProgress || SignalRConnectionTriesError > 50)
                return;
            try
            {
                await ReceiveWorkerList();
                SignalRConnectionTriesError = 0;
            } catch (Exception ex) {
                SignalRConnectionTriesError++;
            }
        }

        #endregion Services Helper

        #region Properties

        #region Enable and Disable Action Buttons

        #region Listings

        public bool CanStartEdit
        {
            get
            {
                return SelectedListingRequest != null && UploaderEngine.UploaderSupports<IEditor>(SelectedListingRequest.FullListing);
            }
        }

        public bool CanStartUpload
        {
            get
            {
                return SelectedListingRequest != null && UploaderEngine.UploaderSupports<IUploader>(SelectedListingRequest.FullListing);
            }
        }

        public bool CanStartImageUpdate
        {
            get
            {
                return SelectedListingRequest != null && UploaderEngine.UploaderSupports<IImageUploader>(SelectedListingRequest.FullListing);
            }
        }

        public bool CanStartStatusUpdate
        {
            get
            {
                return SelectedListingRequest != null && UploaderEngine.UploaderSupports<IStatusUploader>(SelectedListingRequest.FullListing) && !string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNum);
            }
        }

        public bool CanStartPriceUpdate
        {
            get
            {
                return SelectedListingRequest != null && UploaderEngine.UploaderSupports<IPriceUploader>(SelectedListingRequest.FullListing) && !string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNum);
            }
        }

        public bool CanStartCompletionDateUpdate
        {
            get
            {
                return SelectedListingRequest != null && UploaderEngine.UploaderSupports<ICompletionDateUploader>(SelectedListingRequest.FullListing) && !string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNum);
            }
        }

        public bool CanStartOHUpdate
        {
            get
            {
                bool notNull = SelectedListingRequest != null && UploaderEngine.UploaderSupports<IUpdateOpenHouseUploader>(SelectedListingRequest.FullListing);
                bool hasMLSNumber = notNull && !string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNum);
                bool enableListingOH = notNull && SelectedListingRequest.FullListing.EnableOpenHouse;
                
                bool isPending = notNull && (SelectedListingRequest.FullListing.ListStatus == "PEND" || 
                                                SelectedListingRequest.FullListing.ListStatus == "PND");
                bool showOHPending = notNull && SelectedListingRequest.FullListing.AllowPendingList == "Y";
                bool pendingFeature = isPending && showOHPending;

                return  /*(notNull && 
                        hasMLSNumber && 
                        pendingFeature) || 
                        (notNull &&
                        hasMLSNumber && !pendingFeature && enableListingOH)*/true;
            }
        }

        public bool CanStartUploadVirtualTour
        {
            get
            {
                return SelectedListingRequest != null && UploaderEngine.UploaderSupports<IImageUploader>(SelectedListingRequest.FullListing);
            }
        }

        #endregion Lising

        #region Lease

        public bool CanStartLeaseUpdate
        {
            get
            {
                return ( (SelectedListingRequest != null && SelectedListingRequest.IsLeasing == "Yes") && UploaderEngine.UploaderSupports<ILeaseUploader>(SelectedListingRequest.FullListing));
            }
        }

        public bool CanStartStatusLeaseUpdate
        {
            get
            {
                return (SelectedListingRequest != null && SelectedListingRequest.IsLeasing == "Yes") && UploaderEngine.UploaderSupports<IStatusLeaseUploader>(SelectedListingRequest.FullListing) && !string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNum);
            }
        }

        #endregion

        #region Lot

        public bool CanStartLotUpdate
        {
            get
            {
                return (CurrentEntity == Entity.Lot && 
                        SelectedListingRequest != null &&
                        SelectedListingRequest.IsLot == "Yes") 
                        && UploaderEngine.UploaderSupports<ILotUploader>(SelectedListingRequest.FullListing);
            }
        }

        public bool CanStartStatusLotUpdate
        {
            get
            {
                return (CurrentEntity == Entity.Lot && 
                        SelectedListingRequest != null && 
                        SelectedListingRequest.InternalLotRequestId > 0) 
                        && UploaderEngine.UploaderSupports<IStatusLotUploader>(SelectedListingRequest.FullListing) && !string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNumLot);
            }
        }

        #endregion Lot

        #endregion Enable and Disable Action Buttons

        #region StatusProperties

        public bool NoUploadInProgress
        {
            get
            {
                return State != UploaderState.UploadInProgress &&
                    State != UploaderState.UploadSucceeded &&
                    State != UploaderState.UploadSucceededWithErrors &&
                    State != UploaderState.UploadFailed && !LoadFailed;
            }
        }

        public bool IsReadyListing
        {
            get { return CurrentEntity == Entity.Listing && State == UploaderState.Ready && SelectedListingRequest != null; }
        }

        public bool IsReadyLeasing
        {
            get { return (CurrentEntity == Entity.Listing || CurrentEntity == Entity.Leasing) && State == UploaderState.Ready && SelectedListingRequest != null && SelectedListingRequest.IsLeasing == "Yes"; }
        }

        public bool IsReadyLot
        {
            get { return CurrentEntity == Entity.Lot && State == UploaderState.Ready && SelectedListingRequest != null; }
        }

        public bool ShowPanelAction
        {
            get { return (IsReadyListing || IsReadyLeasing || IsReadyLot || UploadSucceeded || UploadFailed || ShowCancelButton); }
        }

        public bool UploadSucceeded
        {
            get { return State == UploaderState.UploadSucceeded || State == UploaderState.UploadSucceededWithErrors; }
        }

        public bool UploadFailed
        {
            get { return State == UploaderState.UploadFailed; }
        }

        public bool IsNewListing
        {
            get
            {
                return UploadResponse != null && UploadResponse.Driver.UploadInformation.IsNewListing;
            }
        }

        private UploadResponse UploadResponse { get; set; }
        #endregion

        #endregion Properties

        #region Common Actions

        public async Task StartUpload()
        {
            var media = new Lazy<IEnumerable<IListingMedia>>(() =>
            {
                var request = SelectedListingRequest.FullListing;
                return _loader.GetListingMedia(request.ResidentialListingRequestID, request.MarketName);
            });

            if (String.IsNullOrEmpty(SelectedListingRequest.FullListing.MLSNum))
            {
                var wm = new WindowManager();
                var vm = new LatLonInputViewModel();
                var result = wm.ShowDialog(vm);

                if (result.HasValue && result.Value)
                {
                    SelectedListingRequest.FullListing.Latitude = Decimal.Parse(vm.Latitude);
                    SelectedListingRequest.FullListing.Longitude = Decimal.Parse(vm.Longitude);
                }
                else
                {
                    FinishUpload();
                    return;
                }
            }

            ShowCancelButton = true;

            await Start(UploadType.InserOrUpdate, listing => UploaderEngine.Upload(listing, media), SelectedListingRequest.FullListing);
        }

        #endregion Common Actions

        public async Task LogIn()
        {
            await DoLoginAction();

            if (HasPermission)
            {
                CurrentEntity = (int)Entity.Listing;
                SelectedTabItem = 1;
                NotifyOfPropertyChange(() => SelectedTabItem);
                var config = UploaderConfiguration.GetConfiguration();
                ConfigureReload(config);
                ConfigureSignalR(config);
            }
        }

        public async Task ReleaseNotesLogIn()
        {
            System.Diagnostics.Process.Start("chrome", "https://homesusa.atlassian.net/l/c/0fyqC8Jm" + " --new-window");
        }

        public async Task ReleaseNotesMainScreen()
        {
            System.Diagnostics.Process.Start("chrome", "https://homesusa.atlassian.net/l/c/0fyqC8Jm" + " --new-window");
        }

        /// <summary>
        /// This method notify to other users about the current listing selected
        /// </summary>
        /// <param name="selectedID">Current entity request ID or empty</param>
        /// <returns></returns>
        public async Task BroadcastSelectedList(string selectedID)
        {
            if (SignalRConnectionTriesError > 50)
            {
                SignalROnline = SignalRStatus.Failed;
                return;
            }

            /*if (State == UploaderState.UploadInProgress)
                return;*/

            try
            {
                var config = UploaderConfiguration.GetConfiguration();
                string url = config.SignalRURLServer;

                var connection = new HubConnection(url);
                var echo = connection.CreateHubProxy("uploaderHub");

                // Start singarl service
                await connection.Start();
                SignalROnline = SignalRStatus.Online;

                string workingStatus = State.ToString();

                StringBuilder builder = new StringBuilder();
                foreach (char c in workingStatus)
                {
                    if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                    builder.Append(c);
                }
                workingStatus = builder.ToString();

                // send message
                Item item = new Item
                {
                    Status = !String.IsNullOrEmpty(selectedID) ? workingStatus : "",
                    SelectedItemID = selectedID
                };
                await echo.Invoke("SendSelectedItem", UserFullName, item);
            }  
            catch(Exception e)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error connecting to SignalR service SendSelectedItem. " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                SignalROnline = SignalRStatus.Failed;
            }
        }

        private async Task RefreshWorkersOnTable(string responseUserName, Item responseItem)
        {
            bool updateTable = false;
            UploadListingItem contains = null;
            List<UploadListingItem> list = ListingRequests.ToList();
            switch (CurrentEntity)
            {
                case Entity.Listing:
                case Entity.Leasing:
                    contains = list.FirstOrDefault(c => c.RequestId.ToString() == responseItem.SelectedItemID);
                    break;
                case Entity.Lot:
                    contains = list.FirstOrDefault(c => c.InternalLotRequestId.ToString() == responseItem.SelectedItemID);
                    break;
            }

            if (contains != null)
            {
                updateTable = true;
                var i = list.IndexOf(contains);
                list[i].WorkingBy = responseUserName;
                list[i].WorkingStatus = responseItem.Status;
            }

            if (updateTable)
            {
                try
                {
                    ListingRequests = await Task.Run(() => new ObservableCollection<UploadListingItem>(list));
                }
                catch { }
            }
        }

        public async Task ReceiveWorkerList()
        {
            if (SignalRConnectionTriesError > 50)
            {
                SignalROnline = SignalRStatus.Failed;
                return;
            }

            /*if (State == UploaderState.UploadInProgress)
                return;*/

            try
            {
                var config = UploaderConfiguration.GetConfiguration();
                string url = config.SignalRURLServer;

                var connection = new HubConnection(url);
                var echo = connection.CreateHubProxy("uploaderHub");

                // reciving data to other user
                echo.On<Dictionary<string, Item>>("updateWorkerList", async response =>
                {
                    try
                    {
                        bool workerListHasChanged = false;
                        if (Workers == null)
                        {
                            Workers = response;
                            workerListHasChanged = true;
                        }
                        else
                        {
                            var diff = response.Where(entry => Workers.ContainsKey(entry.Key) && 
                                            (Workers[entry.Key].SelectedItemID != entry.Value.SelectedItemID || Workers[entry.Key].Status != entry.Value.Status) )
                                    .ToDictionary(entry => entry.Key, entry => entry.Value);
                            if (diff.Any())
                            {
                                Workers = response;
                                workerListHasChanged = true;
                            }
                        }

                        if (workerListHasChanged)
                        {
                            bool updateTable = false;
                            // Updating table data
                            List<UploadListingItem> list = ListingRequests.ToList();
                            foreach (var y in Workers)
                            {
                                UploadListingItem contains = null;

                                switch (CurrentEntity)
                                {
                                    case Entity.Listing:
                                    case Entity.Leasing:
                                        contains = list.FirstOrDefault(c => c.RequestId.ToString() == y.Value.SelectedItemID);
                                        break;
                                    case Entity.Lot:
                                        contains = list.FirstOrDefault(c => c.InternalLotRequestId.ToString() == y.Value.SelectedItemID);
                                        break;
                                }

                                if (contains != null)
                                {
                                    updateTable = true;

                                    var i = list.IndexOf(contains);
                                    list[i].WorkingBy = y.Key;
                                    list[i].WorkingStatus = y.Value.Status;
                                }
                            }
                            if (updateTable)
                            {
                                try
                                {
                                    ListingRequests = new ObservableCollection<UploadListingItem>(list);
                                }
                                catch
                                { }
                                
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error updating data (from other users) after running SignalR service. " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                    }
                });

                // Start singarl service
                await connection.Start();
                SignalROnline = SignalRStatus.Online;

                // send message
                await echo.Invoke("GetWorkerItems");

                connection.Stop();

                SignalRConnectionTriesError = 0;
            }
            catch (Exception e)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error connecting to SignalR service GetWorkerItems. " + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                SignalROnline = SignalRStatus.Failed;
            }
        }

        private async Task DoLoginAction()
        {
            HasPermission = false;
            using (var stringContent = new StringContent("{ \"username\": \""+ UserName + "\", \"password\": \"" + Password + "\"  }", System.Text.Encoding.UTF8, "application/json"))
            using (var client = new HttpClient())
            {
                try
                {
                    var config = UploaderConfiguration.GetConfiguration();
                    var response = await client.PostAsync(config.AuthenticateServerUrl, stringContent);
                    if(response.StatusCode.Equals(System.Net.HttpStatusCode.OK))
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (!String.IsNullOrEmpty(result))
                        {
                            var jsonVal = JsonConvert.DeserializeObject(result);
                            dynamic value = jsonVal;

                            if (value != null && value.fullName != null)
                            {
                                HasPermission = true;
                                UserFullName = value.fullName.ToString();
                            } 
                        }
                    }
                    if(!HasPermission)
                    {
                        ErrorMessage = "Username or password incorrect. Please, try again.";
                        IsErrorVisible = true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Error to connect to server.";
                    IsErrorVisible = true;
                    EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Error connecting to the Authentication service. " + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                }
            }
        }

        #region Listing Actions

        public async Task StartEdit()
        {
            await Start(UploadType.Edit, UploaderEngine.Edit, SelectedListingRequest.FullListing);
        }

        public async Task StartStatusUpdate()
        {
            await Start(UploadType.Status, UploaderEngine.UpdateStatus, SelectedListingRequest.FullListing);
        }

        public async Task StartPriceUpdate()
        {
            await Start(UploadType.Price, UploaderEngine.UpdatePrice, SelectedListingRequest.FullListing);
        }

        public async Task StartCompletionDateUpdate()
        {
            await Start(UploadType.CompletionDate, UploaderEngine.UpdateCompletionDate, SelectedListingRequest.FullListing);
        }

        public async Task StartImageUpdate()
        {
            var request = SelectedListingRequest.FullListing;
            var media = _loader.GetListingMedia(request.ResidentialListingRequestID, request.MarketName);

            if (string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNum) || string.IsNullOrWhiteSpace(SelectedListingRequest.MlsNumber) || SelectedListingRequest.MlsNumber == "New Listing")
            {
                var wm = new WindowManager();
                var vm = new MlsNumInputViewModel();
                var result = wm.ShowDialog(vm);

                if (result.HasValue && result.Value)
                {
                    request.MLSNum = vm.MlsNum;
                }
                else
                {
                    return;
                }
            }

            await Start(UploadType.Image, listing => UploaderEngine.UpdateImages(listing, media), request);
        }

        public async Task StartOHUpdate()
        {
            await Start(UploadType.OpenHouse, UploaderEngine.UpdateOpenHouse, SelectedListingRequest.FullListing);
        }

        public async Task StartVTUpload()
        {
            var request = SelectedListingRequest.FullListing;
            var media = _loader.GetListingMedia(request.ResidentialListingRequestID, request.MarketName);

            if (string.IsNullOrWhiteSpace(SelectedListingRequest.FullListing.MLSNum) || string.IsNullOrWhiteSpace(SelectedListingRequest.MlsNumber) || SelectedListingRequest.MlsNumber == "New Listing")
            {
                var wm = new WindowManager();
                var vm = new MlsNumInputViewModel();
                var result = wm.ShowDialog(vm);

                if (result.HasValue && result.Value)
                {
                    request.MLSNum = vm.MlsNum;
                }
                else
                {
                    return;
                }
            }

            await Start(UploadType.VirtualTour, listing => UploaderEngine.UpdateVirtualTour(listing, media), request);
        }

        #endregion Listing Actions

        #region Leasing Actions

        public async Task StartLeaseUpdate()
        {
            var media = new Lazy<IEnumerable<IListingMedia>>(() =>
            {
                var request = SelectedListingRequest.FullListing;
                return _loader.GetLeasingMedia(request.ResidentialLeaseRequestID, request.MarketName);
            });

            /*if (SelectedListingRequest.FullListing.ResidentialLeaseRequestID)
            {
                return;
            }*/

            await Start(UploadType.InserOrUpdate, listing => UploaderEngine.UpdateLease(listing, media), SelectedListingRequest.FullListing);
        }

        public async Task StartStatusLeaseUpdate()
        {
            await Start(UploadType.Status, UploaderEngine.UpdateLeaseStatus, SelectedListingRequest.FullListing);
        }

        #endregion

        #region Lots Actions

        public async Task StartLotUpdate()
        {
            var media = new Lazy<IEnumerable<IListingMedia>>(() =>
            {
                var request = SelectedListingRequest.FullListing;
                return _loader.GetListingMedia(request.ResidentialListingRequestID, request.MarketName);
            });

            if (SelectedListingRequest.InternalLotRequestId <= 0)
            {
                return;
            }

            if (String.IsNullOrEmpty(SelectedListingRequest.FullListing.MLSNum))
            {
                var wm = new WindowManager();
                var vm = new LatLonInputViewModel();
                var result = wm.ShowDialog(vm);

                if (result.HasValue && result.Value)
                {
                    SelectedListingRequest.FullListing.Latitude = Decimal.Parse(vm.Latitude);
                    SelectedListingRequest.FullListing.Longitude = Decimal.Parse(vm.Longitude);
                }
                else
                {
                    FinishUpload();
                    return;
                }
            }

            ShowCancelButton = true;

            await Start(UploadType.InserOrUpdate, listing => UploaderEngine.UpdateLot(listing, media), SelectedListingRequest.FullListing);
        }

        public async Task StartStatusLotUpdate()
        {
            if (SelectedListingRequest.InternalLotRequestId <= 0)
            {
                return;
            }

            await Start(UploadType.Status, UploaderEngine.UpdateLotStatus, SelectedListingRequest.FullListing);
        }

        #endregion

        private async Task Start(UploadType opType, Func<ResidentialListingRequest, UploadResponse> action, ResidentialListingRequest listing)
        {
            ShowCancelButton = true;

            string entityID = String.Empty;
            switch (CurrentEntity)
            {
                case Entity.Listing:
                case Entity.Leasing:
                    entityID = listing.ResidentialListingRequestID.ToString();
                    break;
                case Entity.Lot:
                    entityID = listing.InternalLotRequestID.ToString();
                    break;
            }

            try
            {
                UploaderEngine.CloseDrivers();
                SignalRConnectionTriesError = 0;
                State = UploaderState.UploadInProgress;

                // 1. Broadcast the current seleted request ID to other users
                await BroadcastSelectedList(entityID);

                // 2. Refresh the table
                Item item = new Item
                {
                    SelectedItemID = listing.ResidentialListingRequestID.ToString(),
                    Status = State.ToString()
                };
                await RefreshWorkersOnTable(UserFullName, item);

                //2. Execute de action
                UploadResponse = await Task.Run(() => action(listing));
            }
            catch (Exception ex)
            {
                UploadResponse = new UploadResponse
                {
                    Driver = null,
                    Result = UploadResult.Failure,
                    Exception = ex,
                    Uploader = null,
                    Listing = listing
                };
            }
            finally
            {
                switch (UploadResponse.Result)
                {
                    case UploadResult.Success:
                        State = UploaderState.UploadSucceeded;
                        break;
                    case UploadResult.SuccessWithErrors:
                        UploadResponse.Driver.Logger.Error("{UploadType} upload for {Market} listing with {ResidentialListingRequestId} succeeded WITH ERRORS", opType, listing.MarketName, listing.ResidentialListingRequestID);
                        State = UploaderState.UploadSucceededWithErrors;
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "[" + opType + "] upload for [" + listing.MarketName + "] listing with [" + listing.ResidentialListingRequestID + "] succeeded WITH ERRORS", System.Diagnostics.EventLogEntryType.Error);
                        break;
                    case UploadResult.Failure:
                        UploadResponse.Driver.Logger.Error(UploadResponse.Exception, "{UploadType} upload for {Market} of listing {ResidentialListingRequestId} failed", opType, listing.MarketName, listing.ResidentialListingRequestID);
                        EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "[" + opType + "] upload for [" + listing.MarketName + "] listing with [" + listing.ResidentialListingRequestID + "] succeeded WITH ERRORS", System.Diagnostics.EventLogEntryType.Error);
                        State = UploaderState.UploadFailed;
                        break;
                    default:
                        UploaderEngine.CloseDrivers();
                        throw new ArgumentOutOfRangeException();
                }

                // 1. roadcast message to other users
                await BroadcastSelectedList(entityID);

                // 2. Refresh the table
                Item item = new Item
                {
                    SelectedItemID = listing.ResidentialListingRequestID.ToString(),
                    Status = State.ToString()
                };
                await RefreshWorkersOnTable(UserFullName, item);

                try
                {
                    Application.Current.MainWindow.Activate();
                }
                catch (Exception ex)
                {
                    EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to Activate windows when processing listing with " + listing.ResidentialListingRequestID + ".\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                    UploadResponse.Driver.Logger.Warning(ex, "Failed to Activate windows when processing listing with {ResidentialListingRequestId}", listing.ResidentialListingRequestID);
                }
            }
        }

        public void MarkCompleted()
        {
            FinishUpload();
        }

        public void ReportFailure()
        {
            var wm = new WindowManager();
            var vm = new MlsIssueReportViewModel(SelectedListingRequest, true);
            wm.ShowDialog(vm);

            FinishUpload();
        }

        public void CancelProcess()
        {
            ShowCancelButton = false;
            try 
            { 
                UploaderEngine.CloseDrivers();
            } 
            catch  (Exception e)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to kill the Chromedriver.exe proccess." + e.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            }

            State = UploaderState.Ready;
            // 1. Broadcast to the other user the entity request is free
            BroadcastSelectedList("");

            // 2. Refresh the table
            Item item = new Item
            {
                SelectedItemID = SelectedListingRequest.RequestId.ToString(),
                Status = "Cancelled"
            };
            RefreshWorkersOnTable(UserFullName, item);
        }

        public void ReportProblem()
        {
            var wm = new WindowManager();
            var vm = new MlsIssueReportViewModel(SelectedListingRequest, false);
            wm.ShowDialog(vm);

            FinishUpload();
        }

        private void FinishUpload()
        {
            if (UploadResponse != null && UploadResponse.Driver != null && UploadResponse.Uploader != null)
            {
                try
                {
                    UploadResponse.Uploader.Logout(UploadResponse.Driver);
                }
                catch (Exception ex)
                {
                    EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Failed to logout of listing " + UploadResponse.Listing.ResidentialListingRequestID + " with  " + UploadResponse.Uploader.GetType().ToString() + ".\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                    UploadResponse.Driver.Logger.Warning(ex, "Failed to logout of listing {ResidentialListingRequestId} with {Uploader}", UploadResponse.Listing.ResidentialListingRequestID, UploadResponse.Uploader.GetType().ToString());
                }
                UploaderEngine.CloseDriver(UploadResponse.Driver);
            }
            ShowCancelButton = false;
            State = UploaderState.Ready;

            // 1. Broadcast to the other user the entity request is free
            BroadcastSelectedList(SelectedListingRequest.RequestId.ToString());

            // 2. Refresh the table
            Item item = new Item
            {
                SelectedItemID = SelectedListingRequest.RequestId.ToString(),
                Status = State.ToString()
            };
            RefreshWorkersOnTable(UserFullName, item);
        }
    }

    internal enum UploadType
    {
        Image,
        Price,
        Status,
        InserOrUpdate,
        CompletionDate,
        Edit,
        OpenHouse,
        VirtualTour
    }

    public enum Entity
    {
        Listing,
        Leasing,
        Lot,
        Empty
    }

    public enum DataBaseStatus
    {
        Failed = 0,
        Online = 1,
        Unknow = 2
    }

    public enum SignalRStatus
    {
        Failed = 0,
        Online = 1,
        Unknow = 2
    }
}