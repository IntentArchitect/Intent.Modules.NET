using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.Constants.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TextConstraintFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.Constants.TextConstraintFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;


        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var entityTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary));
            foreach (var template in entityTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    if (!@class.TryGetMetadata<ClassModel>("model", out var classModel))
                    {
                        return;
                    }

                    foreach (var attributeModel in classModel.Attributes)
                    {
                        if (!RequiresConstant(attributeModel, out var length))
                        {
                            continue;
                        }

                        @class.AddField("int", $"{attributeModel.Name}MaxLength", field =>
                        {
                            field.Metadata.Add("model", attributeModel);
                            field.Constant(length.ToString());

                            UpdateEfConfiguration(application, template, classModel, attributeModel, field);
                        });
                    }
                });
            }

            PatchValidator(application, "Intent.Application.MediatR.FluentValidation.CommandValidator");
            PatchValidator(application, "Intent.Application.MediatR.FluentValidation.QueryValidator");
            PatchValidator(application, "Intent.Application.FluentValidation.Dtos.DTOValidator");
        }

        private static void PatchValidator(IApplication application, string validatorTemplateId)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(validatorTemplateId));
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("ConfigureValidationRules");
                    foreach (var statement in method.Statements)
                    {
                        if (statement is not CSharpMethodChainStatement propertyStatement)
                        {
                            continue;
                        }

                        var maxLengthStatement = propertyStatement.Statements
                            .FirstOrDefault(s => s.GetText(string.Empty).StartsWith("MaximumLength"));

                        if (maxLengthStatement == null ||
                            !propertyStatement.TryGetMetadata<DTOFieldModel>("model", out var dtoField) ||
                            dtoField.Mapping == null ||
                            !dtoField.Mapping.Element.IsAttributeModel())
                        {
                            continue;
                        }

                        var attributeModel = dtoField.Mapping.Element.AsAttributeModel();
                        var entityTypeName = template.GetTypeName(TemplateRoles.Domain.Entity.Primary, attributeModel.Class);

                        var entityTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, attributeModel.Class);
                        var entityOutput = entityTemplate.CSharpFile.Classes.First();

                        var constant = entityOutput.Fields.FirstOrDefault(f => f.AccessModifier.Contains(" const") && f.TryGetMetadata<AttributeModel>("model", out var constAttributeModel) && constAttributeModel.Id == attributeModel.Id);
                        if (constant != null)
                        {
                            maxLengthStatement.Replace(new CSharpStatement($"MaximumLength( {entityTypeName}.{constant.Name} )"));
                        }
                    }
                });
            }
        }

        private static void UpdateEfConfiguration(IApplication application, ICSharpFileBuilderTemplate entityTemplate, ClassModel classModel, AttributeModel attributeModel, CSharpField constant)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.EntityTypeConfiguration", classModel);
            if (template == null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var method = @class.FindMethod("Configure");

                var propertyBuilderStatement = method
                    .FindStatement(s => s.TryGetMetadata<AttributeModel>("model", out var configuredAttribute) &&
                                        configuredAttribute.Id == attributeModel.Id);
                if (propertyBuilderStatement is not IHasCSharpStatements parentStatement)
                {
                    //Upgrade Intent.EntityFrameworkCore module to >= 4.4.1"
                    return;
                }

                foreach (var statement in parentStatement.Statements.ToArray())
                {
                    var actualStatement = statement.GetText("");
                    var constantExpression = $" {((CSharpTemplateBase<ClassModel>)template).GetTypeName(entityTemplate)}.{constant.Name} ";

                    if (actualStatement.StartsWith(".HasMaxLength("))
                    {
                        statement.Replace(new CSharpStatement($".HasMaxLength({constantExpression})"));
                    }
                    else if (actualStatement.StartsWith(".HasColumnType"))
                    {
                        var columnType = GetSqlColumnType(attributeModel);
                        statement.Replace(new CSharpStatement($".HasColumnType($\"{columnType}({{{constantExpression}}})\")"));
                    }
                }
            });
        }

        private static string GetSqlColumnType(AttributeModel attributeModel)
        {
            return attributeModel.GetTextConstraints().SQLDataType().AsEnum() switch
            {
                AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.VARCHAR => "varchar",
                AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NVARCHAR => "nvarchar",
                _ => throw new Exception($"Unexpected type {attributeModel.GetTextConstraints().SQLDataType().AsEnum()}"),
            };
        }

        private static bool RequiresConstant(AttributeModel attribute, out int length)
        {
            length = -1;
            if (!attribute.HasTextConstraints())
            {
                return false;
            }

            var maxLength = attribute.GetTextConstraints().MaxLength();
            if (!maxLength.HasValue)
            {
                return false;
            }

            length = maxLength.Value;
            if (attribute.GetTextConstraints()?.SQLDataType().IsDEFAULT() == true)
            {
                if (attribute.Type.Element.Name == "string")
                {
                    return true;
                }
            }
            else
            {
                switch (attribute.GetTextConstraints().SQLDataType().AsEnum())
                {
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.VARCHAR:
                        return true;
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NVARCHAR:
                        return true;
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.TEXT:
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.NTEXT:
                    case AttributeModelStereotypeExtensions.TextConstraints.SQLDataTypeOptionsEnum.DEFAULT:
                    default:
                        break;
                }
            }

            return false;
        }
    }
}