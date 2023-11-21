using System.Runtime.InteropServices.ComTypes;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.DomainInteractions;

public interface IDataAccessProvider
{
    CSharpStatement SaveChangesAsync();
    CSharpStatement AddEntity(string entityName);
    CSharpStatement Update(string entityName);
    CSharpStatement Remove(string entityName);
    CSharpStatement FindByIdAsync(string id);
    CSharpStatement FindByIdsAsync(string ids);
    CSharpStatement FindAllAsync(string expression);
    CSharpStatement FindAllAsync(string expression, string pageNo, string pageSize);
    CSharpStatement FindAsync(string expression);
}

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

    public CSharpStatement FindByIdAsync(string id)
    {
        return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindByIdAsync")
            .AddArgument(id)
            .AddArgument("cancellationToken");
    }

    public CSharpStatement FindByIdsAsync(string ids)
    {
        return new CSharpInvocationStatement($"await {_repositoryFieldName}", $"FindByIdsAsync")
            .AddArgument($"{ids}.ToArray()")
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

    public DbContextDataAccessProvider(string dbContextField, ClassModel entity, ICSharpTemplate template)
    {
        _dbContextField = dbContextField;
        _template = template;
        _dbSetAccessor = new CSharpAccessMemberStatement(_dbContextField, entity.Name.ToPascalCase().Pluralize());
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

    public CSharpStatement FindByIdAsync(string id)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        return new CSharpInvocationStatement($"await {_dbSetAccessor}", $"SingleOrDefaultAsync")
            .AddArgument($"x => x.Id == {id}")
            .AddArgument("cancellationToken");
    }

    public CSharpStatement FindByIdsAsync(string ids)
    {
        _template.AddUsing("Microsoft.EntityFrameworkCore");
        return new CSharpInvocationStatement(new CSharpInvocationStatement($"await {_dbSetAccessor}", $"Where")
            .AddArgument($"x => {ids}.Contains(x)"), "ToListAsync(cancellationToken)");
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