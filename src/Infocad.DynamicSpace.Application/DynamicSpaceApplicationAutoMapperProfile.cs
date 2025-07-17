using AutoMapper;
using Infocad.DynamicSpace.DynamicAttirbutes;
using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicControls;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntityAttributes;
using Infocad.DynamicSpace.DynamicEntry;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicHierarchies;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.FileManagement;
using Infocad.DynamicSpace.HybridBuildings;
using Infocad.DynamicSpace.HybridCompanies;
using Infocad.DynamicSpace.HybridRooms;
using Infocad.DynamicSpace.Migrations;
using Infocad.DynamicSpace.Permissions;
using Infocad.DynamicSpace.Totems;
using System.Linq;
using Volo.Abp.AutoMapper;


namespace Infocad.DynamicSpace;

public class DynamicSpaceApplicationAutoMapperProfile : Profile
{
    public DynamicSpaceApplicationAutoMapperProfile()
    {
        CreateMap<DynamicType, DynamicTypeDto>();
        CreateMap<CreateDynamicTypeDto, DynamicType>();

        CreateMap<DynamicAttribute, DynamicAttributeDto>();
        CreateMap<CreateDynamicAttributeDto, DynamicAttribute>();
        
        CreateMap<DynamicFormat, DynamicFormatDto>();
        CreateMap<CreateDynamicFormatDto, DynamicFormat>();

        CreateMap<DynamicRule, DynamicRuleDto>();
        CreateMap<CreateDynamicRuleDto, DynamicRule>();
        CreateMap<UpdateDynamicRuleDto, DynamicRule>().ReverseMap();


        CreateMap<CreateDynamicEntityDto, DynamicEntity>();
        CreateMap<DynamicEntityDto, CreateDynamicEntityDto>();
        CreateMap<UpdateDynamicFormatDto, DynamicFormat>().ReverseMap();
        CreateMap<DynamicFormatDto, UpdateDynamicFormatDto>().ReverseMap();

        CreateMap<DynamicEntity, DynamicEntityDto>();
        CreateMap<DynamicEntity, UpdateDynamicEntityDto>()
            .ForMember(dest => dest.Attributes, opt => opt.Ignore());
        CreateMap<DynamicEntityDto, DynamicEntity>()
            .ForMember(dest => dest.Attributes, opt => opt.Ignore());
        
        CreateMap<DynamicEntityAttribute, DynamicEntityAttributeDto>();
        CreateMap<CreateDynamicEntityAttributeDto, DynamicEntityAttribute>();

        CreateMap<DynamicEntries.DynamicEntry, DynamicEntryDto>();
        CreateMap<DynamicEntryDto, DynamicEntries.DynamicEntry>();

        CreateMap<HybridCompany, HybridCompanyDto>();
        CreateMap<HybridCompanyDto, HybridCompany>();

        CreateMap<HybridBuilding, HybridBuildingDto>();
        CreateMap<HybridBuildingDto, HybridBuilding>();

        CreateMap<HybridRooms.HybridRoom, HybridRoomDto>();
        CreateMap<HybridRoomDto, HybridRooms.HybridRoom>();

        CreateMap<Totems.Totem, TotemDto>();
        CreateMap<CreateTotemDto, Totems.Totem>()
            .Ignore(x => x.TenantId); 
        
        CreateMap<DynamicHierarchy, DynamicHierarchyDto>().ReverseMap();
        CreateMap<CreateDynamicHierarchyDto, DynamicHierarchy>().ReverseMap();
        CreateMap<CreateDynamicHierarchyDto, DynamicHierarchyDto>().ReverseMap();
        
        CreateMap<DynamicHierarchyEntities, DynamicHierarchyEntityDto>().ReverseMap();

        CreateMap<FileInfoEntity, FileInfoDto>();
        CreateMap<DynamicControl, DynamicControlDto>();

        CreateMap<CreateDynamicControlDto, DynamicControl>();
        CreateMap<UpdateDynamicControlDto, DynamicControl>();
        CreateMap<DynamicControl, DynamicControlDto>();


    }
}