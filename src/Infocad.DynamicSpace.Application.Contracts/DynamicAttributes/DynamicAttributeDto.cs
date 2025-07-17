using System;
using System.Security.AccessControl;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicAttributes;

public class DynamicAttributeDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public DynamicAttributeType Type { get; set; }
    public string? NavigationSettings { get; set; }
}