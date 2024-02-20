using System.Collections.Generic;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;

internal interface IConsumerFactory
{
    IReadOnlyCollection<Consumer> CreateConsumers();
}