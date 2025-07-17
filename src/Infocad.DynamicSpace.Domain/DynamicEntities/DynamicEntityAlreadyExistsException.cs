using Volo.Abp;

namespace Infocad.DynamicSpace.DynamicEntities;

public class DynamicEntityAlreadyExistsException : BusinessException
{
    public DynamicEntityAlreadyExistsException(string name)
        : base(DynamicSpaceDomainErrorCodes.DyanmicEntityAlreadyExists, name)
    {
        WithData("name", name);
    }
}