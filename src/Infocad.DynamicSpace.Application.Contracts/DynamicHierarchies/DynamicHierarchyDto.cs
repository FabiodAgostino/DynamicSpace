using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Default { get; set; }
    public IEnumerable<DynamicHierarchyEntityDto> Entities { get; set; }
}