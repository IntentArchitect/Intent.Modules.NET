using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Eventing.MassTransit.RequestResponse.ConfigurationFactories;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Producers;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class FactoriesForMassTransitConfiguration : MassTransitConfigurationDecoratorContract
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Eventing.MassTransit.RequestResponse.FactoriesForMassTransitConfiguration";

        [IntentManaged(Mode.Fully)]
        private readonly MassTransitConfigurationTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public FactoriesForMassTransitConfiguration(MassTransitConfigurationTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IReadOnlyCollection<IProducerFactory> GetProducerFactories()
        {
            return new[] { new CommandQueryProducerFactory(_template) };
        }

        public override IReadOnlyCollection<IConsumerFactory> GetConsumerFactories()
        {
            return new[] { new MediatRConsumerFactory(_template) };
        }
    }
}