using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Application.DomainInteractions.Extensions;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

internal class RepositoryDataAccessProvider : IDataAccessProvider
{
    private readonly string _repositoryFieldName;
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _mappingManager;
    private readonly bool _hasUnitOfWork;
    private readonly CSharpProperty[] _pks;
    private readonly bool _isUsingProjections;
    private readonly IQueryImplementation _queryImplementation;

    public RepositoryDataAccessProvider(
        string repositoryFieldName,
        ICSharpFileBuilderTemplate template,
        CSharpClassMappingManager mappingManager,
        bool hasUnitOfWork,
        QueryActionContext? queryContext,
        ClassModel entity)
    {
        _hasUnitOfWork = hasUnitOfWork;
        _repositoryFieldName = repositoryFieldName;
        _template = template;
        _mappingManager = mappingManager;
        var entityTemplate = _template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, entity);
        _pks = entityTemplate.CSharpFile.Classes.First().GetPropertiesWithPrimaryKey();
        if (_template.TryGetTypeName(Specification, entity, out var specificationType))
        {
            _queryImplementation = new SpecificationImplementation(this, _repositoryFieldName, queryContext, specificationType);
        }
        else
        {
            _queryImplementation = new DefaultQueryImplementation(this, _repositoryFieldName, queryContext);
        }
        _isUsingProjections = queryContext?.ImplementWithProjections() == true;
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

    public IList<CSharpStatement> GetAggregateEntityRetrievalStatements(
        ICSharpClassMethodDeclaration method,
        IElementToElementMapping mapping,
        CSharpClassMappingManager csharpMapping)
    {
        List<CSharpStatement> statements = [];
        var lookupIdElementIds = new HashSet<string>
        {
            "5be4e1b7-855b-4ae6-a56e-6fc9d8ba0a87",
            "11201123-7476-4b32-9452-f4ccc96449ef",
            "48ded4be-abe8-4a2b-b9f6-ad9fb81067d8",
            "67ed3b19-a5eb-4b9a-a72a-d0a33b122fc0"
        };
        
        // Find all mapped ends that use Lookup IDs (target is the static "Lookup IDs" element)
        var lookupIdsMappings = mapping.MappedEnds
            .Where(x => x.TargetElement != null && lookupIdElementIds.Contains(x.TargetElement.Id))
            .ToList();

        foreach (var lookupMapping in lookupIdsMappings)
        {
            // The target path should lead to an association (e.g., Product -> Categories -> Lookup IDs)
            // We want the association element which is typically at TargetPath[TargetPath.Count - 2]
            var targetPath = lookupMapping.TargetPath?.ToList();
            if (targetPath == null || targetPath.Count < 2)
            {
                continue;
            }

            // Get the association from the target path (second-to-last element)
            var associationPathElement = targetPath[targetPath.Count - 2];

            // The Element property contains the association end which has the TypeReference
            var associationElement = associationPathElement.Element;
            if (associationElement == null)
            {
                continue;
            }

            // Get the entity type from the association's type reference (e.g., Category)
            var targetEntityType = associationElement.TypeReference?.Element.AsClassModel();
            if (targetEntityType == null || !targetEntityType.IsAggregateRoot())
            {
                // Only support aggregate roots for now
                continue;
            }

            // Get the repository for the target entity type (Category, not Product)
            if (!_template.TryGetTypeName(TemplateRoles.Repository.Interface.Entity, targetEntityType, out var targetRepositoryInterface))
            {
                // No repository for this entity type, skip it
                continue;
            }

            // Inject the target entity's repository
            var targetRepositoryFieldName = method.Class.InjectService(targetRepositoryInterface);

            // Generate a user-friendly variable name: existing{EntityName}s (e.g., existingCategories)
            var entityName = targetEntityType.Name;
            var variableName = $"existing{entityName.Pluralize()}";

            // Generate the source expression for the IDs (e.g., request.CategoryIds)
            var sourceIdsExpression = csharpMapping.GenerateSourceStatementForMapping(mapping, lookupMapping);

            // Generate the repository lookup statement using the TARGET entity's repository
            _template.AddUsing("System.Linq");
            var lookupStatement = new CSharpInvocationStatement($"await {targetRepositoryFieldName}", "FindByIdsAsync")
                .AddArgument($"{sourceIdsExpression}.ToArray()")
                .AddArgument("cancellationToken");

            statements.Add(new CSharpAssignmentStatement($"var {variableName}", lookupStatement).WithSemicolon());

            // Set up mapping replacements so the object initializer uses this variable
            // Replace the static metadata ID
            foreach (var id in lookupIdElementIds)
            {
                csharpMapping.SetFromReplacement(new StaticMetadata(id), variableName);
            }
        }

        return statements;
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

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageSize, string? cursorToken, out IList<CSharpStatement> prerequisiteStatements)
    {
        return _queryImplementation.FindAllAsync(queryMapping, pageSize, cursorToken, out prerequisiteStatements);
    }

    private FilterExpressionResult CreateQueryFilterExpression(IElementToElementMapping queryMapping, out IList<CSharpStatement> requiredStatements)
    {
        requiredStatements = new List<CSharpStatement>();

        var expression = _mappingManager.GetPredicateExpression(queryMapping);

        if (queryMapping.MappedEnds.All(x => x.SourceElement == null || !x.SourceElement.TypeReference.IsNullable))
        {
            return new FilterExpressionResult(false, expression);
        }

        var elementId = queryMapping.TargetElement.Id;
        string typeName;
        if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Repository.Interface.Entity, elementId, out var repositoryInterfaceTemplate) &&
            repositoryInterfaceTemplate.CSharpFile.TryGetMetadata<string>("entity-state-template-id", out var entityStateTemplateId))
        {
            typeName = _template.GetTypeName(entityStateTemplateId, elementId);
        }
        else
        {
            typeName = _template.GetTypeName((IElement)queryMapping.TargetElement);
        }

        if (!ProviderSupportsQueryable(queryMapping))
        {
            return GenerateExpressionFilter(queryMapping, requiredStatements, expression, typeName);
        }

        return GenerateQueryableFilter(queryMapping, requiredStatements, expression, typeName);
    }

    private FilterExpressionResult GenerateQueryableFilter(IElementToElementMapping queryMapping, IList<CSharpStatement> requiredStatements, string expression, string typeName)
    {
        var filterName = $"Filter{queryMapping.TargetElement.Name.ToPascalCase().Pluralize()}";
        var block = new CSharpLocalMethod($"{_template.UseType("System.Linq.IQueryable")}<{typeName}>", filterName, _template.CSharpFile);
        block.AddParameter($"{_template.UseType("System.Linq.IQueryable")}<{typeName}>", "queryable");
        if (!string.IsNullOrWhiteSpace(expression))
        {
            block.AddStatement($"queryable = queryable.Where({expression})", x => x.WithSemicolon());
        }

        foreach (var mappedEnd in queryMapping.MappedEnds.Where(x => x.SourceElement != null && x.SourceElement.TypeReference.IsNullable))
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

    private FilterExpressionResult GenerateExpressionFilter(IElementToElementMapping queryMapping, IList<CSharpStatement> requiredStatements, string expression, string typeName)
    {
        var filterName = $"filter{queryMapping.TargetElement.Name.ToPascalCase().Pluralize()}";
        _template.AddUsing("System.Linq.Expressions");

        requiredStatements.Add(new CSharpObjectInitStatement($"Expression<Func<{typeName}, bool>> {filterName}", "entity => true;"));

        foreach (var mappedEnd in queryMapping.MappedEnds.Where(x => x.SourceElement != null && !x.SourceElement.TypeReference.IsNullable))
        {
            var assignement = new CSharpAssignmentStatement(filterName, new CSharpStatement($"{filterName}.Combine(entity => entity.{mappedEnd.TargetElement.Name} == {_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd)});"));
            requiredStatements.Add(assignement);
        }

        foreach (var mappedEnd in queryMapping.MappedEnds.Where(x => x.SourceElement != null && x.SourceElement.TypeReference.IsNullable))
        {
            var ifBlock = new CSharpIfStatement(_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd) + " != null");
            ifBlock.AddAssignmentStatement("var requestField", _mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd).WithSemicolon());
            ifBlock.AddAssignmentStatement(filterName, new CSharpStatement($"{filterName}.Combine(entity => entity.{mappedEnd.TargetElement.Name} == requestField);"));

            requiredStatements.Add(ifBlock);
        }

        return new FilterExpressionResult(true, filterName);
    }

    private bool ProviderSupportsQueryable(IElementToElementMapping queryMapping)
    {
        // JPS - this entire method is a temporary hack as there is no way to currently lookup
        // the metadata required to make the decision.

        var targetElement = queryMapping.TargetElement;
        var package = targetElement?.Package;

        // Document DB provider stereotypeId
        if (!package.HasStereotype("8b68020c-6652-484b-85e8-6c33e1d8031f"))
        {
            return true;
        }

        var docDbProvider = package.GetStereotype("8b68020c-6652-484b-85e8-6c33e1d8031f");

        // if the provider is not selected, it means only one document db provider installed
        // check if its Table Storage
        // OR Table Storage is explicitly selected
        if ((string.IsNullOrWhiteSpace(docDbProvider.GetProperty("Provider")?.Value)
            && _template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.Azure.TableStorage")) ||
            docDbProvider.GetProperty("Provider")?.Value == "1d05ee8e-747f-4120-9647-29ac784ef633")
        {
            return false;
        }

        return true;
    }

    private interface IQueryImplementation
    {
        CSharpStatement FindAllAsync(CSharpStatement? expression);
        CSharpStatement FindAllAsync(CSharpStatement? expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageSize, string? cursorToken, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindAsync(CSharpStatement? expression);
        CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements);
        CSharpStatement FindByIdAsync(List<PrimaryKeyFilterMapping> pkMaps);
        CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps);
    }

    private class SpecificationImplementation : DefaultQueryImplementation
    {
        private string _specificationType;
        public SpecificationImplementation(RepositoryDataAccessProvider provider, string repositoryFieldName, QueryActionContext queryContext, string specificationType)
            : base(provider, repositoryFieldName, queryContext)
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

        public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageSize, string? cursorToken, out IList<CSharpStatement> prerequisiteStatements)
        {
            prerequisiteStatements = new List<CSharpStatement>();
            var result = new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindAllAsync")
                .AddArgument($"new {_specificationType}()")
                .AddArgument($"{pageSize}")
                .AddArgument($"{cursorToken}");

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

        public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageSize, string? cursorToken, out IList<CSharpStatement> prerequisiteStatements)
        {
            var expressionResult = _provider.CreateQueryFilterExpression(queryMapping, out prerequisiteStatements);
            var expression = expressionResult.Statement;
            var invocation = new CSharpInvocationStatement($"await {_repositoryFieldName}", _isUsingProjections ? $"FindAllProjectToAsync<{_queryContext!.GetDtoProjectionReturnType()}>" : $"FindAllAsync");
            if (expression is not null && !expressionResult.UsesFilterMethod)
            {
                // When passing in Expression<Func<TDomain, boolean>> (predicate):
                invocation.AddArgument(expressionResult.Statement);
            }

            if (expression is not null && expressionResult.UsesFilterMethod)
            {
                //if (orderBy != null)
                //{
                //    expression = new CSharpStatement($"q => {expression.GetText("")}(q).OrderBy({_provider.GetOrderByValue(orderByIsNullable, orderBy)})");
                //}
                // When passing in Func<IQueryable, IQueryable> (query option):
                invocation.AddArgument(expression);
            }
            invocation.AddArgument($"{pageSize}");
            invocation.AddArgument($"{cursorToken}");
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