using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.DomainInteractions;

#nullable enable

public class RepositoryDataAccessProvider : IDataAccessProvider
{
    private readonly string _repositoryFieldName;
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _mappingManager;
	private readonly bool _hasUnitOfWork;
    private readonly CSharpProperty[] _pks;
    private readonly ClassModel _entity;

    public RepositoryDataAccessProvider(string repositoryFieldName, ICSharpFileBuilderTemplate template, CSharpClassMappingManager mappingManager, bool hasUnitOfWork, ClassModel entity)
    {
        _hasUnitOfWork = hasUnitOfWork;
		_repositoryFieldName = repositoryFieldName;
        _template = template;
        _mappingManager = mappingManager;
        var entityTemplate = _template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, entity);
        _pks = entityTemplate.CSharpFile.Classes.First().GetPropertiesWithPrimaryKey();
        _entity = entity;
    }

    public CSharpStatement SaveChangesAsync()
    {
        if (_hasUnitOfWork)
        {
			return $"await {_repositoryFieldName}.UnitOfWork.SaveChangesAsync(cancellationToken);";
		}
        else
        {
            return "";
        }
	}

    public CSharpStatement AddEntity(string entityName)
    {
        if (_hasUnitOfWork)
        {
            return new CSharpInvocationStatement(_repositoryFieldName, "Add")
                .AddArgument(entityName);
        }
        else
        {
			return new CSharpInvocationStatement($"await {_repositoryFieldName}", "AddAsync")
				.AddArgument(entityName);
		}
	}

    public CSharpStatement Update(string entityName)
    {
        if (_hasUnitOfWork)
        { 
            return new CSharpInvocationStatement(_repositoryFieldName, "Update")
                .AddArgument(entityName);
		}
		else
		{
			return new CSharpInvocationStatement($"await {_repositoryFieldName}", "UpdateAsync")
				.AddArgument(entityName);
		}
	}

	public CSharpStatement Remove(string entityName)
    {
		if (_hasUnitOfWork)
		{
			return new CSharpInvocationStatement(_repositoryFieldName, "Remove")
				.AddArgument(entityName);
		}
		else
		{
			return new CSharpInvocationStatement($"await {_repositoryFieldName}", "RemoveAsync")
				.AddArgument(entityName);
		}
	}

	public CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {

        if (_template.TryGetTypeName(TemplateRoles.Domain.Specification, _entity, out var specificationType))
        {
            return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FirstOrDefaultAsync")
                .AddArgument($"new {specificationType}({pkMaps.Select(x => x.ValueExpression).AsSingleOrTuple()})" )
                .AddArgument("cancellationToken");
        }
        return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindByIdAsync")
            .AddArgument(pkMaps.Select(x => x.ValueExpression).AsSingleOrTuple())
            .AddArgument("cancellationToken");
    }

    public CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindByIdsAsync")
            .AddArgument($"{pkMaps.Select(x => x.ValueExpression).AsSingleOrTuple()}.ToArray()")
            .AddArgument("cancellationToken");
    }

    public CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        var expression = CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAsync");
        if (expression.Statement is not null)
        {
            invocation.AddArgument(expression.Statement);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAsync(CSharpStatement? expression)
    {
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAsync");
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        if (_template.TryGetTypeName(TemplateRoles.Domain.Specification, _entity, out var specificationType))
        {
            prerequisiteStatements = new List<CSharpStatement>();
            return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"ListAsync")
                .AddArgument($"new {specificationType}()")
                .AddArgument("cancellationToken");
        }

        var expression = CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (expression.Statement is not null)
        {
            invocation.AddArgument(expression.Statement);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement? expression)
    {
        if (_template.TryGetTypeName(TemplateRoles.Domain.Specification, _entity, out var specificationType))
        {
            return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"ListAsync")
                .AddArgument($"new {specificationType}()")
                .AddArgument("cancellationToken");
        }

        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
    {
        if (_template.TryGetTypeName(TemplateRoles.Domain.Specification, _entity, out var specificationType))
        {
            prerequisiteStatements = new List<CSharpStatement>();
            var result = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync")
                .AddArgument($"new {specificationType}()")
                .AddArgument($"{pageNo}")
                .AddArgument($"{pageSize}");
            if (orderBy != null)
            {
                result.AddArgument($"queryOptions => queryOptions.OrderBy({GetOrderByValue(orderByIsNullable, orderBy)})");
            }
            result.AddArgument("cancellationToken");
            return result;
        }

        var expressionResult = CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
        var expression = expressionResult.Statement;
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (expression is not null && !expressionResult.UsesFilterMethod)
        {
            // When passing in Expression<Func<TDomain, boolean>> (predicate):
            invocation.AddArgument(expressionResult.Statement);
        }

        invocation.AddArgument($"{pageNo}");
        invocation.AddArgument($"{pageSize}");

        if (expression is not null && expressionResult.UsesFilterMethod)
        {
            if (orderBy != null)
            {
                expression = new CSharpStatement($"q => {expression.GetText("")}(q).OrderBy({GetOrderByValue(orderByIsNullable, orderBy)})");
            }
            // When passing in Func<IQueryable, IQueryable> (query option):
            invocation.AddArgument(expression);
        }
        else if (orderBy != null) 
        {
            invocation.AddArgument($"queryOptions => queryOptions.OrderBy({GetOrderByValue(orderByIsNullable, orderBy)})");
        }
        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement? expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        if (_template.TryGetTypeName(TemplateRoles.Domain.Specification, _entity, out var specificationType))
        {
            var result =  new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync")
                .AddArgument($"new {specificationType}()")
                .AddArgument($"{pageNo}")
                .AddArgument($"{pageSize}");
            if (orderBy != null)
            {
                result.AddArgument($"queryOptions => queryOptions.OrderBy({GetOrderByValue(orderByIsNullable, orderBy)})");
            }
            result.AddArgument("cancellationToken");
            return result;
        }

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
            if (orderBy != null)
            {
                expression = new CSharpStatement(expression.GetText("") + $".OrderBy({GetOrderByValue(orderByIsNullable, orderBy)})");
            }
            // pass in Func<IQueryable, IQueryable> (query option):
            invocation.AddArgument(expression);
        }
        else if (orderBy != null)
        {
            invocation.AddArgument($"queryOptions => queryOptions.OrderBy({GetOrderByValue(orderByIsNullable, orderBy)})");
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    private FilterExpressionResult CreateQueryFilterExpression(IElementToElementMapping queryMapping, out IList<CSharpStatement> requiredStatements)
    {
        requiredStatements = new List<CSharpStatement>();

        var expression = _mappingManager.GetPredicateExpression(queryMapping);

        if (queryMapping.MappedEnds.All(x => x.SourceElement == null || !x.SourceElement.TypeReference.IsNullable))
        {
            return new FilterExpressionResult(false, expression);
        }

        var typeName = _template.GetTypeName((IElement)queryMapping.TargetElement);
        var filterName = $"Filter{typeName.Pluralize()}";
        var block = new CSharpLocalMethod($"{_template.UseType("System.Linq.IQueryable")}<{typeName}>", filterName, _template.CSharpFile);
        block.AddParameter($"{_template.UseType("System.Linq.IQueryable")}<{typeName}>", "queryable");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            block.AddStatement($"queryable = queryable.Where({expression})", x => x.WithSemicolon());
        }

        foreach (var mappedEnd in queryMapping.MappedEnds.Where(x => x.SourceElement == null || x.SourceElement.TypeReference.IsNullable))
        {
            block.AddIfStatement(_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd) + " != null", inside =>
            {
                inside.AddStatement($"queryable = queryable.Where(x => x.{mappedEnd.TargetElement.Name} == {_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd)})", x => x.WithSemicolon());
            });
        }

        block.AddStatement("return queryable;");
        block.SeparatedFromNext();
        requiredStatements.Add(block);

        return new FilterExpressionResult(true, filterName);
    }

    private record FilterExpressionResult(bool UsesFilterMethod, CSharpStatement? Statement);

    private string? GetOrderByValue(bool orderByIsNullable, string? orderByField)
    {
        return orderByIsNullable ? $"{orderByField} ?? \"{_pks[0].Name}\"" : orderByField;
    }
}