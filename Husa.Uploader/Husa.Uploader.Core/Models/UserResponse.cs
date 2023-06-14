namespace Husa.Uploader.Core.Models
{
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;

    public class UserResponse
    {
        public static readonly Guid SuperUserId = new Guid("8e445865-a24d-4543-a6c6-9443d048cdb9");

        public int UserID { get; set; }
        public string UserGUID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public object Email2 { get; set; }
        public object IsWindowsAccount { get; set; }
        public string Username { get; set; }

        [JsonProperty("addressID_Primary")]
        [JsonPropertyName("addressID_Primary")]
        public int AddressID { get; set; }
        public string Street1 { get; set; }
        public object Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PhoneBusiness { get; set; }
        public string PhoneMobile { get; set; }
        public string PhoneOther { get; set; }
        public object Fax { get; set; }
        public object WebUrl { get; set; }
        public DateTime LastLoginOn { get; set; }
        public DateTime LastActivityOn { get; set; }
        public object LastDisabledOn { get; set; }
        public object Oid { get; set; }
        public int? PrimaryPhone { get; set; }
        public bool AccountVerified { get; set; }
        public string VerificationHash { get; set; }
        public DateTime LastPasswordChange { get; set; }
        public string LastLoginIP { get; set; }
        public string LefUrl { get; set; }
        public bool IsWordPressAccount { get; set; }
        public int SysStatusID { get; set; }
        public string SysState { get; set; }
        public int SysCreatedBy { get; set; }
        public DateTime SysCreatedOn { get; set; }
        public int SysModifiedBy { get; set; }
        public DateTime SysModifiedOn { get; set; }
        public DateTime SysTimestamp { get; set; }
        public int SysOwnedBy { get; set; }

        public static UserResponse DefaultUser() => new()
        {
            UserID = 1,
            UserGUID = SuperUserId.ToString(),
            Username = "superadmin@homesusa.com",
            FirstName = "Super",
            LastName = "Admin",
            FullName = "Super Admin",
        };
    }
}
