using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Registrations;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Templates.AzureFunctionsProject
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class AzureFunctionsProjectTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public AzureFunctionsProjectTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            var models = _metadataManager.VisualStudio(application).GetAzureFunctionsProjectModels();
            //foreach (var model in models)
            //{
            //    //var project = application.OutputTargets.Single(x => x.GetProject().Id == model.Id);
            //    registry.RegisterTemplate(TemplateId, outputTarget => new AzureFunctionsProjectTemplate(outputTarget, model));
            //}

            foreach (var model in models)
            {
                var project = application.Projects.Single(x => x.Id == model.Id);
                registry.Register(TemplateId, project, p => new AzureFunctionsProjectTemplate(p, model));
            }

        }

        public string TemplateId => AzureFunctionsProjectTemplate.TemplateId;
    }
}