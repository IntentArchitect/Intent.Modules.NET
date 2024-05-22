using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.DtoContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperResponseMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MapperResponseMessageTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperResponseMessage";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MapperResponseMessageTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);

            CSharpFile = new CSharpFile(_namespaceProvider.GetFileNamespace(this), this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddConstructor();

                    var hasAppDto = TryGetTemplate<ITemplate>(TemplateRoles.Application.Contracts.Dto, Model, out _);
                    var hasProxyDto = TryGetTemplate<ITemplate>(DtoContractTemplate.TemplateId, Model, out _);

                    if (hasAppDto)
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameter(GetFullyQualifiedTypeName(TemplateRoles.Application.Contracts.Dto, Model), "dto");
                            foreach (var property in Model.Fields)
                            {
                                ctor.AddStatement($"{property.Name} = dto.{property.Name};");
                            }
                        });
                    }

                    foreach (var property in Model.Fields)
                    {
                        string typeToUse;
                        if (property.TypeReference?.Element?.SpecializationType == "DTO")
                        {
                            typeToUse = GetFullyQualifiedTypeExpression(hasAppDto ? TemplateRoles.Application.Contracts.Dto : DtoContractTemplate.TemplateId, property);
                        }
                        else
                        {
                            typeToUse = GetTypeName(property);
                        }

                        @class.AddProperty(typeToUse, property.Name);
                    }

                    if (hasProxyDto)
                    {
                        @class.AddMethod(GetFullyQualifiedTypeName(DtoContractTemplate.TemplateId, Model), "ToDto", method =>
                        {
                            method.AddInvocationStatement($"return {GetFullyQualifiedTypeName(DtoContractTemplate.TemplateId, Model)}.Create", invoke =>
                            {
                                foreach (var property in Model.Fields)
                                {
                                    invoke.AddArgument(property.Name);
                                }
                            });
                        });
                    }
                });
        }

        private readonly SourcePackageFileNamespaceProvider _namespaceProvider = new();

        private string GetFullyQualifiedTypeExpression(string templateId, DTOFieldModel fieldModel)
        {
            var type = GetFullyQualifiedTypeName(templateId, fieldModel.TypeReference.Element);
            return fieldModel.TypeReference switch
            {
                var returnType when returnType.IsCollection => $"{UseType("System.Collections.Generic.List")}<{type}>",
                var returnType when returnType.IsNullable => $"{type}?",
                _ => type
            };
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