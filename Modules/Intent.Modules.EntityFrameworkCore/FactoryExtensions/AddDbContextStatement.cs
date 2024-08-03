using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions;

public class AddDbContextStatement : CSharpStatement, IHasCSharpStatements
{

    public AddDbContextStatement(string dbContextName) : base($@"services.AddDbContext<{dbContextName}>((sp, options) =>")
    {
        DbContextName = dbContextName;
    }
    
    public string DbContextName { get; }
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public AddDbContextStatement AddConfigOptionStatement(CSharpStatement statement)
    {
        Statements.Add(statement);
        statement.Parent = this;
        return this;
    }

    public AddDbContextStatement AddConfigOptionStatements(IEnumerable<CSharpStatement> statements)
    {
        foreach (var statement in statements)
        {
            AddConfigOptionStatement(statement);
        }
        return this;
    }

    public override CSharpStatement FindAndReplace(string find, string replaceWith)
    {
        foreach (var statement in Statements)
        {
            statement.FindAndReplace(find, replaceWith);
        }
        return base.FindAndReplace(find, replaceWith);
    }

    public override string GetText(string indentation)
    {
        return $@"{indentation}{Text}
{indentation}{{
{string.Join($@"
", Statements.Select(x => x.GetText($"{indentation}    ")))}
{indentation}}});";
    }
}