using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.Application.DomainInteractions;

public class DbContextDataAccessProvider : IDataAccessProvider
{
    private readonly string _dbContextField;
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _mappingManager;
    private readonly CSharpAccessMemberStatement _dbSetAccessor;
    private readonly CSharpProperty[] _pks;

    public DbContextDataAccessProvider(string dbContextField, ClassModel entity, ICSharpFileBuilderTemplate template, CSharpClassMappingManager mappingManager)
    {
        _dbContextField = dbContextField;
        _template = template;
        _mappingManager = mappingManager;
        _dbSetAccessor = new CSharpAccessMemberStatement(_dbContextField, entity.Name.ToPascalCase().Pluralize());
        var entityTemplate = _template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, entity);
        _pks = entityTemplate.CSharpFile.Classes.First().GetPropertiesWithPrimaryKey();
    }

    public CSharpStatement SaveChangesAsync()
    {
        return $"await {_dbContextField}.SaveChangesAsync(cancellationToken);";
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
            invocation.AddArgument($"x => x.{pkMaps[0].Property} == {pkMaps[0].ValueExpression}");
        }
        else
        {
            invocation.AddArgument($"x => {string.Join(" && ", pkMaps.Select(pkMap => $"x.{pkMap.Property} == {pkMap.ValueExpression}"))}");
        }

        return invocation.AddArgument("cancellationToken");
    }

    public CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var whereClause = new CSharpInvocationStatement($"await {_dbSetAccessor}", Where());
        if (pkMaps.Count == 1)
        {
            whereClause.AddArgument($"x => {pkMaps[0].ValueExpression}.Contains(x.{pkMaps[0].Property})");
        }
        else
        {
            whereClause.AddArgument($"x => {string.Join(" && ", pkMaps.Select(pkMap => $"{pkMap.ValueExpression}.Contains(x.{pkMap.Property})"))}");
        }
        return new CSharpInvocationStatement(whereClause.WithoutSemicolon(), "ToListAsync").AddArgument("cancellationToken");
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpMethodChainStatement($"await {CreateQueryFilterExpression(queryMapping, out prerequisiteStatements)}");
        return invocation.AddChainStatement($"ToListAsync(cancellationToken)");
    }

    public CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        var expression = _mappingManager.GetPredicateExpression(queryMapping);
        if (expression == null)
        {
            throw new ElementException(queryMapping.HostElement, "No query fields have been mapped for this Query Entity Action, which signifies a single return value. Either specify this action returns a collection or map at least one field.");
        }
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"FirstOrDefaultAsync");
        invocation.AddArgument(expression);
        invocation.AddArgument("cancellationToken");
        return invocation;
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
            invocation.AddChainStatement(new CSharpInvocationStatement(Where())
                .AddArgument(expression).WithoutSemicolon());
        }

        invocation.AddChainStatement("ToListAsync(cancellationToken)");
        return invocation;
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
    {
        var invocation = new CSharpMethodChainStatement($"await {CreateQueryFilterExpression(queryMapping, out prerequisiteStatements)}");

        if (orderBy != null)
        {
            invocation.AddChainStatement(new CSharpInvocationStatement(_template.UseType($"System.Linq.Dynamic.Core.OrderBy"))
                .AddArgument(GetOrderByValue(orderByIsNullable, orderBy))
                .WithoutSemicolon());
        }

        return invocation.AddChainStatement($"ToPagedListAsync({pageNo}, {pageSize}, cancellationToken)");
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        var invocation = new CSharpMethodChainStatement($"await {_dbSetAccessor}");
        if (expression != null)
        {
            invocation.AddChainStatement(new CSharpInvocationStatement(Where())
                .AddArgument(expression)
                .WithoutSemicolon());
        }
        if (orderBy != null)
        {
            invocation.AddChainStatement(new CSharpInvocationStatement(_template.UseType($"System.Linq.Dynamic.Core.OrderBy"))
                .AddArgument(GetOrderByValue(orderByIsNullable, orderBy))
                .WithoutSemicolon());
        }
        return invocation.AddChainStatement($"ToPagedListAsync({pageNo}, {pageSize}, cancellationToken)");
    }

    private string GetOrderByValue(bool orderByIsNullable, string? orderByField)
    {
        return orderByIsNullable ? $"{orderByField} ?? \"{_pks[0].Name}\"" : orderByField;
    }

    private string Where()
    {
        return _template.UseType($"System.Linq.Where");
    }

    private CSharpStatement CreateQueryFilterExpression(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();

        var expression = _mappingManager.GetPredicateExpression(queryMapping);

        if (queryMapping.MappedEnds.All(x => !x.SourceElement.TypeReference.IsNullable))
        {
            var invocation = new CSharpMethodChainStatement($"{_dbSetAccessor}").WithoutSemicolon();
            if (expression != null)
            {
                invocation.AddChainStatement(new CSharpInvocationStatement(Where())
                    .AddArgument(expression).WithoutSemicolon());
            }

            return invocation;
        }

        var declareQueryable = new CSharpAssignmentStatement("var queryable", new CSharpInvocationStatement(_dbSetAccessor, "AsQueryable"));
        prerequisiteStatements.Add(declareQueryable);
        if (!string.IsNullOrWhiteSpace(expression))
        {
            prerequisiteStatements.Add($"queryable = queryable.{Where()}({expression});");
        }

        foreach (var mappedEnd in queryMapping.MappedEnds.Where(x => x.SourceElement.TypeReference.IsNullable))
        {
            prerequisiteStatements.Add(new CSharpIfStatement(_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd) + " != null")
                .AddStatement($"queryable = queryable.{Where()}(x => x.{mappedEnd.TargetElement.Name} == {_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd)})", x => x.WithSemicolon()));
        }

        return "queryable";
    }
}

public static class CSharpStatementMappingExtensions
{
    public static string GetPredicateExpression(this CSharpClassMappingManager mappingManager, IElementToElementMapping queryMapping)
    {
        var queryFields = queryMapping.MappedEnds.Where(x => x.SourceElement == null || !x.SourceElement.TypeReference.IsNullable)
            .Select(x => x.SourceElement == null || x.IsOneToOne()
                ? $"x.{x.TargetElement.Name} == {mappingManager.GenerateSourceStatementForMapping(queryMapping, x)}"
                : $"x.{x.TargetElement.Name}.{mappingManager.GenerateSourceStatementForMapping(queryMapping, x)}")
            .ToList();

        var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}" : null;
        return expression;
    }
}