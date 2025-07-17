using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.HybridRooms
{
    public class HybridRoom : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid? DynamicEntityId { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }

        public HybridRoom()
        {
            Id = Guid.NewGuid();
        }

    }
}
