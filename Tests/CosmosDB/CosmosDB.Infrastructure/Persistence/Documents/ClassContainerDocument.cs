using System;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class ClassContainerDocument : ClassContainer, ICosmosDBDocument<ClassContainerDocument, ClassContainer>
    {
        private string? _type;
        [JsonProperty("id")]
        public new string Id
        {
            get => base.Id ??= Guid.NewGuid().ToString();
            set => base.Id = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        string IItem.PartitionKey => ClassPartitionKey;

        public static ClassContainerDocument FromEntity(ClassContainer entity)
        {
            if (entity is ClassContainerDocument document)
            {
                return document;
            }

            return new ClassContainerDocument
            {
                Id = entity.Id,
                ClassPartitionKey = entity.ClassPartitionKey
            };
        }
    }
}