namespace Husa.Uploader.Data.QuicklisterEntities.Ctx.Records
{
    using Husa.Extensions.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public record SalePropertyRecord
    {
        public virtual Guid Id { get; set; }

        public virtual string OwnerName { get; set; }

        public virtual Guid? PlanId { get; set; }

        public virtual Guid? CommunityId { get; set; }

        public virtual string Address { get; set; }

        public virtual DateTime SysCreatedOn { get; set; }

        public virtual DateTime? SysModifiedOn { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual Guid? SysModifiedBy { get; set; }

        public virtual Guid? SysCreatedBy { get; set; }

        public virtual DateTime SysTimestamp { get; set; }

        public virtual Guid CompanyId { get; set; }

        public virtual AddressRecord AddressInfo { get; set; }

        public virtual PropertyRecord PropertyInfo { get; set; }

        public virtual SpacesDimensionsRecord SpacesDimensionsInfo { get; set; }

        public virtual FeaturesRecord FeaturesInfo { get; set; }

        public virtual FinancialRecord FinancialInfo { get; set; }

        public virtual ShowingRecord ShowingInfo { get; set; }

        public virtual SchoolRecord SchoolsInfo { get; set; }

        public virtual SalesOfficeRecord SalesOfficeInfo { get; set; }

        public virtual ICollection<RoomRecord> Rooms { get; set; }

        public virtual ICollection<OpenHouseRecord> OpenHouses { get; set; }
    }
}
