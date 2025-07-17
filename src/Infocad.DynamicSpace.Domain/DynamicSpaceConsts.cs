using Volo.Abp.Identity;

namespace Infocad.DynamicSpace;

public static class DynamicSpaceConsts
{
    public const string DbTablePrefix = "DynamicSpace";
    public const string? DbSchema = null;
    public const string AdminEmailDefaultValue = IdentityDataSeedContributor.AdminEmailDefaultValue;
    public const string AdminPasswordDefaultValue = IdentityDataSeedContributor.AdminPasswordDefaultValue;
}
