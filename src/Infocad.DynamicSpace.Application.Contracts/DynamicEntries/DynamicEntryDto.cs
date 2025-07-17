using System;
using System.Collections.Generic;
using System.Threading.Channels;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.DynamicEntry;

public class DynamicEntryDto: ExtensibleEntityDto<Guid>
{
    public Guid DynamicEntityId { get; set; }
}