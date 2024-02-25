using System.Collections.Generic;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Consumers;

public interface IConsumerFactory
{
    IReadOnlyCollection<Consumer> CreateConsumers();
}