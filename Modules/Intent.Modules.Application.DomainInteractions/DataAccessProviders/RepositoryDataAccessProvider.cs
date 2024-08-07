using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.DomainInteractions;

public class RepositoryDataAccessProvider : IDataAccessProvider
{
    private readonly string _repositoryFieldName;
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _mappingManager;
	private readonly bool _hasUnitOfWork;

	public RepositoryDataAccessProvider(string repositoryFieldName, ICSharpFileBuilderTemplate template, CSharpClassMappingManager mappingManager, bool hasUnitOfWork)
    {
        _hasUnitOfWork = hasUnitOfWork;
		_repositoryFieldName = repositoryFieldName;
        _template = template;
        _mappingManager = mappingManager;
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
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
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

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        var expression = CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
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

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, out IList<CSharpStatement> prerequisiteStatements)
    {
        var expression = CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
        var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync");
        if (expression?.ToString().StartsWith("x =>") == true) // a bit rudimentary
        {
            // When passing in Expression<Func<TDomain, boolean>> (predicate):
            invocation.AddArgument(expression);
        }

        invocation.AddArgument($"{pageNo}");
        invocation.AddArgument($"{pageSize}");

        if (expression?.ToString().StartsWith("x =>") == false) // a bit rudimentary
        {
            if (orderBy != null)
            {
                expression = new CSharpStatement(expression.GetText("") + $".OrderBy({orderBy})");
            }
            // When passing in Func<IQueryable, IQueryable> (query option):
            invocation.AddArgument(expression);
        }
        else if (orderBy != null) 
        {
            invocation.AddArgument($"queryOptions => queryOptions.OrderBy({orderBy})");
        }
        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression, string pageNo, string pageSize, string? orderBy, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
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
                expression = new CSharpStatement(expression.GetText("") + $".OrderBy({orderBy})");
            }
            // pass in Func<IQueryable, IQueryable> (query option):
            invocation.AddArgument(expression);
        }
        else if (orderBy != null)
        {
            invocation.AddArgument($"queryOptions => queryOptions.OrderBy({orderBy})");
        }

        invocation.AddArgument("cancellationToken");
        return invocation;
    }

    private CSharpStatement CreateQueryFilterExpression(IElementToElementMapping queryMapping, out IList<CSharpStatement> requiredStatements)
    {
        requiredStatements = new List<CSharpStatement>();

        var expression = _mappingManager.GetPredicateExpression(queryMapping);

        if (queryMapping.MappedEnds.All(x => x.SourceElement == null || !x.SourceElement.TypeReference.IsNullable))
        {
            return expression;
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

        return filterName;
    }
}