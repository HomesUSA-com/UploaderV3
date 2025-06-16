namespace Husa.Uploader.Core.Interfaces.ShowingTime
{
    using Husa.Extensions.Common.Enums;
    using Husa.Quicklister.Extensions.Api.Contracts.Models.ShowingTime;
    using Husa.Quicklister.Extensions.Domain.Enums.ShowingTime;
    using Husa.Uploader.Core.Models;
    using Husa.Uploader.Crosscutting.Enums;
    using Husa.Uploader.Data.Entities;

    public interface IShowingTimeUploadService
    {
        MarketCode CurrentMarket { get; }
        Task<LoginResult> Login(Guid companyId, CancellationToken cancellationToken = default);
        Task<bool> FindListing(string mlsNumber, CancellationToken cancellationToken = default);
        Task NavigateToListing(string listingId, CancellationToken cancellationToken = default);
        Task SetAppointmentCenter(CancellationToken cancellationToken);
        Task SetAppointmentSettings(AppointmentType appointmentType, CancellationToken cancellationToken = default);
        Task SetAppointmentRestrictions(AppointmentRestrictionsInfo info, CancellationToken cancellationToken = default);
        Task SetAccessInformation(AccessInformationInfo info, CancellationToken cancellationToken = default);
        Task SetAdditionalInstructions(AdditionalInstructionsInfo info, CancellationToken cancellationToken = default);
        Task<bool> AddExistentContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default);
        Task<bool> AddNewContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default);
        Task EditContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default);
        Task SetContactConfirmSection(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default);
        Task SetContactNotificationSection(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default);
        Task SetContact(ContactDetailInfo contact, int position, CancellationToken cancellationToken = default);
        Task SetContacts(IEnumerable<ContactDetailInfo> contacts, CancellationToken cancellationToken);
        Task<UploaderResponse> Upload(ResidentialListingRequest request, bool logIn = true, CancellationToken cancellationToken = default);
        void CancelOperation();
    }
}
