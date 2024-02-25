using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Decorators
{
    [Description(FactoriesForMassTransitConfiguration.DecoratorId)]
    public class FactoriesForMassTransitConfigurationRegistration : DecoratorRegistration<MassTransitConfigurationTemplate, MassTransitConfigurationDecoratorContract>
    {
        [IntentManaged(Mode.Fully)]
        public override MassTransitConfigurationDecoratorContract CreateDecoratorInstance(MassTransitConfigurationTemplate template, IApplication application)
        {
            return new FactoriesForMassTransitConfiguration(template, application);
        }

        public override string DecoratorId => FactoriesForMassTransitConfiguration.DecoratorId;
    }
}