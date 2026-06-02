using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Integration.HttpClients.Fakes.Templates.ResponseDtoFactory;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Fakes.Templates.FactoryHelpers
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class FactoryHelpersTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public FactoryHelpersTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => FactoryHelpersTemplate.TemplateId;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication application)
        {
            // Every generated factory delegates its Create(Action<T>) (and any CreateList) to
            // FactoryHelpers, so the helper is generated whenever at least one factory exists.
            var hasAnyFactory = ResponseDtoFactoryModelProvider
                .GetModels(_metadataManager, application)
                .Any();

            if (hasAnyFactory)
            {
                registry.RegisterTemplate(TemplateId, outputTarget => new FactoryHelpersTemplate(outputTarget));
            }
        }
    }
}
