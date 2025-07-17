using Volo.Abp;

namespace Infocad.DynamicSpace.DynamicEntities;

public class DynamicEntityAttributeAlreadyExistsException : BusinessException
{
    public DynamicEntityAttributeAlreadyExistsException(string name)
        : base(DynamicSpaceDomainErrorCodes.DynamicEntityAttributeAlreadyExists, name)
    {
        WithData("name", name);
    }
}