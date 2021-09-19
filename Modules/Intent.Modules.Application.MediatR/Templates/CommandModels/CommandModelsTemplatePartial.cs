using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.Templates;
using System.Collections.Generic;
using Intent.Modules.Application.Dtos.Templates;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates.DomainEnum;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.CommandModels
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CommandModelsTemplate : CSharpTemplateBase<CommandModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.CommandModels";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandModelsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
            AddTypeSource(DomainEnumTemplate.TemplateId, "List<{0}>");
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.GetConceptName())}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}");
        }

        private string GetRequestInterface()
        {
            return Model.TypeReference.Element != null ? $"IRequest<{GetTypeName(Model.TypeReference)}>" : "IRequest";
        }
    }
}