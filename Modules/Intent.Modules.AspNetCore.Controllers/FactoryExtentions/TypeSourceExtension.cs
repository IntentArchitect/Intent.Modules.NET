using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.DownloadFile;
using Intent.Modules.AspNetCore.Controllers.Templates.UploadFile;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TypeSourceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.Controllers.TypeSourceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var typeSourcesToAdd = new List<ITypeSource>();
            var uploadFileTemplate = application.FindTemplateInstance<UploadFileTemplate>(UploadFileTemplate.TemplateId);
            if (uploadFileTemplate != null)
            {
                typeSourcesToAdd.Add(new UploadFileTypeSource(uploadFileTemplate));
            }
            var downloadFileTemplate = application.FindTemplateInstance<DownloadFileTemplate>(DownloadFileTemplate.TemplateId);
            if (downloadFileTemplate != null)
            {
                typeSourcesToAdd.Add(new DownloadFileTypeSource(downloadFileTemplate));
            }

            foreach (var template in GetTemplatesWhichRequireControllerTypeSources(application))
            {
                foreach (var typeSource in typeSourcesToAdd)
                {
                    template.AddTypeSource(typeSource);
                }
            }

        }

        private IEnumerable<IntentTemplateBase> GetTemplatesWhichRequireControllerTypeSources(ISoftwareFactoryExecutionContext executionContext)
        {
            return executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.Application.MediatR.CommandModels")
                .Concat(executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.Application.MediatR.QueryModels"))
                .Concat(executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.Application.MediatR.CommandHandler"))
                .Concat(executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.Application.MediatR.QueryHandler"))
                .Concat(executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.Application.ServiceImplementations.ServiceImplementation"))
                .Concat(executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.Application.Contracts.ServiceContract"))
                .Concat(executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.Application.Dtos.DtoModel"))
                .Concat(executionContext.FindTemplateInstances<IntentTemplateBase>("Intent.AspNetCore.Controllers.Controller"));
        }
    }
}