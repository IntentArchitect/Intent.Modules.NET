using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;

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
        var name = !string.IsNullOrWhiteSpace(_model.GetColumn().Name())
            ? $@"
{indentation}    .HasColumnName(""{_model.GetColumn().Name()}"")"
            : string.Empty;

        var type = !string.IsNullOrWhiteSpace(_model.GetColumn().Type())
            ? $@"
{indentation}    .HasColumnType(""{_model.GetColumn().Type()}"")"
            : string.Empty;

        return @$"{indentation}builder.Property(x => x.{_model.Name.ToPascalCase()}){name}{type};";
    }
}