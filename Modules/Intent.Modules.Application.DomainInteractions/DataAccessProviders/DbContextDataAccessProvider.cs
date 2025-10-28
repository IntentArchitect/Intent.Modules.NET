using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Intent.Modules.Constants.TemplateRoles.Domain;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

internal class DbContextDataAccessProvider : IDataAccessProvider
{
    private readonly string _dbContextField;
    private readonly ICSharpTemplate _template;
    private readonly CSharpClassMappingManager _mappingManager;
    private readonly CSharpAccessMemberStatement _dbSetAccessor;
    private readonly CSharpProperty[] _pks;
    private readonly bool _isUsingProjections;
    private readonly QueryActionContext? _queryContext;

    public DbContextDataAccessProvider(
        string dbContextField,
        ClassModel entity,
        ICSharpTemplate template,
        CSharpClassMappingManager mappingManager,
        QueryActionContext? queryContext)
    {
        _dbContextField = dbContextField;
        _template = template;
        _mappingManager = mappingManager;

        _dbSetAccessor = new CSharpAccessMemberStatement(_dbContextField, GetDbSetName(entity));

        var entityTemplate = _template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, entity);
        _pks = entityTemplate.CSharpFile.Classes.First().GetPropertiesWithPrimaryKey();
        _isUsingProjections = queryContext?.ImplementWithProjections() == true;
        _queryContext = queryContext;
    }

    public bool IsUsingProjections => _isUsingProjections;

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
        if (_isUsingProjections)
        {
            var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", Where())
                .AddArgument(GetPkFilterEquals(pkMaps));
            var projectTo = AddProjectTo(invocation);
            return new CSharpInvocationStatement(projectTo.WithoutSemicolon(), "SingleOrDefaultAsync").AddArgument("cancellationToken");
        }
        else
        {
            return new CSharpInvocationStatement($"await {_dbSetAccessor}", $"SingleOrDefaultAsync")
                .AddArgument(GetPkFilterEquals(pkMaps))
                .AddArgument("cancellationToken");
        }
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
        if (_isUsingProjections)
        {
            whereClause = AddProjectTo(whereClause);
        }
        return new CSharpInvocationStatement(whereClause.WithoutSemicolon(), "ToListAsync").AddArgument("cancellationToken");
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        var invocation = new CSharpMethodChainStatement($"await {CreateQueryFilterExpression(queryMapping, out prerequisiteStatements)}");
        if (_isUsingProjections)
        {
            AddProjectTo(invocation);
        }
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
        if (_isUsingProjections)
        {
            var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", Where())
                .AddArgument(expression);
            invocation = AddProjectTo(invocation);
            return new CSharpInvocationStatement(invocation.WithoutSemicolon(), "FirstOrDefaultAsync").AddArgument("cancellationToken");
        }
        else
        {
            return new CSharpInvocationStatement($"await {_dbSetAccessor}", $"FirstOrDefaultAsync")
                .AddArgument(expression)
                .AddArgument("cancellationToken");
        }
    }

    public CSharpStatement FindAsync(CSharpStatement expression)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        if (_isUsingProjections)
        {
            CSharpInvocationStatement invocation;
            if (expression is not null)
            {
                invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", Where())
                    .AddArgument(expression);
                invocation = AddProjectTo(invocation);
            }
            else
            {
                invocation = AddProjectTo($"await {_dbSetAccessor}");
            }

            return new CSharpInvocationStatement(invocation.WithoutSemicolon(), "FirstOrDefaultAsync").AddArgument("cancellationToken");
        }
        else
        {
            var invocation = new CSharpInvocationStatement($"await {_dbSetAccessor}", $"FirstOrDefaultAsync");

            if (expression != null)
            {
                invocation.AddArgument(expression);
            }

            return invocation.AddArgument("cancellationToken");
        }
    }

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNullable, out IList<CSharpStatement> prerequisiteStatements)
    {
        var invocation = new CSharpMethodChainStatement($"await {CreateQueryFilterExpression(queryMapping, out prerequisiteStatements)}");

        if (_isUsingProjections)
        {
            AddProjectTo(invocation);
        }

        if (orderBy != null)
        {
            invocation.AddChainStatement(new CSharpInvocationStatement(AddOrderBy())
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
        if (_isUsingProjections)
        {
            AddProjectTo(invocation);
        }
        if (orderBy != null)
        {
            invocation.AddChainStatement(new CSharpInvocationStatement(AddOrderBy())
                .AddArgument(GetOrderByValue(orderByIsNullable, orderBy))
                .WithoutSemicolon());
        }
        return invocation.AddChainStatement($"ToPagedListAsync({pageNo}, {pageSize}, cancellationToken)");
    }

    private string AddOrderBy()
    {
        _template.AddUsing("static System.Linq.Dynamic.Core.DynamicQueryableExtensions");
        return "OrderBy";
    }

    private string GetPkFilterEquals(List<PrimaryKeyFilterMapping> pkMaps)
    {
        if (pkMaps.Count == 1)
        {
            return $"x => x.{pkMaps[0].Property} == {pkMaps[0].ValueExpression}";
        }
        else
        {
            return $"x => {string.Join(" && ", pkMaps.Select(pkMap => $"x.{pkMap.Property} == {pkMap.ValueExpression}"))}";
        }
    }

    private string GetOrderByValue(bool orderByIsNullable, string? orderByField)
    {
        return orderByIsNullable ? $"{orderByField} ?? \"{_pks[0].Name}\"" : orderByField;
    }

    private CSharpInvocationStatement AddProjectTo(CSharpStatement statement)
    {
        _template.AddUsing("AutoMapper.QueryableExtensions");
        return new CSharpInvocationStatement(statement, $"ProjectTo<{_queryContext!.GetDtoProjectionReturnType()}>").AddArgument("_mapper.ConfigurationProvider");
    }

    private CSharpInvocationStatement AddProjectTo(CSharpInvocationStatement statement)
    {
        return AddProjectTo((CSharpStatement)statement.WithoutSemicolon());
    }

    private void AddProjectTo(CSharpMethodChainStatement chain)
    {
        _template.AddUsing("AutoMapper.QueryableExtensions");
        chain.AddChainStatement($"ProjectTo<{_queryContext!.GetDtoProjectionReturnType()}>(_mapper.ConfigurationProvider)");
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

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageSize, string? cursorToken, out IList<CSharpStatement> prerequisiteStatements)
    {
        // for now cursor based is not implements with EF
        prerequisiteStatements = new List<CSharpStatement>();
        return new CSharpStatement("");
    }

    public string GetDbSetName(ClassModel model)
    {
        if (_template.ExecutionContext.Settings.GetSetting("ac0a788e-d8b3-4eea-b56d-538608f1ded9", "6010e890-6e2d-4812-9969-ffbdb8f93d87")?.Value == "same-as-entity")
        {
            return model.Name.ToPascalCase();
        }

        return model.Name.ToPascalCase().Pluralize();
    }
}