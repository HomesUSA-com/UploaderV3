namespace Husa.Uploader.Desktop.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Input;
    using Husa.Extensions.Common.Enums;
    using Husa.Extensions.Common.Exceptions;
    using Husa.Quicklister.Extensions.Domain.Enums;
    using Husa.Uploader.Core.Interfaces.ServiceActions;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;
    using Husa.Uploader.Desktop.Commands;
    using Husa.Uploader.Desktop.Factories;
    using Husa.Uploader.Desktop.Models;
    using Husa.Uploader.Desktop.Views;
    using Microsoft.Extensions.Logging;

    [ExcludeFromCodeCoverage]
    public partial class ShellViewModel
    {
        private readonly IAbstractFactory<BulkUploadView> bulkUploadViewFactory;
        private readonly IBulkUploadFactory bulkUploadFactory;
        private List<UploadListingItem> filteredListings;
        private ICommand startBulkUploadCommand;

        public bool CanStartBulk => this.CurrentEntity == Entity.Listing && this.ListingRequests?.Any() != null;

        public ICommand StartBulkUploadCommand
        {
            get
            {
                this.startBulkUploadCommand ??= new RelayAsyncCommand(param => this.StartBulkUpload(), param => this.CanStartBulk);
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
                await this.SetBulkFullRequestsInformation();
                var token = this.cancellationTokenSource.Token;
                return await Task.Run(() => action(token));
            }
            catch (OperationCanceledException)
            {
                return this.CatchCanceledException();
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Failed to logout of listing {ResidentialListingRequestId}", this.SelectedListingRequest.FullListing.ResidentialListingRequestID);
                return UploadResult.Failure;
            }
            finally
            {
                this.cancellationTokenSource.Dispose();
                this.cancellationTokenSource = null;

                this.ShowCancelButton = false;
                this.State = UploaderState.Ready;
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

            var filteredBulkListings = await this.FilterBulkUpdater(bulkUploadInfo.Market.Value, bulkUploadInfo.RequestFieldChange.Value);
            if (!filteredBulkListings.Any())
            {
                this.FinishBulkUpload();
                return;
            }

            this.filteredListings = filteredBulkListings.ToList();

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
                    this.logger.LogWarning(ex, "Failed to logout of listing {ResidentialListingRequestId} with {uploaderType}", this.SelectedListingRequest.FullListing.ResidentialListingRequestID, this.uploadFactory.Uploader.GetType().ToString());
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

        private async Task<List<UploadListingItem>> FilterBulkUpdater(MarketCode market, RequestFieldChange requestFieldChange)
        {
            var fullListings = await this.sqlDataLoader.GetListingRequestsByMarketAndAction(market, requestFieldChange);
            var uploadListingItems = GetUploadListingItems(fullListings);

            var bulkListingsViewModel = new BulkListingsViewModel(uploadListingItems, market);
            var childWindow = new BulkListingsView(bulkListingsViewModel);
            var result = childWindow.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var childViewModel = (BulkListingsViewModel)childWindow.DataContext;
                return childViewModel.GetBulkUploadResidentialListingFiltered();
            }

            return new List<UploadListingItem>();

            ObservableCollection<UploadListingItem> GetUploadListingItems(IEnumerable<ResidentialListingRequest> fullListings)
            {
                try
                {
                    var uploadItems = fullListings.Select(listingRequest =>
                    {
                        var worker = string.Empty;
                        var workingStatus = string.Empty;
                        var workingSourceAction = string.Empty;
                        var uploadItem = listingRequest.AsUploadItem(
                            builderName: "Ben Caballero",
                            brokerOffice: "HHRE00",
                            isLeasing: string.Empty,
                            isLot: "No",
                            this.CurrentEntity,
                            worker,
                            workingStatus,
                            workingSourceAction);

                        return uploadItem;
                    }).ToList();

                    return new ObservableCollection<UploadListingItem>(uploadItems);
                }
                catch
                {
                    return new ObservableCollection<UploadListingItem>();
                }
            }
        }

        private async Task SetBulkFullRequestsInformation()
        {
            foreach (var bulkListing in this.filteredListings)
            {
                var requestData = await this.sqlDataLoader.GetListingRequest(
                    bulkListing.RequestId,
                    bulkListing.FullListing.MarketCode,
                    this.cancellationTokenSource.Token)
                    ?? throw new NotFoundException<ResidentialListingRequest>(bulkListing.RequestId);
                bulkListing.SetFullListing(requestData);
            }
        }
    }
}
