using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.EnumContract;
using Intent.Modules.AspNetCore.IntegrationTesting.Templates.PagedResult;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.FileNamespaceProviders;
using Intent.Modules.Contracts.Clients.Shared.Templates.DtoContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.DtoContract
{
    [IntentManaged(Mode.Ignore)]
    public partial class DtoContractTemplate : DtoContractTemplateBase
    {
        public const string TemplateId = "Intent.AspNetCore.IntegrationTesting.DtoContract";

        public DtoContractTemplate(IOutputTarget outputTarget, DTOModel model)
            : base(
        templateId: TemplateId,
        outputTarget: outputTarget,
        model: model,
        enumContractTemplateId: EnumContractTemplate.TemplateId,
        pagedResultTemplateId: PagedResultTemplate.TemplateId,
        fileNamespaceProvider: new OutputTargetFileNamespaceProvider())
        {
        }

    }
}