using System;

namespace Infocad.DynamicSpace.DynamicAttributes;

public class NavSettingDto
{
    public Guid Entity { get; set; }
    public bool IsHybrid { get; set; }
    public string NavField { get; set; }
    public string UIField { get; set; }
    public string EntityName { get; set; }
    public string? FullQualifieldName { get; set; }
}