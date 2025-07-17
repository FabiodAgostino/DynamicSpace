using System;
using Volo.Abp;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyEntityNotFoundException: BusinessException
{
    public DynamicHierarchyEntityNotFoundException(Guid id)
        : base(DynamicSpaceDomainErrorCodes.DynamicHierarchyEntityNotFound, id.ToString())
    {
        WithData("id", id);
    }
   
}