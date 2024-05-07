namespace Husa.Uploader.Desktop.ViewModels
{
    using System.Windows;
    using System.Windows.Input;
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Factories;
    using Husa.Uploader.Desktop.Models;
    using Husa.Uploader.Desktop.Views;
    using Microsoft.Extensions.Logging;

    public partial class ShellViewModel
    {
        private readonly IAbstractFactory<BulkUploadView> bulkUploadViewFactory;
        private readonly IBulkUploadFactory bulkUploadFactory;
        private ICommand startBulkUploadCommand;

        public ICommand StartBulkUploadCommand
        {
            get
            {
                this.startBulkUploadCommand ??= new RelayAsyncCommand(param => this.StartBulkUpload(), param => true);
                return this.startBulkUploadCommand;
            }
        }

        private async Task StartBulk(Func<CancellationToken, Task<UploadResult>> action)
        {
            this.ShowCancelButton = true;
            try
            {
                this.signalRConnectionTriesError = 0;
                this.State = UploaderState.UploadInProgress;
                // 2. Execute de action
                var response = await this.RunBulkAction(action);
                this.HandleUploadExecutionResult(response);
            }
            catch (Exception exception)
            {
                this.logger.LogError(exception, "Failed when processing the user request.");
                this.HandleUploadExecutionResult(response: UploadResult.Failure);
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

        private async Task<UploadResult> RunBulkAction(Func<CancellationToken, Task<UploadResult>> action)
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
            }
        }

        private async Task StartBulkUpload()
        {
            var bulkUploadInfo = this.RequestBulkUploadInfo();
            if (!bulkUploadInfo.IsValid)
            {
                this.FinishBulkUpload();
                return;
            }

            var filteredBulkListings = this.FilterBulkUpdater(bulkUploadInfo.Market.Value);
            if (!filteredBulkListings.Any())
            {
                this.FinishBulkUpload();
                return;
            }

            this.ShowCancelButton = true;
            var uploader = this.bulkUploadFactory.Create<IBulkUploadListings>(bulkUploadInfo.Market.Value, bulkUploadInfo.RequestFieldChange.Value, filteredBulkListings);
            await this.StartBulk(action: uploader.Upload);
        }

        private void FinishBulkUpload()
        {
            if (this.State == UploaderState.UploadInProgress)
            {
                try
                {
                    this.bulkUploadFactory.Uploader.Logout();
                }
                catch (Exception ex)
                {
                    this.logger.LogWarning(ex, "Failed to logout of listing {ResidentialListingRequestId} with {UploaderType}", this.SelectedListingRequest.FullListing.ResidentialListingRequestID, this.uploadFactory.Uploader.GetType().ToString());
                }

                this.bulkUploadFactory.Uploader.CancelOperation();
            }

            this.ShowCancelButton = false;
            this.State = UploaderState.Ready;
        }

        private BulkUploadInfo RequestBulkUploadInfo()
        {
            var childWindow = this.bulkUploadViewFactory.Create();
            var result = childWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var childViewModel = (BulkUploadViewModel)childWindow.DataContext;
                return childViewModel.GetBulkUploadInfo();
            }

            return new();
        }

        private List<UploadListingItem> FilterBulkUpdater(MarketCode market)
        {
            var bulkListingsViewModel = new BulkListingsViewModel(this.listingRequests, market);
            var childWindow = new BulkListingsView(bulkListingsViewModel);
            var result = childWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var childViewModel = (BulkListingsViewModel)childWindow.DataContext;
                return childViewModel.GetBulkUploadResidentialListingFiltered();
            }

            return new();
        }
    }
}
