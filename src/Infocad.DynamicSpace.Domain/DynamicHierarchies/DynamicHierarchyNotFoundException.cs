using System;
using Volo.Abp;

namespace Infocad.DynamicSpace.DynamicHierarchies;

public class DynamicHierarchyNotFoundException : BusinessException
{
    public DynamicHierarchyNotFoundException(Guid id)
        : base(DynamicSpaceDomainErrorCodes.DinamycHierarchyNotFound, id.ToString())
    {
        WithData("id", id);
    }
}