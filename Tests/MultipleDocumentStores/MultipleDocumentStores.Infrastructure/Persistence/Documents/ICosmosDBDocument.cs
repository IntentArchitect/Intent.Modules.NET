using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Persistence.Documents
{
    internal interface ICosmosDBDocument<out TDocument, in TDomain> : ICosmosDBDocument
        where TDocument : ICosmosDBDocument<TDocument, TDomain>
    {
        TDocument PopulateFromEntity(TDomain entity);
    }

    internal interface ICosmosDBDocument : IItem
    {
        string IItem.PartitionKey => PartitionKey!;
        new string? PartitionKey
        {
            get => Id;
            set => Id = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}