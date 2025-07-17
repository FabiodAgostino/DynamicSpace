using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Infocad.DynamicSpace.DynamicAttributes;

public class CreateDynamicAttributeDto
{
    [Required]
    [StringLength(DynamicAttributeConsts.MaxNameLenght, MinimumLength = DynamicAttributeConsts.MinNameLenght)]
    public string Name { get; set; }

    [StringLength(DynamicAttributeConsts.MaxDescriptionLenght)]
    public string? Description { get; set; }

    [Required]
    public DynamicAttributeType Type { get; set; }

    [RequiredIfNavigation(nameof(Type), DynamicAttributeType.Navigation)]
    public string NavigationSettings { get; set; }
}