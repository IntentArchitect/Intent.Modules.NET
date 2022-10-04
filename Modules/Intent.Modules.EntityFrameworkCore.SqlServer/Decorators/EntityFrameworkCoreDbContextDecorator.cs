using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.SqlServer.Settings;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using AttributeModelStereotypeExtensions = Intent.Metadata.RDBMS.Api.AttributeModelStereotypeExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.SqlServer.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreDbContextDecorator : DecoratorBase
    {
        private readonly EntityTypeConfigurationTemplate _template;

        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.SqlServer.EntityTypeConfigurationTemplateDecorator";

        public ISoftwareFactoryExecutionContext ExecutionContext { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityFrameworkCoreDbContextDecorator(EntityTypeConfigurationTemplate template, IApplication application)
        {
            _template = template;
            ExecutionContext = application;
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                foreach (var method in @class.Methods)
                {
                    if (method.TryGetMetadata<IElement>("model", out var model))
                    {
                        if (model.IsClassModel())
                        {
                            method.InsertStatement(0, GetKeyMapping(model.AsClassModel()).ToString());
                        }
                    }
                    foreach (var statement in method.Statements.OfType<EFCoreFieldConfigStatement>())
                    {
                        if (statement.TryGetMetadata<AttributeModel>("model", out var attribute))
                        {
                            statement.AddStatements(GetAttributeMappingStatements(attribute));
                        }
                    }

                    foreach (var statement in method.Statements.OfType<EFCoreAssociationConfigStatement>())
                    {
                        if (statement.TryGetMetadata<AssociationEndModel>("model", out var associationEnd))
                        {
                            //statement.AddStatements(GetAttributeMappingStatements(associationEnd));
                        }
                    }
                }
            });
        }

        private CSharpStatement GetKeyMapping(ClassModel model)
        {
            if (model.ParentClass != null && (!model.ParentClass.IsAbstract || !ExecutionContext.Settings
                    .GetDatabaseSettings().InheritanceStrategy().IsTPC()))
            {
                return string.Empty;
            }

            if (!model.GetExplicitPrimaryKey().Any())
            {
                var rootEntity = model;
                while (rootEntity.ParentClass != null)
                {
                    rootEntity = rootEntity.ParentClass;
                }
                EnsureColumnsOnEntity(rootEntity.InternalElement, new EntityTypeConfigurationTemplate.RequiredColumn(Type: this.GetDefaultSurrogateKeyType(), Name: "Id", Order: 0));

                return $@"builder.HasKey(x => x.Id);";
            }
            else
            {
                var keys = model.GetExplicitPrimaryKey().Count() == 1
                    ? "x." + model.GetExplicitPrimaryKey().Single().Name.ToPascalCase()
                    : $"new {{ {string.Join(", ", model.GetExplicitPrimaryKey().Select(x => "x." + x.Name.ToPascalCase()))} }}";
                return $@"builder.HasKey(x => {keys});";
            }
        }


        private List<CSharpStatement> GetAttributeMappingStatements(AttributeModel attribute)
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
                if (decimalPrecision.HasValue && decimalScale.HasValue)
                {
                    statements.Add($".HasColumnType(\"decimal({decimalPrecision}, {decimalScale})\")");
                }
                else if (!string.IsNullOrWhiteSpace(columnType))
                {
                    statements.Add($".HasColumnType(\"{columnType}\")");
                }
            }

            var columnName = attribute.GetColumn()?.Name();
            if (!string.IsNullOrWhiteSpace(columnName))
            {
                statements.Add($".HasColumnName(\"{columnName}\")");
            }

            var computedValueSql = attribute.GetComputedValue()?.SQL();
            if (!string.IsNullOrWhiteSpace(computedValueSql))
            {
                statements.Add(
                    $".HasComputedColumnSql(\"{computedValueSql}\"{(attribute.GetComputedValue().Stored() ? ", stored: true" : string.Empty)})");
            }

            if (attribute.HasRowVersion())
            {
                statements.Add($".IsRowVersion()");
            }

            return statements;
        }


        private void EnsureColumnsOnEntity(ICanBeReferencedType entityModel, params EntityTypeConfigurationTemplate.RequiredColumn[] columns)
        {
            if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", entityModel.Id, out var template))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var associatedClass = file.Classes.First();
                    foreach (var column in columns)
                    {
                        if (!associatedClass.GetAllProperties().Any(x => x.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var associationProperty = associatedClass.Properties.SingleOrDefault(x => x.Name.Equals(column.Name.RemoveSuffix("Id")));

                            if (column.Order.HasValue)
                            {
                                associatedClass.InsertProperty(column.Order.Value, template.UseType(column.Type), column.Name, ConfigureProperty);
                            }
                            else if (associationProperty != null)
                            {
                                associatedClass.InsertProperty(associatedClass.Properties.IndexOf(associationProperty), template.UseType(column.Type), column.Name, ConfigureProperty);
                            }
                            else
                            {
                                associatedClass.AddProperty(template.UseType(column.Type), column.Name, ConfigureProperty);
                            }

                            void ConfigureProperty(CSharpProperty property)
                            {
                                if (column.IsPrivate)
                                {
                                    property.Private();
                                }
                            }
                        }
                    }
                });
            }
        }

        public string GetDefaultSurrogateKeyType()
        {
            return GetDefaultSurrogateKeyType(_template.ExecutionContext);
        }

        public static string GetDefaultSurrogateKeyType(ISoftwareFactoryExecutionContext executionContext)
        {
            var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
            switch (settingType)
            {
                case "guid":
                    return "System.Guid";
                case "int":
                    return "int";
                case "long":
                    return "long";
                default:
                    return settingType;
            }
        }
    }
}