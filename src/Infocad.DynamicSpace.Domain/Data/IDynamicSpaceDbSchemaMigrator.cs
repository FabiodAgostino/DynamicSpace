using System.Threading.Tasks;

namespace Infocad.DynamicSpace.Data;

public interface IDynamicSpaceDbSchemaMigrator
{
    Task MigrateAsync();
}
