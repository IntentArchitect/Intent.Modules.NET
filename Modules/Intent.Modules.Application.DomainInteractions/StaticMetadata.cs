using Intent.Metadata.Models;

namespace Intent.Modules.Application.DomainInteractions;

internal record StaticMetadata(string Id) : IMetadataModel;
