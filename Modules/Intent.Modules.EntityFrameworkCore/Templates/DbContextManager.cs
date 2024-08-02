using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using PackageDbProvider = Intent.EntityFrameworkCore.Api.DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum;

#nullable enable

namespace Intent.Modules.EntityFrameworkCore.Templates;

public static class DbContextManager
{
    public static IList<DbContextInstance> GetDbContexts(string applicationId, IMetadataManager metadataManager)
    {
        return GetDbContexts(metadataManager.Domain(applicationId));
    }

    public static DbContextInstance GetDbContext(ClassModel classModel)
    {
        var pkg = classModel.InternalElement.Package.AsDomainPackageModel();
        if (pkg is null)
        {
            throw new Exception($"Class ({classModel.Id}, {classModel.Name}) is not found within a Domain Package");
        }

        return new DbContextInstance(pkg);
    }

    internal static DatabaseSettingsExtensions.DatabaseProviderOptionsEnum GetDatabaseProviderForDbContext(
        PackageDbProvider? packageDbProvider,
        ISoftwareFactoryExecutionContext executionContext)
    {
        return GetDatabaseProviderForDbContext(packageDbProvider, executionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum());
    }

    internal static DatabaseSettingsExtensions.DatabaseProviderOptionsEnum GetDatabaseProviderForDbContext(
        PackageDbProvider? packageDbProvider,
        DatabaseSettingsExtensions.DatabaseProviderOptionsEnum defaultDbProvider)
    {
        return packageDbProvider switch
        {
            null => defaultDbProvider,
            PackageDbProvider.Default => defaultDbProvider,
            PackageDbProvider.SQLServer => DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer,
            PackageDbProvider.PostgreSQL => DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql,
            PackageDbProvider.MySQL => DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql,
            PackageDbProvider.Oracle => DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Oracle,
            PackageDbProvider.InMemory => DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory,
            _ => throw new ArgumentOutOfRangeException($"DbProvider option '{packageDbProvider}' is not supported")
        };
    }

    private static IList<DbContextInstance> GetDbContexts(IDesigner domainDesigner)
    {
        var dbContextInstances = domainDesigner.GetDomainPackageModels()
            .Where(p => p.HasRelationalDatabase())
            .Select(pkg => new DbContextInstance(pkg))
            .Distinct()
            .ToArray();

        var dbContextInstanceWithSameConnStr = dbContextInstances
            .GroupBy(dbContext => dbContext.ConnectionStringName)
            .Where(group => group.Select(dbContext => dbContext.DbProvider).Distinct().Count() > 1)
            .SelectMany(group => group)
            .FirstOrDefault();
        if (dbContextInstanceWithSameConnStr is not null)
        {
            throw new ElementException(dbContextInstanceWithSameConnStr.DomainPackageModel.UnderlyingPackage,
                "Database Settings on this package shares the same Connection String Name with others but has a different Database Provider.");
        }

        return dbContextInstances;
    }
}

public class DbContextInstance : IMetadataModel
{
    private const string ApplicationDbContext = "ApplicationDbContext";
    private const string DefaultConnection = "DefaultConnection";
    
    public DbContextInstance(DomainPackageModel domainPackageModel)
    {
        var dbSettings = domainPackageModel.GetDatabaseSettings();
        
        var connectionStringInput = dbSettings?.ConnectionStringName();
        if (string.IsNullOrWhiteSpace(connectionStringInput))
        {
            connectionStringInput = DefaultConnection;
        }
        ConnectionStringName = connectionStringInput;
        
        Id = ConnectionStringName;
        DbProvider = dbSettings?.DatabaseProvider().AsEnum() ?? PackageDbProvider.Default;
        DomainPackageModel = domainPackageModel;
    }
    
    public string Id { get; }

    public string ConnectionStringName { get; }
    public PackageDbProvider DbProvider { get; }
    public DomainPackageModel DomainPackageModel { get; }

    public bool IsApplicationDbContext => DbContextName == ApplicationDbContext;
    
    private string? _dbContextName;

    public string DbContextName
    {
        get
        {
            if (_dbContextName is not null)
            {
                return _dbContextName;
            }

            if (ConnectionStringName == DefaultConnection)
            {
                _dbContextName = ApplicationDbContext;
                return _dbContextName;
            }

            _dbContextName = ConnectionStringName
                .Replace("ConnectionString", string.Empty)
                .Replace("Connection", string.Empty)
                .ToPascalCase()
                .RemoveSuffix("DbContext") + "DbContext";
            return _dbContextName;
        }
    }
    
    public string GetTypeName(IIntentTemplate template, string? defaultDbContextName = null)
    {
        return template.TryGetTypeName(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext, this, out var dbContextName) ? dbContextName : (defaultDbContextName ?? string.Empty);
    }

    protected bool Equals(DbContextInstance other)
    {
        return ConnectionStringName == other.ConnectionStringName && DbProvider == other.DbProvider;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DbContextInstance)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ConnectionStringName, DbProvider);
    }
}