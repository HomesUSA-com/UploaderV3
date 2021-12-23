// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;
using System.Threading.Tasks;
using Husa.Cargador.Support;
//using Husa.Core.ErrorReporter;
using Serilog;
using Husa.Cargador.EventLog;

namespace Husa.Cargador
{
    public class MlsIssueReportViewModel : Caliburn.Micro.Screen
    {
        private UiState _state;
        private string _url;

        private readonly bool _isFailure;

        public MlsIssueReportViewModel(UploadListingItem selectedListingRequest, bool isFailure)
        {
            State = UiState.Fields;
            SelectedListingRequest = selectedListingRequest;
            Url = "";
            _isFailure = isFailure;
        }

        public void Cancel()
        {
            if (_isFailure)
            {
                Log.Error("User decided not to report an Uploader Failure with listing: {ResidentialListingRequestId}", SelectedListingRequest.RequestId);
            }
            this.TryClose(false);
        }

        public void Close()
        {
            this.TryClose(false);
        }

        public void Finish()
        {
            this.TryClose(false);
        }

        public async Task Report()
        {
            try
            {
                //State = UiState.Creating;

                //var client = new ErrorReporterClient(new Uri("http://aer.dev.homesusa.com"));

                //var summary = string.Format(_isFailure ? "Failed Upload for: {0} in {1}" : "Upload Issue for: {0} in {1}", SelectedListingRequest.Address, SelectedListingRequest.Market);

                /*CreatedIssue = await client.ReportIssue(
                    applicationId: LoggingSupport.ApplicationId,
                    summary: summary,
                       description: "|ResidentialListingRequestId: " + SelectedListingRequest.RequestId + "|\n" + IssueDescription,
                    trackingId: SelectedListingRequest.RequestId.ToString(),
                    labels: new[] { "user-reported" }).ConfigureAwait(false);
                State = UiState.Issue;

                Url = CreatedIssue.Key;*/
            }
            catch (Exception ex)
            {
                EventLogWriter.Write(SystemLog.UploaderApp, "UploaderApp", 0, "Unspecified exception when generating a Jira issue from uploader.\n\n" + ex.Message + "\n\n" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                Log.Fatal(ex, "Unspecified exception when generating a Jira issue from uploader");
                State = UiState.Error;
            }
        }

        public void Navigate()
        {
            //System.Diagnostics.Process.Start(CreatedIssue.Url);
        }

        private UiState State
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyOfPropertyChange(() => ShowIssue);
                NotifyOfPropertyChange(() => ShowFields);
                NotifyOfPropertyChange(() => ShowError);
                NotifyOfPropertyChange(() => ShowCreating);
            }
        }

        public bool ShowCreating { get { return _state == UiState.Creating; } }

        public bool ShowIssue { get { return _state == UiState.Issue; } }

        public bool ShowFields { get { return _state == UiState.Fields; } }

        public bool ShowError { get { return _state == UiState.Error; } }

//        private BasicIssueInformation CreatedIssue { get; set; }

        public string Url
        {
            get { return _url; }
            set
            {
                if (value == _url) return;
                _url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        public string IssueDescription { get; set; }

        private UploadListingItem SelectedListingRequest { get; set; }
    }

    internal enum UiState
    {
        Issue,
        Fields,
        Error,
        Creating
    }
}