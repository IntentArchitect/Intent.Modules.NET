using System;
using System.Collections.Generic;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;
using Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Producers;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class MassTransitConfigurationDecoratorContract : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual IReadOnlyCollection<IConsumerFactory> GetConsumerFactories()
        {
            return ArraySegment<IConsumerFactory>.Empty;
        }

        public virtual IReadOnlyCollection<IProducerFactory> GetProducerFactories()
        {
            return ArraySegment<IProducerFactory>.Empty;
        }
    }
}