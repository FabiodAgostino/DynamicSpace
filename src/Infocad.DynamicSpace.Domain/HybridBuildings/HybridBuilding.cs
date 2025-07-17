using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.HybridBuildings
{
    public class HybridBuilding : AuditedAggregateRoot<Guid>, IMultiTenant
    {
        public Guid? TenantId { get; set; }
        public Guid? DynamicEntityId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public string Name { get; set; }
    }
}
