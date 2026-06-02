using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Clients.Templates.DtoContract;
using Intent.Modules.Application.Contracts.Clients.Templates.EnumContract;
using Intent.Modules.Application.Contracts.Clients.Templates.PagedResult;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.Integration.HttpClients.Fakes.Templates.FactoryHelpers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.ResponseDtoFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ResponseDtoFactoryTemplate : CSharpTemplateBase<ResponseDtoFactoryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Integration.HttpClients.Fakes.ResponseDtoFactory";
        private const string CreateMethodName = "Create";
        private const string CreateListMethodName = "CreateList";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ResponseDtoFactoryTemplate(IOutputTarget outputTarget, ResponseDtoFactoryModel model)
            : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            PagedResultTypeSource.ApplyTo(this, PagedResultTemplate.TemplateId);
            AddTypeSource(DtoContractTemplate.TemplateId);
            AddTypeSource(EnumContractTemplate.TemplateId);

            CSharpFile = new CSharpFile(GetFakesNamespace(), GetFakesFolderPath(), this)
                .AddAssemblyAttribute("[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]")
                .IntentManagedFully()
                .AddClass(Model.Name, @class =>
                {
                    var dtoTypeName = GetDtoTypeName(Model.Dto);

                    @class.Static();

                    foreach (var genericType in Model.Dto.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    @class.AddMethod(dtoTypeName, CreateMethodName, method =>
                    {
                        method.Static();

                        if (Model.Dto.Fields.Any())
                        {
                            method.AddObjectInitializerBlock($"return new {dtoTypeName}", block =>
                            {
                                foreach (var field in Model.Dto.Fields)
                                {
                                    block.AddInitStatement(
                                        field.Name.ToPascalCase(),
                                        GetPlaceholderExpression(field.TypeReference));
                                }

                                block.WithSemicolon();
                            });
                        }
                        else
                        {
                            method.AddReturn($"new {dtoTypeName}()");
                        }
                    });

                    var factoryHelpersTypeName = GetTypeName(FactoryHelpersTemplate.TemplateId);

                    @class.AddMethod(dtoTypeName, CreateMethodName, method =>
                    {
                        method.Static();
                        method.AddParameter(UseType($"System.Action<{dtoTypeName}>"), "configure");
                        method.AddReturn($"{factoryHelpersTypeName}.Configure({CreateMethodName}(), configure)");
                    });

                    if (Model.RequiresCreateList)
                    {
                        @class.AddMethod(UseType($"System.Collections.Generic.List<{dtoTypeName}>"), CreateListMethodName, method =>
                        {
                            method.Static();
                            method.AddParameter("int", "count");
                            method.AddParameter($"{UseType($"System.Action<{dtoTypeName}, int>")}?", "configure", parameter => parameter.WithDefaultValue("null"));
                            method.AddReturn($"{factoryHelpersTypeName}.List({CreateMethodName}, count, configure)");
                        });
                    }
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

        private string GetPlaceholderExpression(ITypeReference typeReference)
        {
            if (IsDictionaryType(typeReference))
            {
                return GetDictionaryPlaceholderExpression(typeReference);
            }

            if (typeReference.IsCollection)
            {
                return GetCollectionPlaceholderExpression(typeReference);
            }

            return GetScalarPlaceholderExpression(typeReference);
        }

        private string GetCollectionPlaceholderExpression(ITypeReference typeReference)
        {
            if (IsCyclicTarget(typeReference))
            {
                return "[]";
            }

            if (typeReference.Element is IElement element && element.IsDTOModel() && !IsGenericDto(element))
            {
                return $"{GetFactoryTypeName(element)}.{CreateListMethodName}(1)";
            }

            var itemTypeReference = GetCollectionItemTypeReference(typeReference);
            var itemExpression = GetScalarPlaceholderExpression(itemTypeReference);
            return itemExpression == "default!"
                ? "[]"
                : GetCollectionExpression(itemExpression);
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
    [string.Empty] = {GetPlaceholderExpression(valueTypeReference)}
}}";
        }

        private static bool IsDictionaryType(ITypeReference typeReference)
        {
            return typeReference.GenericTypeParameters.Count() >= 2 &&
                   typeReference.Element?.Name.EndsWith("Dictionary", StringComparison.Ordinal) == true;
        }

        private string GetScalarPlaceholderExpression(ITypeReference typeReference)
        {
            if (IsCyclicTarget(typeReference))
            {
                return "default!";
            }

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

        private bool IsCyclicTarget(ITypeReference typeReference)
        {
            var elementId = typeReference.Element?.Id;
            return elementId != null && Model.CyclicTargetDtoIds.Contains(elementId);
        }

        // Generic DTOs are not factory targets (see the model provider); a reference to one falls back
        // to a structural default placeholder rather than an unresolvable generic factory call.
        private static bool IsGenericDto(IElement element)
        {
            return new DTOModel(element).GenericTypes.Any();
        }

        private string GetDtoTypeName(DTOModel dto)
        {
            return GetTypeName(DtoContractTemplate.TemplateId, dto);
        }

        // Resolve a referenced DTO's factory through Intent's type system rather than emitting a bare
        // class name, so the reference gets the correct using/qualification even if the factory lives in
        // a different namespace, and Intent can disambiguate clashing short names. The factory is keyed
        // by (service proxy + DTO), with the factory name carried for fallback resolution.
        private string GetFactoryTypeName(IElement dtoElement)
        {
            var dto = new DTOModel(dtoElement);
            var factoryName = Model.FactoryNamesByDtoId.TryGetValue(dtoElement.Id, out var configuredFactoryName)
                ? configuredFactoryName
                : ResponseDtoFactoryModel.GetDefaultFactoryName(dto);

            return GetTypeName(
                TemplateId,
                new ResponseDtoFactoryModel(
                    Model.ServiceProxy,
                    dto,
                    requiresCreateList: false,
                    cyclicTargetDtoIds: Array.Empty<string>(),
                    name: factoryName,
                    factoryNamesByDtoId: Model.FactoryNamesByDtoId));
        }

        private static string GetCollectionExpression(string itemExpression)
        {
            return $"[{itemExpression}]";
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
