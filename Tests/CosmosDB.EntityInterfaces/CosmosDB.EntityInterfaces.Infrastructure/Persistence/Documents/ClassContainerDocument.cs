using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class ClassContainerDocument : IClassContainerDocument, ICosmosDBDocument<IClassContainer, ClassContainer, ClassContainerDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? ICosmosDBDocument.PartitionKey
        {
            get => ClassPartitionKey;
            set => ClassPartitionKey = value!;
        }
        public string Id { get; set; } = default!;
        public string ClassPartitionKey { get; set; } = default!;

        public ClassContainer ToEntity(ClassContainer? entity = default)
        {
            entity ??= new ClassContainer();

            entity.Id = Id;
            entity.ClassPartitionKey = ClassPartitionKey;

            return entity;
        }

        public ClassContainerDocument PopulateFromEntity(IClassContainer entity)
        {
            Id = entity.Id;
            ClassPartitionKey = entity.ClassPartitionKey;

            return this;
        }

        public static ClassContainerDocument? FromEntity(IClassContainer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClassContainerDocument().PopulateFromEntity(entity);
        }
    }
}