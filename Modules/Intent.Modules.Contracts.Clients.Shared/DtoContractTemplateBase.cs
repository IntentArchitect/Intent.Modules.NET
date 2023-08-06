using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Integration.HttpClients.Shared;

namespace Intent.Modules.Contracts.Clients.Shared
{
    public abstract class DtoContractTemplateBase : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        protected DtoContractTemplateBase(
            string templateId,
            IOutputTarget outputTarget,
            DTOModel model,
            string enumContractTemplateId)
            : base(templateId, outputTarget, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            AddTypeSource(templateId, "System.Collections.Generic.List<{0}>");
            AddTypeSource(enumContractTemplateId, "System.Collections.Generic.List<{0}>");

            CSharpFile = new CSharpFile(
                    @namespace: this.GetPackageBasedNamespace(),
                    relativeLocation: this.GetPackageBasedRelativeLocation())
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .AddClass($"{Model.Name}", @class =>
                {
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    @class.AddMethod($"{ClassName}{GenericTypes}", "Create", method =>
                    {
                        method.Static();

                        foreach (var field in Model.Fields)
                        {
                            method.AddParameter(GetTypeName(field.TypeReference), field.Name.ToParameterName());
                        }

                        method.AddObjectInitializerBlock($"return new {ClassName}{GenericTypes}", block =>
                        {
                            foreach (var field in Model.Fields)
                            {
                                block.AddInitStatement(field.Name.ToPascalCase(), field.Name.ToParameterName());
                            }

                            block.WithSemicolon();
                        });
                    });

                    foreach (var field in Model.Fields)
                    {
                        @class.AddProperty(GetTypeName(field.TypeReference), field.Name.ToPascalCase(), property =>
                        {
                            if (TryGetSerializedName(field, out var serializedName))
                            {
                                property.AddAttribute(
                                    $"{UseType("System.Text.Json.Serialization.JsonPropertyName")}(\"{serializedName}\")");
                            }
                        });
                    }
                });
        }

        public string GenericTypes => Model.GenericTypes.Any() ? $"<{string.Join(", ", Model.GenericTypes)}>" : "";

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

        public string ConstructorParameters()
        {
            var parameters = new List<string>();

            foreach (var field in Model.Fields)
            {
                parameters.Add($"{GetTypeName(field.TypeReference)} {field.Name.ToParameterName()}");
            }

            const string newLine = @"
            ";
            return newLine + string.Join($",{newLine}", parameters);
        }

        public CSharpFile CSharpFile { get; }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        public override RoslynMergeConfig ConfigureRoslynMerger() => ToFullyManagedUsingsMigration.GetConfig(Id, 2);
    }
}