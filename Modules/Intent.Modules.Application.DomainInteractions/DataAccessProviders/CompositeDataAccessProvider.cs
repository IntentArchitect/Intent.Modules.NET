using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Interactions;
using Intent.Modules.Common.CSharp.Mapping;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

public class CompositeDataAccessProvider : IDataAccessProvider
{
    private readonly string _accessor;
    private readonly string _saveChangesAccessor;
    private readonly string? _explicitUpdateStatement;
    private readonly CSharpClassMappingManager _mappingManager;

    public CompositeDataAccessProvider(string saveChangesAccessor, string accessor, string? explicitUpdateStatement,
        ICSharpClassMethodDeclaration method)
    {
        _explicitUpdateStatement = explicitUpdateStatement;
        _mappingManager = method.GetMappingManager();
        _saveChangesAccessor = saveChangesAccessor;
        _accessor = accessor;
    }

    public bool IsUsingProjections => false;
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

    public CSharpStatement FindAllAsync(IElementToElementMapping queryMapping, string pageSize, string? cursorToken, out IList<CSharpStatement> prerequisiteStatements)
    {
        prerequisiteStatements = new List<CSharpStatement>();
        return new CSharpStatement("");
    }
}