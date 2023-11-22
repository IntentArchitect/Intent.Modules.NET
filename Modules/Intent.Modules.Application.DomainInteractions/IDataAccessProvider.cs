using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using static Intent.Modules.Constants.TemplateFulfillingRoles.Domain;

namespace Intent.Modules.Application.DomainInteractions;

public interface IDataAccessProvider
{
    CSharpStatement SaveChangesAsync();
    CSharpStatement AddEntity(string entityName);
    CSharpStatement Update(string entityName);
    CSharpStatement Remove(string entityName);
    CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindAllAsync(string expression);
    CSharpStatement FindAllAsync(string expression, string pageNo, string pageSize);
    CSharpStatement FindAsync(string expression);
}

public record PrimaryKeyFilterMapping(CSharpStatement Value, CSharpStatement Property, IElementToElementMappedEnd Mapping);

public class RepositoryDataAccessProvider : IDataAccessProvider
{
    private readonly string _repositoryFieldName;

    public RepositoryDataAccessProvider(string repositoryFieldName)
    {
        _repositoryFieldName = repositoryFieldName;
    }

    public CSharpStatement SaveChangesAsync()
    {
        return $"{_repositoryFieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);";
    }

    public CSharpStatement AddEntity(string entityName)
    {
        return new CSharpInvocationStatement(_repositoryFieldName, "Add")
            .AddArgument(entityName);
    }

    public CSharpStatement Update(string entityName)
    {
        return new CSharpInvocationStatement(_repositoryFieldName, "Update")
            .AddArgument(entityName);
    }

    public CSharpStatement Remove(string entityName)
    {
        return new CSharpInvocationStatement(_repositoryFieldName, "Remove")
            .AddArgument(entityName);
    }

    public CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindByIdAsync")
            .AddArgument(pkMaps.Select(x => x.Value).AsSingleOrTuple())
            .AddArgument("cancellationToken");
    }

    public CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindByIdsAsync")
            .AddArgument($"{pkMaps.Select(x => x.Value).AsSingleOrTuple()}.ToArray()")
            .AddArgument("cancellationToken");
    }

    public CSharpStatement FindAsync(string expression)
    {
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAsync");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(string expression)
    {
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(string expression, string pageNo, string pageSize)
    {
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument($"pageNo: {pageNo}");
        invocation.AddArgument($"pageSize: {pageSize}");
        invocation.AddArgument("cancellationToken");
        return invocation;
    }
}

public class DbContextDataAccessProvider : IDataAccessProvider
{
    private readonly string _dbContextField;
    private readonly ICSharpTemplate _template;
    private readonly CSharpAccessMemberStatement _dbSetAccessor;
    private readonly CSharpProperty[] _pks;

    public DbContextDataAccessProvider(string dbContextField, ClassModel entity, ICSharpTemplate template)
    {
        _dbContextField = dbContextField;
        _template = template;
        _dbSetAccessor = new CSharpAccessMemberStatement(_dbContextField, entity.Name.ToPascalCase().Pluralize());
        var entityTemplate = _template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, entity);
        _pks = entityTemplate.CSharpFile.Classes.First().GetPropertiesWithPrimaryKey();
    }

    public CSharpStatement SaveChangesAsync()
    {
        return $"{_dbContextField}.SaveChangesAsync(cancellationToken);";
    }

    public CSharpStatement AddEntity(string entityName)
    {
        return new CSharpInvocationStatement(_dbSetAccessor, "Add")
            .AddArgument(entityName);
    }

    public CSharpStatement Update(string entityName)
    {
        return new CSharpInvocationStatement(_dbSetAccessor, "Update")
            .AddArgument(entityName);
    }

    public CSharpStatement Remove(string entityName)
    {
        return new CSharpInvocationStatement(_dbSetAccessor, "Remove")
            .AddArgument(entityName);
    }

    public CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"SingleOrDefaultAsync");
        if (pkMaps.Count == 1)
        {
            invocation.AddArgument($"x => x.{pkMaps[0].Property} == {pkMaps[0].Value}");
        }
        else
        {
            invocation.AddArgument($"x => {string.Join(" && ", pkMaps.Select(pkMap => $"x.{pkMap.Property} == {pkMap.Value}"))}");
        }

        return invocation.AddArgument("cancellationToken");
    }

    public CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var whereClause = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"Where");
        if (pkMaps.Count == 1)
        {
            whereClause.AddArgument($"x => {pkMaps[0].Value}.Contains(x.{pkMaps[0].Property})");
        }
        else
        {
            whereClause.AddArgument($"x => {string.Join(" && ", pkMaps.Select(pkMap => $"{pkMap.Value}.Contains(x.{pkMap.Property})"))}");
        }
        return new CSharpInvocationStatement(whereClause.WithoutSemicolon(), "ToListAsync").AddArgument("cancellationToken");
    }

    public CSharpStatement FindAsync(string expression)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"FirstOrDefaultAsync");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            invocation.AddArgument(expression);
        }
        invocation.AddArgument("cancellationToken");

        return invocation;
    }

    public CSharpStatement FindAllAsync(string expression)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpMethodChainStatement($"await {_dbSetAccessor}");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            invocation.AddChainStatement(new CSharpInvocationStatement("Where").AddArgument(expression).WithoutSemicolon());
        }

        invocation.AddChainStatement("ToListAsync(cancellationToken)");
        return invocation;
    }

    public CSharpStatement FindAllAsync(string expression, string pageNo, string pageSize)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"Where");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            invocation.AddArgument(expression);
        }

        return new CSharpMethodChainStatement(invocation.ToString())
            .AddChainStatement($"Skip({pageNo})")
            .AddChainStatement($"Take({pageNo})")
            .AddChainStatement($"ToListAsync(cancellationToken)");
    }
}

public static class CSharpClassExtensions
{
    public static CSharpClass GetRootEntity(this CSharpClass entity)
    {
        while (entity.BaseType != null && !entity.HasMetadata("primary-keys"))
        {
            entity = entity.BaseType;
        }

        return entity;
    }

    public static bool HasPrimaryKey(this CSharpClass rootEntity)
    {
        return rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks)
               && pks.Length > 0;
    }

    public static bool HasSinglePrimaryKey(this CSharpClass rootEntity)
    {
        return rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks)
               && pks.Length == 1;
    }

    public static CSharpProperty GetPropertyWithPrimaryKey(this CSharpClass rootEntity)
    {
        rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks);
        if (pks.Length == 0)
        {
            throw new InvalidOperationException($"Entity [{rootEntity.Name}] has no Primary Keys");
        }
        if (pks.Length > 1)
        {
            throw new InvalidOperationException($"Entity [{rootEntity.Name}] has more than one Primary Key");
        }
        return pks.First();
    }

    public static CSharpProperty[] GetPropertiesWithPrimaryKey(this CSharpClass rootEntity)
    {
        rootEntity.TryGetMetadata<CSharpProperty[]>("primary-keys", out var pks);
        return pks;
    }
}