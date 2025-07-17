using Infocad.DynamicSpace.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Infocad.DynamicSpace.Permissions;

public class DynamicSpacePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DynamicSpacePermissions.GroupName);

        //Define your own permissions here. Example:
        var myDynamicTypePermission = myGroup.AddPermission(DynamicSpacePermissions.DynamicType.Default, L("Permission:DynamicType"));
        myDynamicTypePermission.AddChild(DynamicSpacePermissions.DynamicType.Create, L("Permission:DynamicType.Create"));
        myDynamicTypePermission.AddChild(DynamicSpacePermissions.DynamicType.Edit, L("Permission:DynamicType.Edit"));
        myDynamicTypePermission.AddChild(DynamicSpacePermissions.DynamicType.Delete, L("Permission:DynamicType.Delete"));
        
        var myDynamicAttributePermission = myGroup.AddPermission(DynamicSpacePermissions.DynamicAttribute.Default, L("Permission:DynamicAttribute"));
        myDynamicAttributePermission.AddChild(DynamicSpacePermissions.DynamicAttribute.Create, L("Permission:DynamicAttribute.Create"));
        myDynamicAttributePermission.AddChild(DynamicSpacePermissions.DynamicAttribute.Edit, L("Permission:DynamicAttribute.Edit"));
        myDynamicAttributePermission.AddChild(DynamicSpacePermissions.DynamicAttribute.Delete, L("Permission:DynamicAttribute.Delete"));

        var myDynamicEntityPermission = myGroup.AddPermission(DynamicSpacePermissions.DynamicEntity.Default, L("Permission:DynamicEntity"));
        myDynamicEntityPermission.AddChild(DynamicSpacePermissions.DynamicEntity.Create, L("Permission:DynamicEntity.Create"));
        myDynamicEntityPermission.AddChild(DynamicSpacePermissions.DynamicEntity.Edit, L("Permission:DynamicEntity.Edit"));
        myDynamicEntityPermission.AddChild(DynamicSpacePermissions.DynamicEntity.Delete, L("Permission:DynamicEntity.Delete"));
        myDynamicEntityPermission.AddChild(DynamicSpacePermissions.DynamicEntity.DescView, L("Permission:DynamicEntity.DescView"));

        var myDynamicFormatPermission = myGroup.AddPermission(DynamicSpacePermissions.DynamicFormat.Default, L("Permission:DynamicFormat"));
        myDynamicFormatPermission.AddChild(DynamicSpacePermissions.DynamicFormat.Create, L("Permission:DynamicFormat.Create"));
        myDynamicFormatPermission.AddChild(DynamicSpacePermissions.DynamicFormat.Edit, L("Permission:DynamicFormat.Edit"));
        myDynamicFormatPermission.AddChild(DynamicSpacePermissions.DynamicFormat.Delete, L("Permission:DynamicFormat.Delete"));


        var myDynamicRulePermission = myGroup.AddPermission(DynamicSpacePermissions.DynamicRule.Default, L("Permission:DynamicRule"));
        myDynamicRulePermission.AddChild(DynamicSpacePermissions.DynamicRule.Create, L("Permission:DynamicRule.Create"));
        myDynamicRulePermission.AddChild(DynamicSpacePermissions.DynamicRule.Edit, L("Permission:DynamicRule.Edit"));
        myDynamicRulePermission.AddChild(DynamicSpacePermissions.DynamicRule.Delete, L("Permission:DynamicRule.Delete"));
        var myTotemPermission = myGroup.AddPermission(DynamicSpacePermissions.Totem.Default, L("Permission:TotemRule"));
        myTotemPermission.AddChild(DynamicSpacePermissions.Totem.Create, L("Permission:TotemRule.Create"));
        myTotemPermission.AddChild(DynamicSpacePermissions.Totem.Edit, L("Permission:TotemRule.Edit"));
        myTotemPermission.AddChild(DynamicSpacePermissions.Totem.Delete, L("Permission:TotemRule.Delete"));

        
        var myDynamicHierarchyPermission = myGroup.AddPermission(DynamicSpacePermissions.DynamicHierarchy.Default, L("Permission:DynamicHierarchy"));
        myDynamicHierarchyPermission.AddChild(DynamicSpacePermissions.DynamicHierarchy.Create, L("Permission:DynamicHierarchy.Create"));
        myDynamicHierarchyPermission.AddChild(DynamicSpacePermissions.DynamicHierarchy.Edit, L("Permission:DynamicHierarchy.Edit"));
        myDynamicHierarchyPermission.AddChild(DynamicSpacePermissions.DynamicHierarchy.Delete, L("Permission:DynamicHierarchy.Delete"));

        var myFeaturePermission = myGroup.AddPermission(DynamicSpacePermissions.Feature.Default, L("Permission:Feature"));
        myFeaturePermission.AddChild(DynamicSpacePermissions.Feature.Create, L("Permission:Feature.Create"));
        myFeaturePermission.AddChild(DynamicSpacePermissions.Feature.Edit, L("Permission:Feature.Edit"));
        myFeaturePermission.AddChild(DynamicSpacePermissions.Feature.Delete, L("Permission:Feature.Delete"));

        var myDynamicControlPermission = myGroup.AddPermission(DynamicSpacePermissions.DynamicControl.Default, L("Permission:DynamicControl"));
        myDynamicControlPermission.AddChild(DynamicSpacePermissions.DynamicControl.Create, L("Permission:DynamicControl.Create"));
        myDynamicControlPermission.AddChild(DynamicSpacePermissions.DynamicControl.Edit, L("Permission:DynamicControl.Edit"));
        myDynamicControlPermission.AddChild(DynamicSpacePermissions.DynamicControl.Delete, L("Permission:DynamicControl.Delete"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DynamicSpaceResource>(name);
    }
}
