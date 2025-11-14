using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.UnitTesting.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.UnitTesting.Api;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Templates.ServiceOperationTest
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ServiceOperationTestTemplateRegistration : FilePerModelTemplateRegistration<IElement>
    {
        private readonly IMetadataManager _metadataManager;

        public ServiceOperationTestTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ServiceOperationTestTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IElement model)
        {
            return new ServiceOperationTestTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IElement> GetModels(IApplication application)
        {
            return _metadataManager.GetDesigner(application.Id, "Services").GetElementsOfType(SpecializationTypeIds.Service)
                .Where(s => s.HasUnitTestStereotype() ||
                s.GetOperations().Any(o => o.HasUnitTestStereotype()));
        }
    }
}