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
using Intent.Modules.Integration.HttpClients.Shared.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs.Templates.HttpClientStub
{
    [IntentManaged(Mode.Ignore)]
    public class HttpClientStubTemplate : CSharpTemplateBase<IServiceProxyModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Integration.HttpClients.Stubs.HttpClientStub";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HttpClientStubTemplate(IOutputTarget outputTarget, IServiceProxyModel model)
            : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            PagedResultTypeSource.ApplyTo(this, PagedResultTemplate.TemplateId);
            AddTypeSource(ServiceContractTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId);
            AddTypeSource(EnumContractTemplate.TemplateId);

            CSharpFile = new CSharpFile(
                    @namespace: this.GetNamespace(),
                    relativeLocation: this.GetFolderPath())
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .IntentManagedFully()
                .AddClass($"{Model.Name.RemoveSuffix("Http", "Client")}HttpClientStub", @class =>
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

        // Pagination has no Intent stereotype; paged queries are detected by the PagedResult return type
        // plus conventionally-named fields. These patterns mirror Intent.Modules.Application.Dtos.Pagination
        // (PagingDefaultsExtension) and the MediatR CRUD paged strategy so detection stays consistent.
        private static readonly string[] PageNumberFieldNames = { "page", "pageno", "pagenum", "pagenumber", "pageindex" };
        private static readonly string[] PageSizeFieldNames = { "size", "pagesize" };

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

            var value = endpoint.ReturnType.Element?.Id == PagedResultTemplateBase.TypeDefinitionElementId
                ? BuildPagedResultBlock(endpoint.ReturnType, new HashSet<string>(), endpoint)
                : BuildValueExpression(endpoint.ReturnType, new HashSet<string>());
            var typeArgument = WouldResolveToDefault(endpoint.ReturnType)
                ? $"<{GetTypeName(endpoint.ReturnType)}>"
                : string.Empty;

            method.AddReturn(new CSharpInvocationStatement($"await {UseType("System.Threading.Tasks.Task")}.FromResult{typeArgument}")
                .AddArgument(value)
                .WithoutSemicolon());
        }

        // Builds a fully-defaulted value for the given type reference. Every DTO is newed up directly;
        // every list (even nullable) gets exactly one fully-nested item; every property (even nullable)
        // is assigned a default rather than left null. Object/collection initializers are built through
        // the fluent CSharpObjectInitializerBlock API — never as raw brace strings.
        private CSharpStatement BuildValueExpression(ITypeReference typeReference, ISet<string> ancestorDtoIds)
        {
            if (typeReference.Element?.Id == PagedResultTemplateBase.TypeDefinitionElementId)
            {
                return BuildPagedResultBlock(typeReference, ancestorDtoIds);
            }

            if (typeReference.IsCollection)
            {
                return BuildCollectionBlock(typeReference, ancestorDtoIds);
            }

            return BuildScalarExpression(typeReference, ancestorDtoIds);
        }

        private CSharpStatement BuildScalarExpression(ITypeReference typeReference, ISet<string> ancestorDtoIds)
        {
            if (typeReference.Element is IElement element && element.IsDTOModel() && !IsGenericDto(element))
            {
                // A field whose DTO type is already an ancestor would recurse forever — terminate it.
                return ancestorDtoIds.Contains(element.Id)
                    ? new CSharpStatement("default!")
                    : BuildDtoBlock(element, ancestorDtoIds);
            }

            return new CSharpStatement(GetScalarLiteral(typeReference));
        }

        // The literal expression for a primitive, enum, or well-known struct default. Returned as a
        // string so collection items bind to the string AddStatement overload — passing a base-typed
        // CSharpStatement instead selects the obsolete AddStatement(TParent, CSharpStatement, Action) overload.
        private string GetScalarLiteral(ITypeReference typeReference)
        {
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

        // Recursively news up a DTO with one fully-nested default value per field.
        private CSharpObjectInitializerBlock BuildDtoBlock(IElement dtoElement, ISet<string> ancestorDtoIds)
        {
            var dto = new DTOModel(dtoElement);
            var block = new CSharpObjectInitializerBlock($"new {GetTypeName(DtoContractTemplate.TemplateId, dto)}");
            var childAncestorDtoIds = new HashSet<string>(ancestorDtoIds) { dtoElement.Id };

            foreach (var field in dto.Fields)
            {
                block.AddInitStatement(field.Name.ToPascalCase(), BuildValueExpression(field.TypeReference, childAncestorDtoIds));
            }

            return block;
        }

        // A list materialised with exactly one fully-nested item (empty only to break a DTO cycle).
        private CSharpStatement BuildCollectionBlock(ITypeReference typeReference, ISet<string> ancestorDtoIds)
        {
            var listTypeName = GetTypeName(typeReference);

            if (typeReference.Element is IElement element && element.IsDTOModel() && !IsGenericDto(element))
            {
                if (ancestorDtoIds.Contains(element.Id))
                {
                    return new CSharpStatement($"new {listTypeName}()");
                }

                return new CSharpObjectInitializerBlock($"new {listTypeName}")
                    .AddStatement(BuildDtoBlock(element, ancestorDtoIds));
            }

            var itemTypeReference = GetCollectionItemTypeReference(typeReference);
            return new CSharpObjectInitializerBlock($"new {listTypeName}")
                .AddStatement(GetScalarLiteral(itemTypeReference));
        }

        private CSharpObjectInitializerBlock BuildPagedResultBlock(ITypeReference typeReference, ISet<string> ancestorDtoIds, IHttpEndpointModel endpoint = null)
        {
            var (totalCount, pageCount, pageSize, pageNumber) = ResolvePagingExpressions(endpoint);

            var block = new CSharpObjectInitializerBlock($"new {GetTypeName(typeReference)}")
                .AddInitStatement("TotalCount", totalCount)
                .AddInitStatement("PageCount", pageCount)
                .AddInitStatement("PageSize", pageSize)
                .AddInitStatement("PageNumber", pageNumber);

            var dataItemTypeReference = typeReference.GenericTypeParameters.FirstOrDefault();
            block.AddInitStatement("Data", BuildPagedDataExpression(dataItemTypeReference, ancestorDtoIds));

            return block;
        }

        // The four PagedResult counters. The stub always materialises exactly one top-level item, so a
        // top-level paged result reports TotalCount/PageCount = 1 and echoes the request's page number and
        // size when those fields can be located on the query/command DTO. `endpoint` is null for nested
        // paged results, which keep the all-zero defaults.
        private (string TotalCount, string PageCount, string PageSize, string PageNumber) ResolvePagingExpressions(IHttpEndpointModel endpoint)
        {
            const string zero = "0";
            if (endpoint == null)
            {
                return (zero, zero, zero, zero);
            }

            var pageNumber = zero;
            var pageSize = zero;

            // Only the standard "whole query/command DTO as a single parameter" shape is echoed. The
            // per-input and single-field shapes keep the safe 0 fallback.
            if (!Model.CreateParameterPerInput)
            {
                var fields = endpoint.InternalElement.ChildElements.Where(x => x.IsDTOFieldModel()).ToArray();
                if (fields.Length > 1)
                {
                    var parameterName = endpoint.InternalElement.SpecializationTypeId switch
                    {
                        CommandModel.SpecializationTypeId => "command",
                        QueryModel.SpecializationTypeId => "query",
                        _ => endpoint.InternalElement.Name.ToParameterName()
                    };

                    var pageNumberField = fields.FirstOrDefault(x => PageNumberFieldNames.Contains(x.Name.ToLowerInvariant()));
                    if (pageNumberField != null)
                    {
                        pageNumber = $"{parameterName}.{pageNumberField.Name.ToPascalCase()}";
                    }

                    var pageSizeField = fields.FirstOrDefault(x => PageSizeFieldNames.Contains(x.Name.ToLowerInvariant()));
                    if (pageSizeField != null)
                    {
                        pageSize = $"{parameterName}.{pageSizeField.Name.ToPascalCase()}";
                    }
                }
            }

            return ("1", "1", pageSize, pageNumber);
        }

        // PagedResult.Data is a List<T>; build a single-item list of the page's item type.
        private CSharpStatement BuildPagedDataExpression(ITypeReference dataItemTypeReference, ISet<string> ancestorDtoIds)
        {
            var itemTypeName = dataItemTypeReference == null ? "object" : GetTypeName(dataItemTypeReference);
            var listTypeName = $"{UseType("System.Collections.Generic.List")}<{itemTypeName}>";

            if (dataItemTypeReference == null)
            {
                return new CSharpStatement($"new {listTypeName}()");
            }

            if (dataItemTypeReference.Element is IElement element && element.IsDTOModel() && !IsGenericDto(element))
            {
                if (ancestorDtoIds.Contains(element.Id))
                {
                    return new CSharpStatement($"new {listTypeName}()");
                }

                return new CSharpObjectInitializerBlock($"new {listTypeName}")
                    .AddStatement(BuildDtoBlock(element, ancestorDtoIds));
            }

            return new CSharpObjectInitializerBlock($"new {listTypeName}")
                .AddStatement(GetScalarLiteral(dataItemTypeReference));
        }

        // True when the value would be a bare `default!` (a generic DTO or an unsupported scalar type),
        // in which case Task.FromResult needs an explicit type argument to infer the result type.
        private bool WouldResolveToDefault(ITypeReference typeReference)
        {
            if (typeReference.IsCollection || typeReference.Element?.Id == PagedResultTemplateBase.TypeDefinitionElementId)
            {
                return false;
            }

            if (typeReference.Element is IElement element && element.IsDTOModel())
            {
                return IsGenericDto(element);
            }

            return !IsSupportedScalar(typeReference);
        }

        private static bool IsSupportedScalar(ITypeReference typeReference)
        {
            return typeReference.HasGuidType() ||
                   typeReference.HasStringType() ||
                   typeReference.HasIntType() ||
                   typeReference.HasLongType() ||
                   typeReference.HasShortType() ||
                   typeReference.HasDecimalType() ||
                   typeReference.HasDoubleType() ||
                   typeReference.HasFloatType() ||
                   typeReference.HasBoolType() ||
                   typeReference.HasDateTimeType() ||
                   typeReference.HasDateTimeOffsetType() ||
                   typeReference.Element?.IsEnumModel() == true;
        }

        private static ITypeReference GetCollectionItemTypeReference(ITypeReference typeReference)
        {
            return typeReference.GenericTypeParameters.FirstOrDefault() ?? typeReference;
        }

        private string GetFirstEnumValueExpression(IElement enumElement)
        {
            var enumModel = enumElement.AsEnumModel();
            var firstLiteral = enumModel.Literals.FirstOrDefault();

            return firstLiteral == null
                ? "default"
                : $"{GetTypeName(EnumContractTemplate.TemplateId, enumModel)}.{firstLiteral.Name.ToCSharpIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper)}";
        }

        // Generic DTOs are not newable inline (open type parameters have no concrete placeholder); a
        // reference to one falls back to a structural default rather than an unresolvable construction.
        private static bool IsGenericDto(IElement element)
        {
            return new DTOModel(element).GenericTypes.Any();
        }
    }
}
