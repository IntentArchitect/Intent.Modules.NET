using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

internal record AggregateKeyMapping(AttributeModel Key, string? Match);