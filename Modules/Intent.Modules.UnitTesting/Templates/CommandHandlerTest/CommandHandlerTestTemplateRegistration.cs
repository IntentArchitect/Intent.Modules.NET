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

namespace Intent.Modules.UnitTesting.Templates.CommandHandlerTest
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class CommandHandlerTestTemplateRegistration : FilePerModelTemplateRegistration<IElement>
    {
        private readonly IMetadataManager _metadataManager;

        public CommandHandlerTestTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CommandHandlerTestTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IElement model)
        {
            return new CommandHandlerTestTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IElement> GetModels(IApplication application)
        {
            var generationMode = application.Settings.GetUnitTestSettings().UnitTestGenerationMode().AsEnum();

            return _metadataManager.GetDesigner(application.Id, "Services").GetElementsOfType(SpecializationTypeIds.Command)
                .Where(c => generationMode == UnitTestSettings.UnitTestGenerationModeOptionsEnum.All ||
                    c.HasUnitTestStereotype());
        }
    }
}