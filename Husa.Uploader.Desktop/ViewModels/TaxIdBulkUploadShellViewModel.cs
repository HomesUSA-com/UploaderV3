namespace Husa.Uploader.Desktop.ViewModels
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using Husa.Extensions.Common.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities.BulkUpload;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Factories;
    using Husa.Uploader.Desktop.Models.BulkUpload;
    using Husa.Uploader.Desktop.ViewModels.BulkUpload;
    using Husa.Uploader.Desktop.Views;
    using Microsoft.Extensions.Logging;

    public partial class ShellViewModel
    {
        private readonly IAbstractFactory<TaxIdBulkUploadView> taxIdBulkUploadViewFactory;
        private readonly ITaxIdBulkUploadFactory taxIdBulkUploadFactory;
        private ICommand startTaxIdBulkUploadCommand;

        public bool CanStartTaxIdBulk => this.CurrentEntity == Entity.Listing && this.ListingRequests?.Any() != null;

        public ICommand StartTaxIdBulkUploadCommand
        {
            get
            {
                this.startTaxIdBulkUploadCommand ??= new RelayAsyncCommand(param => this.StartTaxIdBulkUpload(), param => this.CanStartTaxIdBulk);
                return this.startTaxIdBulkUploadCommand;
            }
        }

        private async Task StartTaxIdBulk(Func<CancellationToken, Task<UploaderResponse>> action)
        {
            UploaderResponse response = new UploaderResponse();

            this.ShowCancelButton = true;
            try
            {
                this.signalRConnectionTriesError = 0;
                this.State = UploaderState.UploadInProgress;
                response = await this.RunTaxIdBulkAction(action);
                this.HandleUploadExecutionResult(response);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed when processing the user request.");
                this.HandleUploadExecutionResult(response);
            }
            finally
            {
                try
                {
                    Application.Current.MainWindow.Activate();
                }
                catch (Exception exception)
                {
                    this.logger.LogWarning(exception, "Failed to Activate windows when processing Bulk Upload");
                }
            }
        }

        private async Task<UploaderResponse> RunTaxIdBulkAction(Func<CancellationToken, Task<UploaderResponse>> action)
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            try
            {
                this.logger.LogInformation("Starting the requested upload operation");
                var token = this.cancellationTokenSource.Token;
                return await Task.Run(() => action(token));
            }
            catch (OperationCanceledException)
            {
                return this.CatchCanceledException();
            }
            finally
            {
                this.cancellationTokenSource.Dispose();
                this.cancellationTokenSource = null;

                this.ShowCancelButton = false;
                this.State = UploaderState.Ready;
            }
        }

        private async Task StartTaxIdBulkUpload()
        {
            var bulkUploadInfo = this.RequestTaxIdBulkUploadInfo();
            if (!bulkUploadInfo.IsValid)
            {
                this.FinishTaxIdBulkUpload();
                return;
            }

            var filteredBulkListings = await this.FilterTaxIdBulkUpdater(bulkUploadInfo.Market.Value);
            if (filteredBulkListings.Count == 0)
            {
                this.FinishTaxIdBulkUpload();
                return;
            }

            this.ShowCancelButton = true;
            var uploader = this.taxIdBulkUploadFactory.Create<ITaxIdBulkUploadListings>(bulkUploadInfo.Market.Value, filteredBulkListings);
            await this.StartTaxIdBulk(action: uploader.Upload);
        }

        private void FinishTaxIdBulkUpload()
        {
            if (this.State == UploaderState.UploadInProgress)
            {
                try
                {
                    this.taxIdBulkUploadFactory.Uploader.Logout();
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, "Failed to logout of listing {ResidentialListingRequestId} with {uploaderType}", this.SelectedListingRequest.FullListing.ResidentialListingRequestID, this.uploadFactory.Uploader.GetType().ToString());
                }

                this.taxIdBulkUploadFactory.Uploader.CancelOperation();
            }

            this.ShowCancelButton = false;
            this.State = UploaderState.Ready;
        }

        private TaxIdBulkUploadInfo RequestTaxIdBulkUploadInfo()
        {
            var childWindow = this.taxIdBulkUploadViewFactory.Create();
            var result = childWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var childViewModel = (TaxIdBulkUploadViewModel)childWindow.DataContext;
                return childViewModel.GetBulkUploadInfo();
            }

            return new();
        }

        private async Task<List<TaxIdBulkUploadListingItem>> FilterTaxIdBulkUpdater(MarketCode market)
        {
            var listingsWithInvalidTaxId = await this.sqlListingDataLoader.GetListingsWithInvalidTaxId(market);

            var bulkListingsViewModel = new TaxIdBulkListingsViewModel(listingsWithInvalidTaxId, market);
            var childWindow = new TaxIdBulkListingsView(bulkListingsViewModel);
            var result = childWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var childViewModel = (TaxIdBulkListingsViewModel)childWindow.DataContext;
                return childViewModel.GetBulkUploadResidentialListingFiltered();
            }

            return new List<TaxIdBulkUploadListingItem>();
        }
    }
}
