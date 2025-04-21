using Intent.Metadata.Models;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.Modules.SqlDatabaseProject.Templates;

namespace Intent.SqlDatabaseProject.Api;

public record SqlStoredProcedureModel : IMetadataModel
{
    public SqlStoredProcedureModel(StoredProcedureModel storedProcedureModel)
    {
        this.StoredProcedureModel = storedProcedureModel;
        this.Schema = storedProcedureModel.InternalElement.FindSchema() ?? "dbo";
    }
    
    public string Id => StoredProcedureModel.Id;
    
    public StoredProcedureModel StoredProcedureModel { get; init; }
    public string Schema { get; init; }
};