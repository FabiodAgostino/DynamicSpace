using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class CreateDynamicHierarchyDto
{
    [Required]
    public string Name { get; set; }
    public string? Description { get; set; }
    public bool Default { get; set; }
}