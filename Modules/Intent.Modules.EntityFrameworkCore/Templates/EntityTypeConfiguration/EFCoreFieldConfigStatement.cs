using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.EntityFrameworkCore.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Utils;
using AttributeModelStereotypeExtensions = Intent.Metadata.RDBMS.Api.AttributeModelStereotypeExtensions;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EfCoreFieldConfigStatement : CSharpStatement, IHasCSharpStatements
{
    public IList<CSharpStatement> Statements { get; } = new List<CSharpStatement>();

    public static EfCoreFieldConfigStatement CreateProperty(AttributeModel attribute,
        DatabaseSettings dbSettings,
        int? columnOrder = null)
    {
        var databaseProvider = dbSettings.DatabaseProvider().AsEnum();
        var field = new EfCoreFieldConfigStatement($"builder.Property(x => x.{attribute.Name.ToPascalCase()})", attribute);

		if (!attribute.Type.IsNullable)
        {
            field.AddStatement(".IsRequired()");
        }

        if (databaseProvider != DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos)
        {
            field.AddStatements(field.AddRdbmsMappingStatements(attribute, dbSettings, columnOrder));
        }

        return field;
    }

    public static EfCoreFieldConfigStatement CreateOwnsOne(AttributeModel attribute)
    {
        var field = new EfCoreFieldConfigStatement($"builder.OwnsOne(x => x.{attribute.Name.ToPascalCase()}, Configure{attribute.Name.ToPascalCase()})", attribute);
        if (!attribute.TypeReference.IsNullable)
        {
            field.AddStatement($".Navigation(x => x.{attribute.Name.ToPascalCase()}).IsRequired()");
        }

        return field;
    }
    
    public static EfCoreFieldConfigStatement CreateOwnsMany(AttributeModel attribute)
    {
        return new EfCoreFieldConfigStatement($"builder.OwnsMany(x => x.{attribute.Name.ToPascalCase()}, Configure{attribute.Name.ToPascalCase()})", attribute);
    }

    private EfCoreFieldConfigStatement(string text, AttributeModel model) : base(text)
    {
        AddMetadata("model", model);

    }

    private EfCoreFieldConfigStatement(string text, AssociationEndModel model) : base(text)
    {
        AddMetadata("model", model);
    }

    private void InnerAddStatement(CSharpStatement statement)
    {
        statement.Parent = this;
        Statements.Add(statement);
    }

    public EfCoreFieldConfigStatement AddStatement(CSharpStatement statement)
    {
        InnerAddStatement(statement);
        return this;
    }

    public EfCoreFieldConfigStatement AddStatements(IEnumerable<CSharpStatement> statements)
    {
        foreach (var statement in statements)
        {
            InnerAddStatement(statement);
        }
        return this;
    }

    public override string GetText(string indentation)
    {
        return $@"{indentation}{Text}{(Statements.Any() ? $@"
    {string.Join(@"
    ", Statements.Select(x => x.GetText(indentation)))}" : string.Empty)};";
    }

    private List<CSharpStatement> AddRdbmsMappingStatements(AttributeModel attribute, DatabaseSettings dbSettings, int? implicitColumnOrder)
    {
        var statements = new List<CSharpStatement>();

        if (attribute.GetPrimaryKey()?.Identity() == true)
        {
            statements.Add(".UseSqlServerIdentityColumn()");
        }

        if (attribute.HasDefaultConstraint())
        {
            var treatAsSqlExpression = attribute.GetDefaultConstraint().TreatAsSQLExpression();
            var defaultValue = attribute.GetDefaultConstraint()?.Value() ?? string.Empty;

            if (!treatAsSqlExpression &&
                !defaultValue.TrimStart().StartsWith("\"") &&
                attribute.Type.Element.Name == "string")
            {
                defaultValue = $"\"{defaultValue}\"";
            }

            if (treatAsSqlExpression &&
                !defaultValue.TrimStart().StartsWith("\""))
            {
                defaultValue = $"\"{defaultValue}\"";
            }

            var method = treatAsSqlExpression
                ? "HasDefaultValueSql"
                : "HasDefaultValue";

            statements.Add($".{method}({defaultValue})");
        }

        if (attribute.GetTextConstraints()?.SQLDataType().IsDEFAULT() == true)
        {
            var maxLength = attribute.GetTextConstraints().MaxLength();
            if (maxLength.HasValue && attribute.Type.Element.Name == "string")
            {
                statements.Add($".HasMaxLength({maxLength.Value})");
            }
        }
        else if (attribute.HasTextConstraints())
        {
            var maxLength = attribute.GetTextConstraints().MaxLength();
            switch (attribute.GetTextConstraints().SQLDataType().AsEnum())
            {
                case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.VARCHAR:
                    statements.Add($".HasColumnType(\"varchar({maxLength?.ToString() ?? "max"})\")");
                    break;
                case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NVARCHAR:
                    statements.Add($".HasColumnType(\"nvarchar({maxLength?.ToString() ?? "max"})\")");
                    break;
                case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.TEXT:
                    Logging.Log.Warning($"{attribute.InternalElement.ParentElement.Name}.{attribute.Name}: The ntext, text, and image data types will be removed in a future version of SQL Server. Avoid using these data types in new development work, and plan to modify applications that currently use them. Use nvarchar(max), varchar(max), and varbinary(max) instead.");
                    statements.Add($".HasColumnType(\"text\")");
                    break;
                case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NTEXT:
                    Logging.Log.Warning($"{attribute.InternalElement.ParentElement.Name}.{attribute.Name}: The ntext, text, and image data types will be removed in a future version of SQL Server. Avoid using these data types in new development work, and plan to modify applications that currently use them. Use nvarchar(max), varchar(max), and varbinary(max) instead.");
                    statements.Add($".HasColumnType(\"ntext\")");
                    break;
                case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.DEFAULT:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            var decimalPrecision = attribute.GetDecimalConstraints()?.Precision();
            var decimalScale = attribute.GetDecimalConstraints()?.Scale();
            var columnType = attribute.GetColumn()?.Type();

            if (!string.IsNullOrWhiteSpace(columnType))
            {
                statements.Add($".HasColumnType(\"{columnType}\")");
            }
            else if (attribute.Type.Element.Name == "decimal")
            {
                if (decimalPrecision.HasValue && decimalScale.HasValue)
                {
                    statements.Add($".HasColumnType(\"decimal({decimalPrecision}, {decimalScale})\")");
                }
                else if (!string.IsNullOrEmpty(dbSettings.DecimalPrecisionAndScale()))
                {
                    var safeString = dbSettings.DecimalPrecisionAndScale().Trim().Replace("(","").Replace(")", "");
                    statements.Add($".HasColumnType(\"decimal({safeString})\")");
                }
            }
            else if (attribute.Type.Element.GetStereotype("C#")?.GetProperty("Namespace")?.Value == "NetTopologySuite.Geometries" && 
                     dbSettings.DatabaseProvider().IsPostgresql())
            {
                // https://www.npgsql.org/efcore/mapping/nts.html#constraining-your-type-names
                statements.Add($@".HasColumnType(""geography ({attribute.Type.Element.Name.ToLower()})"")");
            }
        }

        var columnName = attribute.GetColumn()?.Name();
        if (!string.IsNullOrWhiteSpace(columnName))
        {
            statements.Add($".HasColumnName(\"{EscapeHelper.EscapeName(columnName)}\")");
        }

		var columnOrder = attribute.GetColumn()?.Order();
		if (columnOrder != null || implicitColumnOrder != null)
		{
			statements.Add($".HasColumnOrder({columnOrder ?? implicitColumnOrder})");
		}

		var computedValueSql = attribute.GetComputedValue()?.SQL();
        if (!string.IsNullOrWhiteSpace(computedValueSql))
        {

            statements.Add(
                $".HasComputedColumnSql({SanitizeUserInputString(computedValueSql)}{(attribute.GetComputedValue().Stored() ? ", stored: true" : string.Empty)})");
        }

        if (attribute.HasRowVersion())
        {
            statements.Add($".IsRowVersion()");
        }

        if (!string.IsNullOrEmpty(attribute.InternalElement?.Comment))
        {
            statements.Add($".HasComment({SanitizeUserInputString(attribute.InternalElement?.Comment)})");
        }

        return statements;
    }

    private static string SanitizeUserInputString(string computedValueSql)
    {
        var result = computedValueSql.Trim().Trim('"').Trim();
        result = result.Replace("\"", "\"\"");
        return "@\"" + result + "\"";
    }
}