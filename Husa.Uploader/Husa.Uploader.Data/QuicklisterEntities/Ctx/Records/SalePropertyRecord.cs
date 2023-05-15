namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Extensions.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public record SalePropertyRecord
    {
        public Guid Id { get; set; }

        public string OwnerName { get; set; }

        public Guid? PlanId { get; set; }

        public Guid? CommunityId { get; set; }

        public string Address { get; set; }

        public DateTime SysCreatedOn { get; set; }

        public DateTime? SysModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? SysModifiedBy { get; set; }

        public Guid? SysCreatedBy { get; set; }

        public DateTime SysTimestamp { get; set; }

        public Guid CompanyId { get; set; }

        public AddressRecord AddressInfo { get; set; }

        public PropertyRecord PropertyInfo { get; set; }

        public SpacesDimensionsRecord SpacesDimensionsInfo { get; set; }

        public FeaturesRecord FeaturesInfo { get; set; }

        public FinancialRecord FinancialInfo { get; set; }

        public ShowingRecord ShowingInfo { get; set; }

        public SchoolRecord SchoolsInfo { get; set; }

        public SalesOfficeRecord SalesOfficeInfo { get; set; }

        public ICollection<RoomRecord> Rooms { get; set; }

        public ICollection<OpenHouseRecord> OpenHouses { get; set; }
    }
}
