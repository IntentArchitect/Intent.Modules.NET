using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CqrsSpike.Scenarios.Infrastructure;

/// <summary>
/// Moves an app's DbContext from EF Core InMemory to SQL Server. The apps keep InMemory so they run
/// without a database; the tests need a provider that enforces transactions and unique indexes.
/// </summary>
internal static class SqlServerDatabase
{
    public static string For(string serverConnectionString, string database) =>
        new SqlConnectionStringBuilder(serverConnectionString) { InitialCatalog = database }.ConnectionString;

    public static void Replace<TContext>(IServiceCollection services, string connectionString)
        where TContext : DbContext
    {
        // EF Core 9+ accumulates option configurations per context, so the InMemory one must go too.
        services.RemoveAll<DbContextOptions<TContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<TContext>>();
        services.AddDbContext<TContext>(options => options
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies());
    }
}
