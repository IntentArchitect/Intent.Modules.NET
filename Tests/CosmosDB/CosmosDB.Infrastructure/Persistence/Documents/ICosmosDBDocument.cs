using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal interface ICosmosDBDocument<out TDocument, in TDomain> : IItem
        where TDocument : ICosmosDBDocument<TDocument, TDomain>
    {
        string IItem.PartitionKey => Id;
        TDocument PopulateFromEntity(TDomain entity);
    }
}