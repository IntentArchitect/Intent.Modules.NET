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
    private readonly bool _isUsingProjections;
    private IQueryImplementation _queryImplementation;

    public RepositoryDataAccessProvider(string repositoryFieldName, ICSharpFileBuilderTemplate template, CSharpClassMappingManager mappingManager, bool hasUnitOfWork, QueryActionContext queryContext, ClassModel entity)
    {
        _hasUnitOfWork = hasUnitOfWork;
		_repositoryFieldName = repositoryFieldName;
        _template = template;
        _mappingManager = mappingManager;
        var entityTemplate = _template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, entity);
        _pks = entityTemplate.CSharpFile.Classes.First().GetPropertiesWithPrimaryKey();
        _entity = entity;
        if (_template.TryGetTypeName(TemplateRoles.Domain.Specification, _entity, out var specificationType))
        {
            _queryImplementation = new SpecificationImplementation(this, _repositoryFieldName, queryContext, specificationType);
        }
        else
        {
            _queryImplementation = new DefaultQueryImplementation(this, _repositoryFieldName, queryContext);
        }
    }

    public bool IsUsingProjections => _isUsingProjections;

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
        return _queryImplementation.FindByIdAsync(pkMaps);
    }

    public CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        return _queryImplementation.FindByIdsAsync(pkMaps);
    }

    public CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        return _queryImplementation.FindAsync(queryMapping, out prerequisiteStatements);
    }

    public CSharpStatement FindAsync(CSharpStatement? expression)
    {
        return _queryImplementation.FindAsync(expression);
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        return _queryImplementation.FindAllAsync(queryMapping, out prerequisiteStatements);
    }

    public CSharpStatement FindAllAsync(CSharpStatement? expression)
    {
        return _queryImplementation.FindAllAsync(expression);
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
    {
        return _queryImplementation.FindAllAsync(queryMapping, pageNo, pageSize, orderBy, orderByIsNullable, out prerequisiteStatements);
    }

    public CSharpStatement FindAllAsync(CSharpStatement? expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
    {
        return _queryImplementation.FindAllAsync(expression, pageNo, pageSize, orderBy, orderByIsNullable, out prerequisiteStatements);
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

    private interface IQueryImplementation
    {
        CSharpStatement FindAllAsync(CSharpStatement? expression);
        CSharpStatement FindAllAsync(CSharpStatement? expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindAsync(CSharpStatement? expression);
        CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps);
        CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps);
    }

    private class SpecificationImplementation : DefaultQueryImplementation
    {
        private string _specificationType;
        public SpecificationImplementation(RepositoryDataAccessProvider provider, string repositoryFieldName, QueryActionContext queryContext, string specificationType) 
            : base (provider, repositoryFieldName, queryContext)
        {  
            _specificationType = specificationType;
        }

        public override CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps)
        {

            return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FirstOrDefaultAsync")
                .AddArgument($"new {_specificationType}({pkMaps.Select(x => x.ValueExpression).AsSingleOrTuple()})")
                .AddArgument("cancellationToken");
        }


        public override CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
        {
            prerequisiteStatements = new List<CSharpStatement>();
            return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"ListAsync")
                .AddArgument($"new {_specificationType}()")
                .AddArgument("cancellationToken");
        }

        public override CSharpStatement FindAllAsync(CSharpStatement? expression)
        {
            return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"ListAsync")
                .AddArgument($"new {_specificationType}()")
                .AddArgument("cancellationToken");
        }

        public override CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
        {
            prerequisiteStatements = new List<CSharpStatement>();
            var result = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync")
                .AddArgument($"new {_specificationType}()")
                .AddArgument($"{pageNo}")
                .AddArgument($"{pageSize}");
            if (orderBy != null)
            {
                result.AddArgument($"queryOptions => queryOptions.OrderBy({Provider.GetOrderByValue(orderByIsNullable, orderBy)})");
            }
            result.AddArgument("cancellationToken");
            return result;
        }

        public override CSharpStatement FindAllAsync(CSharpStatement? expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
        {
            prerequisiteStatements = new List<CSharpStatement>();
            var result = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync")
                .AddArgument($"new {_specificationType}()")
                .AddArgument($"{pageNo}")
                .AddArgument($"{pageSize}");
            if (orderBy != null)
            {
                result.AddArgument($"queryOptions => queryOptions.OrderBy({Provider.GetOrderByValue(orderByIsNullable, orderBy)})");
            }
            result.AddArgument("cancellationToken");
            return result;
        }
    }

    private class DefaultQueryImplementation : IQueryImplementation
    {
        private readonly RepositoryDataAccessProvider _provider;
        protected readonly string _repositoryFieldName;
        protected readonly bool _isUsingProjections;
        protected readonly QueryActionContext? _queryContext;

        public DefaultQueryImplementation(RepositoryDataAccessProvider provider, string repositoryFieldName, QueryActionContext? queryContext)
        {
            _provider = provider;
            _repositoryFieldName = repositoryFieldName;
            _queryContext = queryContext;
            _isUsingProjections = queryContext?.ImplementWithProjections() == true;
        }

        protected RepositoryDataAccessProvider Provider => _provider;

        public virtual CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps)
        {
            return new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindByIdProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindByIdAsync")
                .AddArgument(pkMaps.Select(x => x.ValueExpression).AsSingleOrTuple())
                .AddArgument("cancellationToken");
        }

        public virtual CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
        {
            return new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindByIdsProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindByIdsAsync")
                .AddArgument($"{pkMaps.Select(x => x.ValueExpression).AsSingleOrTuple()}.ToArray()")
                .AddArgument("cancellationToken");
        }

        public virtual CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
        {
            var expression = _provider.CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
            var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindAsync");
            if (expression.Statement is not null)
            {
                invocation.AddArgument(expression.Statement);
            }

            invocation.AddArgument("cancellationToken");
            return invocation;
        }

        public virtual CSharpStatement FindAsync(CSharpStatement? expression)
        {
            var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindAsync");
            if (expression != null)
            {
                invocation.AddArgument(expression);
            }

            invocation.AddArgument("cancellationToken");
            return invocation;
        }

        public virtual CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
        {
            var expression = _provider.CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
            var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindAllProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindAllAsync");
            if (expression.Statement is not null)
            {
                invocation.AddArgument(expression.Statement);
            }

            invocation.AddArgument("cancellationToken");
            return invocation;
        }

        public virtual CSharpStatement FindAllAsync(CSharpStatement? expression)
        {
            var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindAllProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindAllAsync");
            if (expression != null)
            {
                invocation.AddArgument(expression);
            }

            invocation.AddArgument("cancellationToken");
            return invocation;
        }

        public virtual CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
        {
            var expressionResult = _provider.CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
            var expression = expressionResult.Statement;
            var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindAllProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindAllAsync");
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
                    expression = new CSharpStatement($"q => {expression.GetText("")}(q).OrderBy({_provider.GetOrderByValue(orderByIsNullable, orderBy)})");
                }
                // When passing in Func<IQueryable, IQueryable> (query option):
                invocation.AddArgument(expression);
            }
            else if (orderBy != null)
            {
                invocation.AddArgument($"queryOptions => queryOptions.OrderBy({_provider.GetOrderByValue(orderByIsNullable, orderBy)})");
            }
            invocation.AddArgument("cancellationToken");
            return invocation;
        }

        public virtual CSharpStatement FindAllAsync(CSharpStatement? expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
        {
            prerequisiteStatements = null;
            var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindAllProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindAllAsync");
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
                    expression = new CSharpStatement(expression.GetText("") + $".OrderBy({_provider.GetOrderByValue(orderByIsNullable, orderBy)})");
                }
                // pass in Func<IQueryable, IQueryable> (query option):
                invocation.AddArgument(expression);
            }
            else if (orderBy != null)
            {
                invocation.AddArgument($"queryOptions => queryOptions.OrderBy({_provider.GetOrderByValue(orderByIsNullable, orderBy)})");
            }

            invocation.AddArgument("cancellationToken");
            return invocation;
        }
    }

    private record FilterExpressionResult(bool UsesFilterMethod, CSharpStatement? Statement);

    private string? GetOrderByValue(bool orderByIsNullable, string? orderByField)
    {
        return orderByIsNullable ? $"{orderByField} ?? \"{_pks[0].Name}\"" : orderByField;
    }
}