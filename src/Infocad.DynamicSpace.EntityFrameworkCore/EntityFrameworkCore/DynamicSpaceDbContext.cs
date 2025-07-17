using Infocad.DynamicSpace.DynamicAttributes;
using Infocad.DynamicSpace.DynamicControls;
using Infocad.DynamicSpace.DynamicEntities;
using Infocad.DynamicSpace.DynamicEntries;
using Infocad.DynamicSpace.DynamicFormats;
using Infocad.DynamicSpace.DynamicHierarchies;
using Infocad.DynamicSpace.DynamicRules;
using Infocad.DynamicSpace.DynamicTypes;
using Infocad.DynamicSpace.FileManagement;
using Infocad.DynamicSpace.HybridBuildings;
using Infocad.DynamicSpace.HybridCompanies;
using Infocad.DynamicSpace.HybridRooms;
using Infocad.DynamicSpace.Totems;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Server;
using System.Runtime.CompilerServices;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;


namespace Infocad.DynamicSpace.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class DynamicSpaceDbContext :
    AbpDbContext<DynamicSpaceDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DynamicSpaceDbContext(DbContextOptions<DynamicSpaceDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        builder.Entity<DynamicSpace.DynamicAttirbutes.DynamicAttribute>(b =>
         {
             b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicAttribute", DynamicSpaceConsts.DbSchema);
             b.ConfigureByConvention(); //auto configure for the base class props
             b.Property(da => da.Name).IsRequired().HasMaxLength(DynamicAttributeConsts.MaxNameLenght);
         });
         
         builder.Entity<DynamicType>(b =>
         {
             b.HasKey(v=> v.Id);
             b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicType", DynamicSpaceConsts.DbSchema);
             b.ConfigureByConvention(); //auto configure for the base class props
             b.Property(da => da.Name).IsRequired().HasMaxLength(255);
             b.Property(da => da.Description).HasMaxLength(255);
         });

        builder.Entity<DynamicControl>(b =>
        {
            b.HasKey(v => v.Id);
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicControl", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(da => da.Name).IsRequired().HasMaxLength(255);
            b.Property(da => da.Description).HasMaxLength(255);
            b.Property(da => da.ComponentType).IsRequired();
            b.Property(da => da.Type).IsRequired();
        });

        builder.Entity<DynamicEntities.DynamicEntity>(b =>
         {
             b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicEntity", DynamicSpaceConsts.DbSchema);
             b.ConfigureByConvention(); //auto configure for the base class props
             b.Property(da => da.Name).IsRequired().HasMaxLength(255);
             b.Property(da => da.Description).HasMaxLength(255);
             b.HasOne<DynamicType>().WithMany().HasForeignKey(e => e.DynamicTypeId).IsRequired();
         });
         
         builder.Entity<DynamicHierarchy>(b =>
         {
             b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicHierarchy", DynamicSpaceConsts.DbSchema);
             b.ConfigureByConvention(); //auto configure for the base class props
             b.Property(da => da.Name).IsRequired().HasMaxLength(255);
             b.Property(da => da.Description).HasMaxLength(1000);
             b.HasMany(e => e.Entities).WithOne().HasForeignKey(e => e.DynamicHierarchyId).IsRequired();
         });

        builder.Entity<DynamicHierarchyEntities>(b =>
        {
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicHierarchyEntities", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasKey(e => e.Id);
            b.HasIndex(e => new { e.DynamicHierarchyId, e.DynamicSourceEntityId, e.DynamicTargetEntityId }).IsUnique();
            b.HasOne<DynamicEntity>().WithMany().HasForeignKey(e => e.DynamicSourceEntityId).IsRequired(false);
        });


        builder.Entity<DynamicEntry>(b =>
         {
             b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicEntry", DynamicSpaceConsts.DbSchema);
             b.ConfigureByConvention(); //auto configure for the base class props
             b.HasOne<DynamicEntity>().WithMany().HasForeignKey(e => e.DynamicEntityId);
         });

        builder.Entity<HybridCompany>(b =>
        {
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "HybridCompany", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.HasOne<DynamicEntity>().WithMany().HasForeignKey(e => e.DynamicEntityId);
        });

        builder.Entity<HybridBuilding>(b =>
        {
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "HybridBuilding", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.HasOne<DynamicEntity>().WithMany().HasForeignKey(e => e.DynamicEntityId);
        });

        builder.Entity<HybridRoom>(b =>
        {
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "HybridRoom", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.HasOne<DynamicEntity>().WithMany().HasForeignKey(e => e.DynamicEntityId);
        });

        builder.Entity<Totem>(b =>
        {
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "Totem", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<DynamicEntityAttribute>(b =>
         {
             b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicEntityAttribute", DynamicSpaceConsts.DbSchema);
             b.ConfigureByConvention();
        
             b.HasOne<DynamicEntity>()
              .WithMany(e => e.Attributes)  
              .HasForeignKey(e => e.DynamicEntityId)
              .IsRequired();
        
             b.HasOne<Infocad.DynamicSpace.DynamicAttirbutes.DynamicAttribute>().WithMany()
              .HasForeignKey(e => e.DynamicAttributeId).IsRequired();

             b.HasOne<DynamicFormat>()
                .WithMany()
                .HasForeignKey(e => e.DynamicFormatId)
                .IsRequired(false);

             b.HasOne<DynamicRule>()
                .WithMany()
                .HasForeignKey(e => e.DynamicRuleId)
                .IsRequired(false);

             b.HasOne<DynamicControl>()
               .WithMany()
               .HasForeignKey(e => e.DynamicControlId)
               .IsRequired(false);
         });
        

        builder.Entity<DynamicFormat>(b =>
        {
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicFormat", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(f => f.Name).IsRequired().HasMaxLength(255);
            b.Property(f => f.Description).HasMaxLength(1000);
            b.Property(f => f.AttributeType).IsRequired();
            b.Property(f => f.FormatPattern).HasMaxLength(255);
        });

        builder.Entity<DynamicRule>(b =>
        {
            b.ToTable(DynamicSpaceConsts.DbTablePrefix + "DynamicRule", DynamicSpaceConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(f => f.Name).IsRequired().HasMaxLength(255);
            b.Property(f => f.Description).HasMaxLength(1000);
            b.Property(f => f.AttributeType).IsRequired();
            b.Property(f => f.Rule).IsRequired();
        });


        builder.Entity<FileInfoEntity>(b =>
        {
            b.ToTable("FileInfos");
            b.ConfigureByConvention();

            b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            b.Property(x => x.BlobName).IsRequired().HasMaxLength(255);
            b.Property(x => x.ContentType).HasMaxLength(255);

            b.HasIndex(x => x.BlobName);
        });
    }

    public DbSet<Infocad.DynamicSpace.DynamicAttirbutes.DynamicAttribute> DynamicAttributes { get; set; }
    public DbSet<DynamicType> EntityTypes { get; set; }
    public DbSet<DynamicEntity> DynamicEntities { get; set; }
    public DbSet<DynamicEntityAttribute> DynamicEntityAttributes { get; set; }
    public DbSet<DynamicEntry> DynamicEntries { get; set; }
    public DbSet<DynamicFormat> DynamicFormat { get; set; }
    public DbSet<DynamicRule> DynamicRules { get; set; }
    public DbSet<HybridBuilding> HybridBuildings { get; set; }
    public DbSet<HybridCompany> HybridCompanies { get; set; }
    public DbSet<HybridRoom> HybridRooms { get; set; }
    public DbSet<Totem> Totems { get; set; }
    public DbSet<DynamicHierarchy> DynamicHierarchies { get; set; }
    public DbSet<DynamicHierarchyEntities> DynamicHierariesEntities { get; set; }
    public DbSet<FileInfoEntity> FileInfos { get; set; }
    public DbSet<DynamicControl> DynamicControls { get; set; }





}