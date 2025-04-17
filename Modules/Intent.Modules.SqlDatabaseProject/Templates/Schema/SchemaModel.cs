using Intent.Metadata.Models;

namespace Intent.SqlDatabaseProject.Api;

public record SchemaModel(string Name) : IMetadataModel
{
    public string Id => Name;
}