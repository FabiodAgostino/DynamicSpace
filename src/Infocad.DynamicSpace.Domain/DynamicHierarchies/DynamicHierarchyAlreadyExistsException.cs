using Volo.Abp;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyAlreadyExistsException: BusinessException
{
    public DynamicHierarchyAlreadyExistsException(string name)
        : base(DynamicSpaceDomainErrorCodes.DynamicHierarchyAlreadyExists, name)
    {
        WithData("name", name);
    }
}