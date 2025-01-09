using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Settings;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ControllerTemplateRegistration : FilePerModelTemplateRegistration<IControllerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ControllerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ControllerTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IControllerModel model)
        {
            return new ControllerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IControllerModel> GetModels(IApplication application)
        {
            // var securedByDefault = application.Settings.GetAPISettings().DefaultAPISecurity().AsEnum() switch
            // {
            //     APISettings.DefaultAPISecurityOptionsEnum.Secured => true,
            //     APISettings.DefaultAPISecurityOptionsEnum.Unsecured => false,
            //     _ => throw new ArgumentOutOfRangeException()
            // };

            return _metadataManager.Services(application).GetServiceModels()
                .Where(p => p.Operations.Any(q => q.HasHttpSettings()))
                .Select(x => new ServiceControllerModel(x, application))
                .ToArray();
        }
    }
}