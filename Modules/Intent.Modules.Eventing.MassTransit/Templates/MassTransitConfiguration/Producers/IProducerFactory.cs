using System.Collections.Generic;

namespace Intent.Modules.Eventing.MassTransit.Templates.MassTransitConfiguration.Producers;

internal interface IProducerFactory
{
    IReadOnlyCollection<Producer> CreateProducers();
}