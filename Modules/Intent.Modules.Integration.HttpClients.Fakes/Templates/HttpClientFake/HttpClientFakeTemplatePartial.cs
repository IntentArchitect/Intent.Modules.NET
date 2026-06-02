using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Application.Contracts.Clients.Templates.PagedResult;
using Intent.Modules.Application.Contracts.Clients.Templates.ServiceContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Contracts.Clients.Shared.Templates.ServiceContract;
using Intent.Modules.Integration.HttpClients.Fakes.Templates.ResponseDtoFactory;
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.HttpClientFake
{
    [IntentManaged(Mode.Ignore)]
    public class HttpClientFakeTemplate : CSharpTemplateBase<IServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.Fakes.HttpClientFake";

        private const string CreateMethodName = "Create";
        private const string CreateListMethodName = "CreateList";
        private readonly IReadOnlyDictionary<string, ResponseDtoFactoryModel> _responseDtoFactoryModelsByDtoId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientFakeTemplate(IOutputTarget outputTarget, IServiceProxyModel model)
            : base(TemplateId, outputTarget, model)
        {
            _responseDtoFactoryModelsByDtoId = ResponseDtoFactoryModelProvider
                .GetModels(new[] { Model })
                .ToDictionary(factoryModel => factoryModel.Dto.Id);

            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            PagedResultTypeSource.ApplyTo(this, PagedResultTemplate.TemplateId);
            AddTypeSource(ServiceContractTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId);
            AddTypeSource(EnumContractTemplate.TemplateId);

            CSharpFile = new CSharpFile(
                    @namespace: GetFakesNamespace(),
                    relativeLocation: GetFakesFolderPath())
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .IntentManagedFully()
                .AddClass($"{Model.Name.RemoveSuffix("Http", "Client")}HttpClientFake", @class =>
                {
                    @class.RepresentsModel(Model);
                    @class.ImplementsInterface(GetTypeName(ServiceContractTemplate.TemplateId, Model));

                    foreach (var endpoint in Model.Endpoints)
                    {
                        @class.AddMethod(GetReturnType(endpoint), $"{endpoint.Name.ToPascalCase().RemoveSuffix("Async")}Async", method =>
                        {
                            method
                                .Async()
                                .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyFully());

                            if (Model.UnderlyingModel is ServiceProxyModel serviceProxyModel && serviceProxyModel.Operations.Any())
                            {
                                var operationModel = serviceProxyModel.Operations.Single(x => x.Mapping?.ElementId == endpoint.Id);
                                method.RepresentsModel(operationModel);
                            }

                            if (Model.CreateParameterPerInput)
                            {
                                foreach (var input in endpoint.Inputs)
                                {
                                    method.AddParameter(GetTypeName(input.TypeReference), input.Name.ToParameterName());
                                }
                            }
                            else
                            {
                                var fields = endpoint.InternalElement.ChildElements.Where(x => x.IsDTOFieldModel()).ToArray();

                                switch (fields.Length)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        method.AddParameter(GetTypeName(fields[0].TypeReference), fields[0].Name.ToParameterName());
                                        break;
                                    default:
                                        var parameterName = endpoint.InternalElement.SpecializationTypeId switch
                                        {
                                            CommandModel.SpecializationTypeId => "command",
                                            QueryModel.SpecializationTypeId => "query",
                                            _ => endpoint.InternalElement.Name.ToParameterName()
                                        };
                                        method.AddParameter(GetTypeName(endpoint.InternalElement), parameterName);
                                        break;
                                }
                            }

                            method.AddParameter(UseType("System.Threading.CancellationToken"), "cancellationToken", parameter => parameter.WithDefaultValue("default"));
                            AddReturnStatement(method, endpoint);
                        });
                    }

                    @class.AddMethod("void", "Dispose", method =>
                    {
                    });

                });
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

        private string GetReturnType(IHttpEndpointModel endpoint)
        {
            var taskTypeName = UseType("System.Threading.Tasks.Task");

            return endpoint.ReturnType?.Element == null
                ? taskTypeName
                : $"{taskTypeName}<{GetTypeName(endpoint.ReturnType)}>";
        }

        private void AddReturnStatement(CSharpClassMethod method, IHttpEndpointModel endpoint)
        {
            if (endpoint.ReturnType?.Element == null)
            {
                method.AddStatement($"await {UseType("System.Threading.Tasks.Task")}.CompletedTask;");
                return;
            }

            method.AddReturn(GetReturnExpression(endpoint.ReturnType));
        }

        private string GetReturnExpression(ITypeReference typeReference)
        {
            var placeholderExpression = GetReturnPlaceholderExpression(typeReference);
            var typeArgument = RequiresExplicitFromResultTypeArgument(placeholderExpression)
                ? $"<{GetTypeName(typeReference)}>"
                : string.Empty;

            return $"await {UseType("System.Threading.Tasks.Task")}.FromResult{typeArgument}({placeholderExpression})";
        }

        private string GetReturnPlaceholderExpression(ITypeReference typeReference)
        {
            if (typeReference.Element?.Id == PagedResultTemplateBase.TypeDefinitionElementId)
            {
                return GetPagedResultPlaceholderExpression(typeReference);
            }

            if (IsDictionaryType(typeReference))
            {
                return GetDictionaryPlaceholderExpression(typeReference);
            }

            if (typeReference.IsCollection)
            {
                return GetListPlaceholderExpression(typeReference);
            }

            return GetScalarPlaceholderExpression(typeReference);
        }

        private string GetPagedResultPlaceholderExpression(ITypeReference typeReference)
        {
            var dataTypeReference = typeReference.GenericTypeParameters.FirstOrDefault();
            var dataExpression = dataTypeReference == null
                ? "[]"
                : GetPagedResultDataExpression(dataTypeReference);

            return $@"new {GetTypeName(typeReference)}
{{
    TotalCount = 1,
    PageCount = 1,
    PageSize = 1,
    PageNumber = 1,
    Data = {dataExpression}
}}";
        }

        private string GetPagedResultDataExpression(ITypeReference typeReference)
        {
            if (typeReference.Element is IElement element && element.IsDTOModel() && !IsGenericDto(element))
            {
                return $"{GetFactoryTypeName(element)}.{CreateListMethodName}(1)";
            }

            return GetCollectionExpression(GetScalarPlaceholderExpression(typeReference));
        }

        private string GetListPlaceholderExpression(ITypeReference typeReference)
        {
            if (typeReference.Element is IElement element && element.IsDTOModel() && !IsGenericDto(element))
            {
                return $"{GetFactoryTypeName(element)}.{CreateListMethodName}(1)";
            }

            var itemTypeReference = GetCollectionItemTypeReference(typeReference);
            var itemExpression = GetScalarPlaceholderExpression(itemTypeReference);

            return GetCollectionExpression(itemExpression);
        }

        private static ITypeReference GetCollectionItemTypeReference(ITypeReference typeReference)
        {
            return typeReference.GenericTypeParameters.FirstOrDefault() ?? typeReference;
        }

        private string GetDictionaryPlaceholderExpression(ITypeReference typeReference)
        {
            var keyTypeReference = typeReference.GenericTypeParameters.FirstOrDefault();
            var valueTypeReference = typeReference.GenericTypeParameters.Skip(1).FirstOrDefault();
            if (keyTypeReference == null ||
                valueTypeReference == null ||
                !keyTypeReference.HasStringType())
            {
                return "default!";
            }

            return $@"new {UseType($"System.Collections.Generic.Dictionary<{GetTypeName(keyTypeReference)}, {GetTypeName(valueTypeReference)}>")}
{{
    [string.Empty] = {GetReturnPlaceholderExpression(valueTypeReference)}
}}";
        }

        private static bool IsDictionaryType(ITypeReference typeReference)
        {
            return typeReference.GenericTypeParameters.Count() >= 2 &&
                   typeReference.Element?.Name.EndsWith("Dictionary", StringComparison.Ordinal) == true;
        }

        private string GetScalarPlaceholderExpression(ITypeReference typeReference)
        {
            if (typeReference.IsNullable && IsScalarPlaceholderType(typeReference))
            {
                return "null";
            }

            if (typeReference.Element is IElement element && element.IsDTOModel() && !IsGenericDto(element))
            {
                return $"{GetFactoryTypeName(element)}.{CreateMethodName}()";
            }

            if (typeReference.HasGuidType())
            {
                return $"{UseType("System.Guid")}.Empty";
            }

            if (typeReference.HasStringType())
            {
                return "string.Empty";
            }

            if (typeReference.HasIntType())
            {
                return "0";
            }

            if (typeReference.HasLongType())
            {
                return "0L";
            }

            if (typeReference.HasShortType())
            {
                return "0";
            }

            if (typeReference.HasDecimalType())
            {
                return "0m";
            }

            if (typeReference.HasDoubleType())
            {
                return "0";
            }

            if (typeReference.HasFloatType())
            {
                return "0";
            }

            if (typeReference.HasBoolType())
            {
                return "false";
            }

            if (typeReference.HasDateTimeType())
            {
                return $"{UseType("System.DateTime")}.UnixEpoch";
            }

            if (typeReference.HasDateTimeOffsetType())
            {
                return $"{UseType("System.DateTimeOffset")}.UnixEpoch";
            }

            if (typeReference.Element is IElement enumElement && enumElement.IsEnumModel())
            {
                return GetFirstEnumValueExpression(enumElement);
            }

            return "default!";
        }

        private static bool IsScalarPlaceholderType(ITypeReference typeReference)
        {
            return typeReference.HasStringType() ||
                   typeReference.HasIntType() ||
                   typeReference.HasLongType() ||
                   typeReference.HasShortType() ||
                   typeReference.HasDecimalType() ||
                   typeReference.HasDoubleType() ||
                   typeReference.HasFloatType() ||
                   typeReference.HasBoolType() ||
                   typeReference.HasDateTimeType() ||
                   typeReference.HasDateTimeOffsetType() ||
                   typeReference.HasGuidType() ||
                   typeReference.Element?.IsEnumModel() == true;
        }

        private string GetFirstEnumValueExpression(IElement enumElement)
        {
            var enumModel = enumElement.AsEnumModel();
            var firstLiteral = enumModel.Literals.FirstOrDefault();

            return firstLiteral == null
                ? "default"
                : $"{GetTypeName(EnumContractTemplate.TemplateId, enumModel)}.{firstLiteral.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper)}";
        }

        private static string GetCollectionExpression(string itemExpression)
        {
            return itemExpression == "default!"
                ? "[]"
                : $"[{itemExpression}]";
        }

        private static bool RequiresExplicitFromResultTypeArgument(string placeholderExpression)
        {
            return placeholderExpression is "null" or "default" or "default!" or "[]" ||
                   placeholderExpression.StartsWith("[", StringComparison.Ordinal);
        }

        // Resolve a DTO's factory through Intent's type system rather than emitting a bare class name,
        // so the reference gets the correct using/qualification and Intent can disambiguate clashing
        // short names.
        private string GetFactoryTypeName(IElement dtoElement)
        {
            if (_responseDtoFactoryModelsByDtoId.TryGetValue(dtoElement.Id, out var factoryModel))
            {
                return GetTypeName(ResponseDtoFactoryTemplate.TemplateId, factoryModel);
            }

            return GetTypeName(
                ResponseDtoFactoryTemplate.TemplateId,
                new ResponseDtoFactoryModel(
                    Model,
                    new DTOModel(dtoElement),
                    requiresCreateList: false,
                    cyclicTargetDtoIds: Array.Empty<string>()));
        }

        // Generic DTOs are not factory targets (see the response factory model provider); a reference to
        // one falls back to a structural default placeholder rather than an unresolvable factory call.
        private static bool IsGenericDto(IElement element)
        {
            return new DTOModel(element).GenericTypes.Any();
        }

        private string GetFakesNamespace()
        {
            return $"{this.GetNamespace()}.Fakes";
        }

        private string GetFakesFolderPath()
        {
            var folderPath = this.GetFolderPath();
            return string.IsNullOrWhiteSpace(folderPath)
                ? "Fakes"
                : $"{folderPath}/Fakes";
        }
    }
}
