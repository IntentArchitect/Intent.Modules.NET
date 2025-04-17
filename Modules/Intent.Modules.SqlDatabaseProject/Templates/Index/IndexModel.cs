using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Metadata.RDBMS.Api.Indexes;

namespace Intent.SqlDatabaseProject.Api;

public record IndexModel(Index Index, ClassModel ClassModel) : IMetadataModel
{
    public string Id => $"{ClassModel.Name}.{Index.Name}";
}