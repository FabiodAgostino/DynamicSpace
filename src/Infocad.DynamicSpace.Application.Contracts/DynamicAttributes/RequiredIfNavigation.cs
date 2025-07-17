using System.ComponentModel.DataAnnotations;

namespace Infocad.DynamicSpace.DynamicAttributes;
public class RequiredIfNavigationAttribute : ValidationAttribute
{
    private readonly string _typePropertyName;

    public RequiredIfNavigationAttribute(string typePropertyName, DynamicAttributeType navigation)
    {
        _typePropertyName = typePropertyName;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var typeProperty = validationContext.ObjectType.GetProperty(_typePropertyName);
        if (typeProperty == null)
            return ValidationResult.Success;

        var typeValue = typeProperty.GetValue(validationContext.ObjectInstance, null);
        if (typeValue != null && typeValue.ToString() == "Navigation" && string.IsNullOrWhiteSpace(value as string))
        {
            return new ValidationResult("NavigationSettings è obbligatorio quando Type è Navigation.");
        }

        return ValidationResult.Success;
    }
}