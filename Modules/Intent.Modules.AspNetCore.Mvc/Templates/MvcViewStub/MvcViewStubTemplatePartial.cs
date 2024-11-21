using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.Mvc.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Mvc.Templates.MvcViewStub
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class MvcViewStubTemplate : IntentTemplateBase<OperationModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Mvc.MvcViewStub";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public MvcViewStubTemplate(IOutputTarget outputTarget, OperationModel model) : base(TemplateId, outputTarget, model)
        {
            Types = new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.CreateList(),
                defaultNullableFormatter: CSharpNullableFormatter.Create(OutputTarget.GetProject()));

            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);
        }

        private string GetModelType()
        {
            var typeInfo = (CSharpResolvedTypeInfo)GetTypeInfo(Model);

            return typeInfo.GetFullyQualifiedTypeName();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var viewName = !string.IsNullOrWhiteSpace(Model.GetMVCSettings().ViewName())
                ? Model.GetMVCSettings().ViewName()
                : Model.Name.ToPascalCase();

            return new TemplateFileConfig(
                fileName: $"{viewName}",
                fileExtension: "cshtml",
                relativeLocation: $"{Model.ParentService.Name.RemoveSuffix("Controller", "Service")}",
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled
            );
        }

    }
}