using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;

namespace Intent.Modules.Contracts.Clients.Shared.Templates.DtoContract
{
    public class DtoContractTemplateConfig(
        string enumContractTemplateId,
        string pagedResultTemplateId,
        IFileNamespaceProvider fileNamespaceProvider)
    {
        public string EnumContractTemplateId { get; } = enumContractTemplateId;
        public string PagedResultTemplateId { get; } = pagedResultTemplateId;
        public IFileNamespaceProvider FileNamespaceProvider { get; } = fileNamespaceProvider;
        /// <summary>
        /// Will instantiate properties (i.e. <code>Property = new();</code>) if the property is a DTO and non-nullable
        /// </summary>
        public bool InstantiateNonNullableDtoProperties { get; set; }
    }
    public abstract class DtoContractTemplateBase : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        private static ISet<string> _outboundDtoElementIds = new HashSet<string>();

        protected DtoContractTemplateBase(
            string templateId,
            IOutputTarget outputTarget,
            DTOModel model,
            string enumContractTemplateId,
            string pagedResultTemplateId,
            IFileNamespaceProvider fileNamespaceProvider)
            : this(templateId, outputTarget, model, new DtoContractTemplateConfig(enumContractTemplateId, pagedResultTemplateId, fileNamespaceProvider))
        {
        }

        protected DtoContractTemplateBase(
            string templateId,
            IOutputTarget outputTarget,
            DTOModel model,
            DtoContractTemplateConfig config)
            : base(templateId, outputTarget, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));

            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            PagedResultTypeSource.ApplyTo(this, config.PagedResultTemplateId);
            AddTypeSource(templateId);
            AddTypeSource(config.EnumContractTemplateId);

            CSharpFile = new CSharpFile(
                    @namespace: config.FileNamespaceProvider.GetFileNamespace(this),
                    relativeLocation: config.FileNamespaceProvider.GetFileLocation(this))
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .AddClass($"{Model.Name}", @class =>
                {
                    if (_outboundDtoElementIds.Contains(model.Id))
                    {
                        @class.AddMetadata("is-inbound", true);
                    }

                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    // See this article on how to handle NRTs for DTOs
                    // https://github.com/dotnet/docs/issues/18099
                    var nullableMembers = Model.Fields
                        .Where(x => string.IsNullOrWhiteSpace(x.Value) &&
                                    NeedsNullabilityAssignment(GetTypeInfo(x.TypeReference)))
                        .Select(x => $"{x.Name.ToPascalCase()} = {GetNullabilityAssignment(x.TypeReference, config)};")
                        .ToArray();

                    if (nullableMembers.Any())
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddStatements(nullableMembers);
                        });
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
                        @class.AddProperty(field, property =>
                        {
                            property.AddMetadata("model", field);

                            if (TryGetSerializedName(field, out var serializedName))
                            {
                                property.AddAttribute(
                                    $"{UseType("System.Text.Json.Serialization.JsonPropertyName")}(\"{serializedName}\")");
                            }
                        });
                    }
                });
        }

        internal static void SetOutboundDtoElementIds(ISet<string> outboundDtoElementIds)
        {
            _outboundDtoElementIds = outboundDtoElementIds;
        }

        private static bool NeedsNullabilityAssignment(IResolvedTypeInfo typeInfo)
        {
            return !typeInfo.IsPrimitive
                   && !typeInfo.IsNullable
                   && (typeInfo.TypeReference == null || !typeInfo.TypeReference.Element.IsEnumModel());
        }

        private string GetNullabilityAssignment(ITypeReference typeReference, DtoContractTemplateConfig config)
        {
            return config.InstantiateNonNullableDtoProperties && typeReference.Element.IsDTOModel() 
                ? typeReference.IsCollection ? "[]" : "new()" 
                : "null!";
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