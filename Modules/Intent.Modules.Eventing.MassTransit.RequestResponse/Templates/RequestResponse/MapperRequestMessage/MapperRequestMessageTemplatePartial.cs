using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.ClientContracts.DtoContract;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestMessage
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MapperRequestMessageTemplate : CSharpTemplateBase<HybridDtoModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MapperRequestMessageTemplate(IOutputTarget outputTarget, HybridDtoModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);

            CSharpFile = new CSharpFile(_namespaceProvider.GetFileNamespace(this), this.GetFolderPath())
                .AddClass(model.Name, @class =>
                {
                    var hasMapperRequestInterface = TryGetTemplate<ITemplate>(MapperRequestInterfaceTemplate.TemplateId, out var requestTemplate) && requestTemplate.CanRunTemplate();
                    if (hasMapperRequestInterface)
                    {
                        @class.ImplementsInterface(this.GetMapperRequestInterfaceName());
                    }

                    @class.AddConstructor();

                    if (TryGetTypeName(DtoContractTemplate.TemplateId, Model, out var dtoContractType))
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameter(dtoContractType, "dto");
                            foreach (var property in Model.Properties)
                            {
                                ctor.AddStatement($"{property.Name} = dto.{property.Name};");
                            }
                        });
                    }

                    foreach (var property in Model.Properties)
                    {
                        @class.AddProperty(GetTypeName(property), property.Name);
                    }

                    if (hasMapperRequestInterface)
                    {
                        @class.AddMethod("object", "CreateRequest", method =>
                        {
                            method.AddInvocationStatement($"return new {GetTypeName(model.InternalElement)}", stmt =>
                            {
                                foreach (var property in Model.Properties)
                                {
                                    stmt.AddArgument(property.Name);
                                }
                            });
                        });
                    }
                });
        }

        private readonly SourcePackageFileNamespaceProvider _namespaceProvider = new();

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