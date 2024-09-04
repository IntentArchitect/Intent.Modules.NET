using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Application.DomainInteractions;

public class CompositeDataAccessProvider : IDataAccessProvider
{
    private readonly string _accessor;
    private readonly string _saveChangesAccessor;
    private readonly string? _explicitUpdateStatement;
    private readonly ICSharpFileBuilderTemplate _template;
    private readonly CSharpClassMappingManager _mappingManager;

    public CompositeDataAccessProvider(string saveChangesAccessor, string accessor, string? explicitUpdateStatement,
        ICSharpFileBuilderTemplate template, CSharpClassMappingManager mappingManager)
    {
        _explicitUpdateStatement = explicitUpdateStatement;
        _template = template;
        _mappingManager = mappingManager;
        _saveChangesAccessor = saveChangesAccessor;
        _accessor = accessor;
    }

    public bool RequiresExplicitUpdate()
    {
        return _explicitUpdateStatement != null;
    }

    public CSharpStatement SaveChangesAsync()
    {
        return $"await {_saveChangesAccessor}.SaveChangesAsync(cancellationToken);";
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
            invocation.AddArgument($"x => x.{pkMaps[0].Property} == {pkMaps[0].ValueExpression}");
        }
        else
        {
            invocation.AddArgument($"x => {string.Join(" && ", pkMaps.Select(pkMap => $"x.{pkMap.Property} == {pkMap.ValueExpression}"))}");
        }
        return invocation;
    }

    public CSharpStatement FindByIdsAsync(List<PrimaryKeyFilterMapping> pkMaps)
    {
        return new CSharpStatement("");
        //throw new Exception("Not Implemented");
    }

    public CSharpStatement FindAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        var expression = _mappingManager.GetPredicateExpression(queryMapping);

        var invocation = new CSharpInvocationStatement($"{_accessor}", $"FirstOrDefault");
        if (expression != null)
        {
            invocation.AddArgument(expression);
        }
        return invocation;
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

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        var expression = _mappingManager.GetPredicateExpression(queryMapping);
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

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageNo, string pageSize, string? orderBy, bool orderByIsNUllable, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        return new CSharpStatement("");
    }

    public CSharpStatement FindAllAsync(CSharpStatement expression, string pageNo, string pageSize, string? orderBy, bool orderByIsNUllable, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        return new CSharpStatement("");
        //throw new Exception("Not Implemented");
    }

    // private CSharpStatement CreateQueryFilterExpression(IElementToElementMapping queryMapping, out IList<CSharpStatement> requiredStatements)
    // {
    //     requiredStatements = new List<CSharpStatement>();
    //
    //     var queryFields = queryMapping.MappedEnds.Where(x => !x.SourceElement.TypeReference.IsNullable)
    //         .Select(x => x.IsOneToOne()
    //             ? $"x.{x.TargetElement.Name} == {_mappingManager.GenerateSourceStatementForMapping(queryMapping, x)}"
    //             : $"x.{x.TargetElement.Name}.{_mappingManager.GenerateSourceStatementForMapping(queryMapping, x)}")
    //         .ToList();
    //
    //     var expression = queryFields.Any() ? $"x => {string.Join(" && ", queryFields)}" : null;
    //
    //     if (queryMapping.MappedEnds.All(x => !x.SourceElement.TypeReference.IsNullable))
    //     {
    //         return expression;
    //     }
    //
    //     var typeName = _template.GetTypeName((IElement)queryMapping.TargetElement);
    //     var filterName = $"Filter{typeName.Pluralize()}";
    //     var block = new CSharpLocalMethod($"{_template.UseType("System.Linq.IQueryable")}<{typeName}>", filterName, _template.CSharpFile);
    //     block.AddParameter($"{_template.UseType("System.Linq.IQueryable")}<{typeName}>", "queryable");
    //     if (!string.IsNullOrWhiteSpace(expression))
    //     {
    //         block.AddStatement($"queryable = queryable.Where({expression})", x => x.WithSemicolon());
    //     }
    //
    //     foreach (var mappedEnd in queryMapping.MappedEnds.Where(x => x.SourceElement.TypeReference.IsNullable))
    //     {
    //         block.AddIfStatement(_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd) + " != null", inside =>
    //         {
    //             inside.AddStatement($"queryable = queryable.Where(x => x.{mappedEnd.TargetElement.Name} == {_mappingManager.GenerateSourceStatementForMapping(queryMapping, mappedEnd)})", x => x.WithSemicolon());
    //         });
    //     }
    //
    //     block.AddStatement("return await queryable;");
    //     block.SeparatedFromNext();
    //     requiredStatements.Add(block);
    //
    //     return filterName;
    // }
}