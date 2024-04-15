using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using System.Linq;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EfCoreKeyColumnPropertyStatement : CSharpStatement
{
    private readonly AttributeModel _model;

    public EfCoreKeyColumnPropertyStatement(AttributeModel model) : base(null)
    {
        _model = model;
    }

    public override string GetText(string indentation)
    {
        var name = !string.IsNullOrWhiteSpace(_model.GetColumn()?.Name())
            ? $@"
{indentation}    .HasColumnName(""{EscapeHelper.EscapeName(_model.GetColumn()?.Name())}"")"
            : string.Empty;

        var type = !string.IsNullOrWhiteSpace(_model.GetColumn()?.Type())
            ? $@"
{indentation}    .HasColumnType(""{_model.GetColumn()?.Type()}"")"
            : string.Empty;

        var valueGeneratedOnAdd = IsNonConventionalOrdinalKey()
            ? $@"
{indentation}    .ValueGeneratedOnAdd()"
            : string.Empty;

        return @$"{indentation}builder.Property(x => x.{_model.Name.ToPascalCase()}){name}{type}{valueGeneratedOnAdd};";
    }

    private bool IsNonConventionalOrdinalKey()
    {
        var byteId = "A4E9102F-C1C8-4902-A417-CA418E1874D2";
        var decimalId = "675C7B84-997A-44E0-82B9-CD724C07C9E6";
        var doubleId = "24A77F70-5B97-40DD-8F9A-4208AD5F9219";
        var floatId = "341929E9-E3E7-46AA-ACB3-B0438421F4C4";
        var nonConventionalOrdinalTypeWhitelist = new[] { byteId, decimalId, doubleId, floatId };

        return nonConventionalOrdinalTypeWhitelist.Contains(_model.InternalElement.TypeReference.Element.Id.ToUpper());
    }
}