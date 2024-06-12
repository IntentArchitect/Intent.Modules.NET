using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using System;
using System.Linq;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EfCoreKeyColumnPropertyStatement : CSharpStatement
{
    private readonly AttributeModel _model;
	private readonly int? _implicitOrder;

	public EfCoreKeyColumnPropertyStatement(AttributeModel model, int? implicitOrder) : base(null)
    {
        _model = model;
		_implicitOrder = implicitOrder;
    }

    public override string GetText(string indentation)
    {
        var name = string.Empty;
        if (!string.IsNullOrWhiteSpace(_model.GetColumn()?.Name()))
        {
            name = $@"
{indentation}    .HasColumnName(""{EscapeHelper.EscapeName(_model.GetColumn().Name())}"")";
        }

        var type = string.Empty;
        if (!string.IsNullOrWhiteSpace(_model.GetColumn()?.Type()))
        {
            type = $@"
{indentation}    .HasColumnType(""{_model.GetColumn().Type()}"")";
        }

		var order = string.Empty;
		var columnOrder = _model.GetColumn()?.Order();
		if (columnOrder != null || _implicitOrder!= null)
		{
			order = $@"
{indentation}    .HasColumnOrder({columnOrder ?? _implicitOrder})";
		}

		var valueGeneratedOnAdd = NonConventionalOrdinalPrimaryKeyRequiresConfiguration(_model)
            ? $@"
{indentation}    .ValueGeneratedOnAdd()"
            : string.Empty;

        return @$"{indentation}builder.Property(x => x.{_model.Name.ToPascalCase()}){name}{type}{order}{valueGeneratedOnAdd};";
    }

    public static bool RequiresConfiguration(AttributeModel attribute)
    {
        return attribute.HasColumn() || NonConventionalOrdinalPrimaryKeyRequiresConfiguration(attribute);
    }

    private static bool NonConventionalOrdinalPrimaryKeyRequiresConfiguration(AttributeModel attribute)
    {
        // Conventional Ordinal Primary Key Types: short, int, long, Guid
        // Non-Conventional Ordinal Primary Key Types: byte, decimal, double, float
        var byteId = "A4E9102F-C1C8-4902-A417-CA418E1874D2";
        var decimalId = "675C7B84-997A-44E0-82B9-CD724C07C9E6";
        var doubleId = "24A77F70-5B97-40DD-8F9A-4208AD5F9219";
        var floatId = "341929E9-E3E7-46AA-ACB3-B0438421F4C4";
        var nonConventionalOrdinalTypeWhitelist = new[] { byteId, decimalId, doubleId, floatId };

        return (!attribute.InternalElement.ParentElement.IsClassModel() ||
               attribute.Class.GetExplicitPrimaryKey().All(keyAttribute => keyAttribute.Equals(attribute))) &&
               nonConventionalOrdinalTypeWhitelist.Contains(attribute.InternalElement.TypeReference.Element.Id.ToUpper());
    }
}