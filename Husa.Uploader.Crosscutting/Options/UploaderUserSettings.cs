namespace Husa.Uploader.Crosscutting.Options
{
    using System;
    using Husa.Extensions.Authorization.Enums;
    using Husa.Extensions.Authorization.Models;

    public class UploaderUserSettings
    {
        public const string Section = "UploaderUser";

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool MLSAdministrator { get; set; }

        public UserRole UserRole { get; set; }

        public UserContext GetUploaderUser() => new()
        {
            Email = this.Email,
            Name = this.Name,
            Id = this.Id,
            IsMLSAdministrator = this.MLSAdministrator,
            UserRole = this.UserRole,
        };
    }
}
