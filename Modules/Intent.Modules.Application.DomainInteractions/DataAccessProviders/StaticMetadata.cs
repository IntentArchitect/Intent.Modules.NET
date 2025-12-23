using Intent.Metadata.Models;

namespace Intent.Modules.Application.DomainInteractions.DataAccessProviders;

internal record StaticMetadata(string Id) : IMetadataModel;
