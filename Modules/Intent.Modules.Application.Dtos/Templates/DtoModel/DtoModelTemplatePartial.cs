using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.ContractEnumModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Templates.DtoModel
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DtoModelTemplate : CSharpTemplateBase<DTOModel, DtoModelDecorator>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.DtoModel";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoModelTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
            AddTypeSource(TemplateFulfillingRoles.Domain.Enum, "List<{0}>");
            FulfillsRole(TemplateFulfillingRoles.Application.Contracts.Dto);
            AddTypeSource(ContractEnumModelTemplate.TemplateId, "List<{0}>");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}")
                .OnBuild((Action<CSharpFile>)(file =>
                {
                    file.AddUsing("System");
                    file.AddUsing("System.Collections.Generic");
                    var @class = file.Classes.First();

                    ConfigureClass(@class);

                    var enterClass = GetDecorators(d => d.EnterClass());
                    foreach (var line in enterClass)
                        @class.AddCodeBlock(line);

                    @class.AddConstructor();

                    if (!Model.IsAbstract)
                    {
                        @class.AddMethod(base.GetTypeName(this), "Create", method =>
                        {
                            method.Static();
                            foreach (var field in Model.Fields)
                            {
                                method.AddParameter(base.GetTypeName(field.TypeReference), field.Name.ToParameterName());
                            }
                            method.AddStatement($"return new {base.GetTypeName(this)}");
                            method.AddStatement( "{");
                            foreach (var field in Model.Fields)
                            {
                                method.AddStatement($"    {field.Name.ToPascalCase()} = {(field.Name.ToCamelCase(reservedWordEscape: true))},");
                            }
                            method.AddStatement( "};");
                        });
                    }

                    foreach (var field in Model.Fields)
                    {
                        @class.AddProperty(base.GetTypeName(field.TypeReference), field.Name.ToPascalCase(), property =>
                        {
                            property.WithComments(field.GetXmlDocLines());
                            AddPropertyAttributes(property, field);
                        });
                    }

                    var exitClass = GetDecorators(d => d.ExitClass());
                    foreach (var line in exitClass)
                        @class.AddCodeBlock(line);
                }));
        }

        private void AddPropertyAttributes(CSharpProperty property, DTOFieldModel field)
        {
            var attributes = GetDecorators(x => x.PropertyAttributes(Model, field));
            foreach (var attribute in attributes)
                property.AddAttribute(attribute);
            if (TryGetSerializedName(field, out var serializedName))
            {
                property.AddAttribute(UseType("System.Text.Json.Serialization.JsonPropertyName"), attribute =>
                {
                    attribute.AddArgument($"\"{serializedName}\"");
                });
            }
        }

        private bool TryGetSerializedName(DTOFieldModel field, out string serializedName)
        {
            if (field.HasStereotype("Serialization Settings"))
            {
                var serializationSettings = field.GetStereotype("Serialization Settings");
                var namingConvention = serializationSettings.GetProperty<string>("Naming Convention");
                serializedName = namingConvention == "Custom"
                    ? serializationSettings.GetProperty<string>("Custom Name")
                    : ApplyConvention(field.Name, namingConvention);
                return true;
            }

            if (Model.HasStereotype("Serialization Settings"))
            {
                var namingConvention = Model.GetStereotype("Serialization Settings").GetProperty<string>("Field Naming Convention");
                serializedName = ApplyConvention(field.Name, namingConvention);
                return true;
            }

            serializedName = null;
            return false;
        }

        private static string ApplyConvention(string name, string convention)
        {
            return convention switch
            {
                "Camel Case" => name.ToCamelCase(),
                "Pascal Case" => name.ToPascalCase(),
                "Snake Case" => name.ToSnakeCase(),
                "Kebab Case" => name.ToKebabCase(),
                _ => throw new ArgumentOutOfRangeException(nameof(convention), $"{convention} is not a valid casing convention"),
            };
        }

        private void ConfigureClass(CSharpClass @class)
        {
            if (Model.IsAbstract)
            {
                @class.Abstract();
            }
            if (Model.GenericTypes.Any())
            {
                foreach (var genericType in Model.GenericTypes)
                {
                    @class.AddGenericParameter(genericType);
                }
            }
            if (Model.ParentDtoTypeReference != null)
            {
                @class.WithBaseType(GetTypeName(Model.ParentDtoTypeReference));
            }
            else if (GetDecorators().Any(x => !string.IsNullOrWhiteSpace(x.BaseClass())))
            {
                @class.WithBaseType(GetDecorators().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.BaseClass()))?.BaseClass());
            }

            @class.ImplementsInterfaces(GetDecorators().SelectMany(x => x.BaseInterfaces()));

            @class.XmlComments.AddStatements(Model.GetXmlDocLines());

            var attributes = GetDecorators(x => x.ClassAttributes(Model));
            foreach (var attr in attributes)
            {
                @class.AddAttribute(attr);
            }
        }

        private List<string> GetDecorators( Func<DtoModelDecorator, string> decoratorAction)
        {
            return GetDecorators()
                    .Select(x => decoratorAction(x))
                    .Where(x => x != null && x.Trim() != string.Empty)
                    .ToList();
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}