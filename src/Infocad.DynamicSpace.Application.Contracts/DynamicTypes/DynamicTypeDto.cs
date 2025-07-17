using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicTypes;

public class DynamicTypeDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}