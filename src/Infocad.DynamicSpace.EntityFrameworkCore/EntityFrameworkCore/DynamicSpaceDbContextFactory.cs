using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infocad.DynamicSpace.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class DynamicSpaceDbContextFactory : IDesignTimeDbContextFactory<DynamicSpaceDbContext>
{
    public DynamicSpaceDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        DynamicSpaceEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<DynamicSpaceDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new DynamicSpaceDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Infocad.DynamicSpace.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
