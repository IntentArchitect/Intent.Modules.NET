using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Contracts.Clients.Templates.DtoContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class DtoContractTemplate : CSharpTemplateBase<ServiceProxyDTOModel>
    {
        public const string TemplateId = "Intent.Application.Contracts.Clients.DtoContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoContractTemplate(IOutputTarget outputTarget, ServiceProxyDTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            AddTypeSource(TemplateId, "List<{0}>");
            AddTypeSource("Domain.Enum", "List<{0}>");
            AddTypeSource(EnumContractTemplate.TemplateId, "List<{0}>");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{((IntentTemplateBase)this).GetNamespace(Model.ServiceProxy.Name.ToPascalCase())}",
                relativeLocation: $"{((IntentTemplateBase)this).GetFolderPath(Model.ServiceProxy.Name.ToPascalCase())}");
        }

        public string GenericTypes => Model.GenericTypes.Any() ? $"<{string.Join(", ", Model.GenericTypes)}>" : "";

        private string GetAttributes(DTOFieldModel field)
        {
            return TryGetSerializedName(field, out var serializedName)
                ? $"{Environment.NewLine}        [{UseType("System.Text.Json.Serialization.JsonPropertyName")}(\"{serializedName}\")]"
                : string.Empty;
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
    }
}