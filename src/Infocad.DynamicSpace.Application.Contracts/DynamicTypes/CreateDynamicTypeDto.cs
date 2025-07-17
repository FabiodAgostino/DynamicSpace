using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicTypes;

public class CreateDynamicTypeDto
{
    [Required]
    [StringLength(DynamicTypeConsts.MaxNameLenght, MinimumLength = DynamicTypeConsts.MinNameLenght)]
    public string Name { get; set; }

    [StringLength(DynamicTypeConsts.MaxDescriptionLenght)]
    public string Description { get; set; }
}