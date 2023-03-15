using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.ContractEnumModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Application.Dtos.Templates.DtoModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DtoModelTemplate : CSharpTemplateBase<DTOModel, DtoModelDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Dtos.DtoModel";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DtoModelTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
            AddTypeSource(TemplateFulfillingRoles.Domain.Enum, "List<{0}>");
            FulfillsRole(TemplateFulfillingRoles.Application.Contracts.Dto);
            AddTypeSource(ContractEnumModelTemplate.TemplateId, "List<{0}>");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        public string ClassAttributes()
        {
            return GetDecorators().Aggregate(x => x.ClassAttributes(Model));
        }

        public string PropertyAttributes(DTOFieldModel field)
        {
            var list = GetDecorators()
                .Select(x => x.PropertyAttributes(Model, field))
                .Select((x, i) => (i == 0 ? "": Environment.NewLine) + x?.Trim())
                .ToList();

            if (TryGetSerializedName(field, out var serializedName)) 
            {
                list.Add($"[{UseType("System.Text.Json.Serialization.JsonPropertyName")}(\"{serializedName}\")]");
            }
            return string.Concat(list);
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

        public string EnterClass()
        {
            return GetDecorators().Aggregate(x => x.EnterClass());
        }

        public string ExitClass()
        {
            return GetDecorators().Aggregate(x => x.ExitClass());
        }

        public string GetBaseTypes()
        {
            var baseTypes = new List<string>();

            if (Model.ParentDtoTypeReference != null)
            {
                baseTypes.Add(GetTypeName(Model.ParentDtoTypeReference));
            }
            else if (GetDecorators().Any(x => !string.IsNullOrWhiteSpace(x.BaseClass())))
            {
                baseTypes.Add(GetDecorators().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.BaseClass()))?.BaseClass());
            }
            baseTypes.AddRange(GetDecorators().SelectMany(x => x.BaseInterfaces()));
            return baseTypes.Any() ? $" : {string.Join(", ", baseTypes)}" : "";
        }

        public string ConstructorParameters()
        {
            var parameters = new List<string>();
            //if (Model.HasMapFromDomainMapping())
            //{
            //    if (GetTemplate<ITemplate>("Domain.Entity", Model.Mapping.ElementId).GetMetadata().CustomMetadata
            //        .TryGetValue("Surrogate Key Type", out var entitySurrogateKeyType))
            //    {
            //        parameters.Add($"{UseType(entitySurrogateKeyType)} id");
            //    }
            //}
            foreach (var field in Model.Fields)
            {
                parameters.Add($"{GetTypeName(field.TypeReference)} {field.Name.ToParameterName()}");
            }
            return $@"
            {string.Join(@",
            ", parameters)}";
        }

        public string GenericTypes => Model.GenericTypes.Any()
            ? $"<{string.Join(", ", Model.GenericTypes)}>"
            : string.Empty;

        private string AbstractDefinition => Model.IsAbstract
            ? " abstract"
            : string.Empty;
    }
}
