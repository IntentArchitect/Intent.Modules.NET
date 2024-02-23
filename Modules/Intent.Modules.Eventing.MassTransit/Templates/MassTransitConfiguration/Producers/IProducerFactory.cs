using System.Collections.Generic;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Producers;

public interface IProducerFactory
{
    IReadOnlyCollection<Producer> CreateProducers();
}