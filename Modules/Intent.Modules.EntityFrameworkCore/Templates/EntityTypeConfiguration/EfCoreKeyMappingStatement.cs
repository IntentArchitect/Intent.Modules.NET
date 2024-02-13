using System;
using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EfCoreKeyMappingStatement : CSharpStatement
{
    public RequiredEntityProperty[] KeyColumns { get; set; }

    public EfCoreKeyMappingStatement(ClassModel model) : base(null)
    {
        if (model.GetExplicitPrimaryKey().Any())
        {
            KeyColumns = model.GetExplicitPrimaryKey()
                .Select(x => new RequiredEntityProperty(Class: model.InternalElement, Name: x.Name.ToPascalCase(), Type: x.Type.Element)).ToArray();
            return;
        }

        KeyColumns = new[] { new RequiredEntityProperty(Class: model.InternalElement, Name: "Id", Type: null) };
    }

    public override string GetText(string indentation)
    {
        var keys = KeyColumns.Count() == 1
            ? "x." + KeyColumns[0].Name
            : $"new {{ {string.Join(", ", KeyColumns.Select(key => $"x.{key.Name}"))} }}";

        return $@"{indentation}builder.HasKey(x => {keys});";
    }
}