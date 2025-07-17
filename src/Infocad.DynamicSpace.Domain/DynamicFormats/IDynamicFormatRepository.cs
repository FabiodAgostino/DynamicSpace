using System;
using Volo.Abp.Domain.Repositories;

namespace Infocad.DynamicSpace.DynamicFormats;

public interface IDynamicFormatRepository : IRepository<DynamicFormat,Guid>
{
    
}