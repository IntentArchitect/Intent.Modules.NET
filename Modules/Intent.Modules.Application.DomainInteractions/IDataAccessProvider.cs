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
using static Intent.Modules.Constants.TemplateRoles.Domain;
using System.Linq.Expressions;

namespace Intent.Modules.Application.DomainInteractions;

public interface IDataAccessProvider
{
    CSharpStatement SaveChangesAsync();
    CSharpStatement AddEntity(string entityName);
    CSharpStatement Update(string entityName);
    CSharpStatement Remove(string entityName);
    CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps);
    CSharpStatement FindAllAsync(CSharpStatement expression);
    CSharpStatement FindAllAsync(CSharpStatement expression, string pageNo, string pageSize);
    CSharpStatement FindAsync(CSharpStatement expression);
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

    public CSharpStatement FindAsync(CSharpStatement expression)
    {
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAsync");
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression)
    {
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression, string pageNo, string pageSize)
    {
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (expression?.ToString().StartsWith("x =>") == true) // a bit rudimentary
        {
            // pass in Expression<Func<TDomain, boolean>> (predicate):
            invocation.AddArgument(expression);
        }

        invocation.AddArgument($"{pageNo}");
        invocation.AddArgument($"{pageSize}");

        if (expression?.ToString().StartsWith("x =>") == false) // a bit rudimentary
        {
            // pass in Func<IQueryable, IQueryable> (query option):
            invocation.AddArgument(expression);
        }
        invocation.AddArgument("cancellationToken");
        return invocation;
    }
}

public class CompositeDataAccessProvider : IDataAccessProvider
{
    private readonly string _accessor;
    private readonly string _saveChangesAccessor;
    private readonly string? _explicitUpdateStatement;

    public CompositeDataAccessProvider(string saveChangesAccessor, string accessor, string? explicitUpdateStatement = null)
    {
        _explicitUpdateStatement = explicitUpdateStatement;
        _saveChangesAccessor = saveChangesAccessor;
        _accessor = accessor;
    }

    public bool RequiresExplicitUpdate()
    {
        return _explicitUpdateStatement != null;
    }

    public CSharpStatement SaveChangesAsync()
    {
        return $"{_saveChangesAccessor}.SaveChangesAsync(cancellationToken);";
    }

    public CSharpStatement AddEntity(string entityName)
    {
        if (_explicitUpdateStatement != null)
        {
            return new CSharpStatement($@"{_accessor}.Add({entityName});
        {_explicitUpdateStatement}");
        }
        return new CSharpInvocationStatement(_accessor, "Add")
            .AddArgument(entityName);
    }

    public CSharpStatement Update(string entityName)
    {
        return new CSharpStatement(_explicitUpdateStatement ?? "");
    }

    public CSharpStatement Remove(string entityName)
    {
        if (_explicitUpdateStatement != null)
        {
            return new CSharpStatement($@"{_accessor}.Remove({entityName});
        {_explicitUpdateStatement}");
        }
        return new CSharpInvocationStatement(_accessor, "Remove")
            .AddArgument(entityName);
    }

    public CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        var invocation = new CSharpInvocationStatement($"{_accessor}", $"SingleOrDefaultAsync");
        if (pkMaps.Count == 1)
        {
            invocation.AddArgument($"x => x.{pkMaps[0].Property} == {pkMaps[0].Value}");
        }
        else
        {
            invocation.AddArgument($"x => {string.Join(" && ", pkMaps.Select(pkMap => $"x.{pkMap.Property} == {pkMap.Value}"))}");
        }
        return invocation;
    }

    public CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        return new CSharpStatement("");
        //throw new Exception("Not Implemented");
    }

    public CSharpStatement FindAsync(CSharpStatement expression)
    {
        var invocation = new CSharpInvocationStatement($"{_accessor}", $"FirstOrDefault");
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }
        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression)
    {
        if (expression != null)
        {
            var invocation = new CSharpInvocationStatement($"{_accessor}", $"Where");
            {
                invocation.AddArgument(expression);
            }
            return invocation;
        }
        return new CSharpStatement($"{_accessor};");
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression, string pageNo, string pageSize)
    {
        return new CSharpStatement("");
        //throw new Exception("Not Implemented");
    }
}

internal record ElementToElementMappedEndStub : IElementToElementMappedEnd
{
    private readonly ICanBeReferencedType _sourceElement;
    private readonly ICanBeReferencedType _targetElement;
    public ElementToElementMappedEndStub(ICanBeReferencedType sourceElement, ICanBeReferencedType targetElement)
    {
        _sourceElement = sourceElement;
        _targetElement = targetElement;
    }

    public string MappingType => throw new NotImplementedException();

    public string MappingTypeId => throw new NotImplementedException();

    public string MappingExpression => throw new NotImplementedException();

    public IList<IElementMappingPathTarget> TargetPath => throw new NotImplementedException();

    public ICanBeReferencedType TargetElement => _targetElement;

    public IEnumerable<IElementToElementMappedEndSource> Sources => throw new NotImplementedException();

    public IList<IElementMappingPathTarget> SourcePath => throw new NotImplementedException();

    public ICanBeReferencedType SourceElement => _sourceElement;

    public IElementToElementMappedEndSource GetSource(string identifier)
    {
        throw new NotImplementedException();
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
        var entityTemplate = _template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, entity);
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

    public CSharpStatement FindAsync(CSharpStatement expression)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"FirstOrDefaultAsync");
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }
        invocation.AddArgument("cancellationToken");

        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpMethodChainStatement($"await {_dbSetAccessor}");
        if (expression != null)
        {
            invocation.AddChainStatement(new CSharpInvocationStatement("Where")
                .AddArgument(expression).WithoutSemicolon());
        }

        invocation.AddChainStatement("ToListAsync(cancellationToken)");
        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression, string pageNo, string pageSize)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"Where");
        if (expression != null)
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