using AutoMapper;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntityAttributes;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicHierarchies;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.Permissions;

namespace Infocad.DynamicSpace.Blazor.Client;

public class DynamicSpaceBlazorAutoMapperProfile : Profile
{
    public DynamicSpaceBlazorAutoMapperProfile()
    {
        //DynamicTypeDto => CreateDynamicTypeDto
        CreateMap<DynamicTypeDto, CreateDynamicTypeDto>();
        //DynamicAttributeDto => CreateDynamicAttributeDto
        CreateMap<DynamicAttributeDto, CreateDynamicAttributeDto>();

        CreateMap<DynamicEntityAttributeDto, CreateDynamicEntityAttributeDto>()
            .ForMember(dest => dest.DynamicEntityId, opt => opt.MapFrom(src => src.DynamicEntityId))
            .ForMember(dest => dest.DynamicAttributeId, opt => opt.MapFrom(src => src.DynamicAttributeId))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
            .ForMember(dest => dest.DynamicFormatId, opt => opt.MapFrom(src => src.DynamicFormatId));

        CreateMap<CreateDynamicEntityAttributeDto, DynamicEntityAttributeDto>()
            .ForMember(dest => dest.DynamicEntityId, opt => opt.MapFrom(src => src.DynamicEntityId))
            .ForMember(dest => dest.DynamicAttributeId, opt => opt.MapFrom(src => src.DynamicAttributeId))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
            .ForMember(dest => dest.Label, opt => opt.MapFrom(src => src.Label))
            .ForMember(dest => dest.DynamicFormatId, opt => opt.MapFrom(src => src.DynamicFormatId));

        CreateMap<DynamicEntityDto, CreateDynamicEntityDto>();
        CreateMap<CreateDynamicEntityDto, DynamicEntityDto>();

        CreateMap<DynamicFormatDto, CreateDynamicFormatDto>();
        CreateMap<DynamicFormatDto, UpdateDynamicFormatDto>().ReverseMap();

        CreateMap<DynamicRuleDto, CreateDynamicRuleDto>();
        CreateMap<DynamicRuleDto, UpdateDynamicRuleDto>().ReverseMap();

        CreateMap<DynamicHierarchyDto,CreateDynamicHierarchyDto>().ReverseMap();
        
        CreateMap<DynamicEntityDto, UpdateDynamicEntityDto>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

        CreateMap<UpdateDynamicEntityDto, DynamicEntityDto>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));
        
        CreateMap<DynamicFormatDto, CreateDynamicFormatDto>();
    }
}
