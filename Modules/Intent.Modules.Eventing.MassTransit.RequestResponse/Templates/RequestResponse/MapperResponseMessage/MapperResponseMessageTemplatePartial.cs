using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
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
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);

            CSharpFile = new CSharpFile(_namespaceProvider.GetFileNamespace(this), this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddConstructor();

                    if (TryGetTemplate<ITemplate>(TemplateRoles.Application.Contracts.Dto, Model, out _))
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
                        @class.AddProperty(GetTypeName(property), property.Name);
                    }

                    if (TryGetTemplate<ITemplate>(DtoContractTemplate.TemplateId, Model, out _))
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