using System;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.DynamicEntries;

public class DynamicEntry : AuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public Guid DynamicEntityId { get; set; }
    
 }

