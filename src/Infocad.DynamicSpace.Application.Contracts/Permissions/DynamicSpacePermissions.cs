using Volo.Abp.Reflection;

namespace Infocad.DynamicSpace.Permissions;

public static class DynamicSpacePermissions
{
    public const string GroupName = "DynamicSpace";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(DynamicSpacePermissions));
    }

    public static class DynamicAttribute
    {
        public const string Default = GroupName + ".DynamicAttribute";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class DynamicType
    {
        public const string Default = GroupName + ".DynamicType";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    
    public static class DynamicEntity
    {
        public const string Default = GroupName + ".DynamicEntity";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string DescView = Default + ".DescView";
    }

    public static class DynamicFormat
    {
        public const string Default = GroupName + ".DynamicFormat";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Totem
    {
        public const string Default = GroupName + ".Totem";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class DynamicRule
    {
        public const string Default = GroupName + ".DynamicRule";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class DynamicControl
    {
        public const string Default = GroupName + ".DynamicControl";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class DynamicHierarchy
    {
        public const string Default = GroupName + ".DynamicHierarchy";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Feature
    {
        public const string Default = GroupName + ".Feature";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

}
