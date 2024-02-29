using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EfCoreKeyMappingStatement : CSharpStatement
{
    public RequiredEntityProperty[] KeyColumns { get; set; }

    public EfCoreKeyMappingStatement(ClassModel model) : base(null)
    {
        KeyColumns = model.GetExplicitPrimaryKey()
            .Select(x => new RequiredEntityProperty(Class: model.InternalElement, Name: x.Name.ToPascalCase(), Type: x.Type.Element))
            .ToArray();
    }

    public override string GetText(string indentation)
    {
        var method = KeyColumns.Length switch
        {
            0 => "HasNoKey()",
            1 => $"HasKey(x => x.{KeyColumns[0].Name})",
            _ => $"HasKey(x => new {{ {string.Join(", ", KeyColumns.Select(key => $"x.{key.Name}"))} }})"
        };
        
        return $"{indentation}builder.{method};";
    }
}