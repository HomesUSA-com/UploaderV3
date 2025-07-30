namespace Husa.Uploader.Core.Interfaces.ShowingTime
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Api.Contracts.Response.ShowingTime;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Data.Entities;

    public interface IShowingTimeUploadService
    {
        MarketCode CurrentMarket { get; }
        Task<bool> FindListingOnMls(string mlsNumber, CancellationToken cancellationToken = default);
        Task<bool> GetInShowingTimeSite(Guid companyId, string mlsNumber, CancellationToken cancellationToken = default);
        Task SetAppointmentCenter(AppointmentSettingsResponse info, CancellationToken cancellationToken);
        Task SetAppointmentSettings(AppointmentSettingsResponse info, CancellationToken cancellationToken = default);
        Task SetAppointmentRestrictions(AppointmentRestrictionsResponse info, CancellationToken cancellationToken = default);
        Task SetAccessInformation(AccessInformationResponse info, CancellationToken cancellationToken = default);
        Task SetAdditionalInstructions(AdditionalInstructionsResponse info, CancellationToken cancellationToken = default);
        Task SetDrivingDirections(ResidentialListingRequest request, CancellationToken cancellationToken = default);
        Task<bool> AddExistentContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default);
        Task<bool> AddNewContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default);
        Task EditContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default);
        Task SetContactConfirmSection(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default);
        Task SetContactNotificationSection(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default);
        Task SetContact(ContactDetailResponse contact, int position, CancellationToken cancellationToken = default);
        Task SetContacts(IEnumerable<ContactDetailResponse> contacts, CancellationToken cancellationToken);
        Task<UploaderResponse> Upload(ResidentialListingRequest request, CancellationToken cancellationToken = default);
        Task<int> DeleteDuplicateClients(Guid companyId, string mlsNumber, CancellationToken cancellationToken = default);
        void CancelOperation();
    }
}
