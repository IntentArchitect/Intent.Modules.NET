using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents
{
    internal interface ICosmosDBDocument<TDomain, out TDocument> : ICosmosDBDocument
        where TDomain : class
        where TDocument : ICosmosDBDocument<TDomain, TDocument>
    {
        TDocument PopulateFromEntity(TDomain entity);
        TDomain ToEntity(TDomain? entity = null);
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