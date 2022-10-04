using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EFCoreFieldConfigStatement : EFCoreConfigStatementBase
{
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();
    public EFCoreFieldConfigStatement(string text, IMetadataModel model)
    {
        Text = text;
        AddMetadata("model", model);
    }

    public EFCoreFieldConfigStatement AddStatement(CSharpStatement statement)
    {
        Statements.Add(statement);
        return this;
    }

    public EFCoreFieldConfigStatement AddStatements(IEnumerable<CSharpStatement> statements)
    {
        foreach (var statement in statements)
        {
            Statements.Add(statement);
        }
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{indentation}{Text}{(Statements.Any() ? $@"
    {string.Join(@"
    ", Statements.Select(x => x.GetText(indentation)))}" : string.Empty)};";
    }
}