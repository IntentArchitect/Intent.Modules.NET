using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class ClassContainerDocument : IClassContainerDocument, ICosmosDBDocument<ClassContainer, ClassContainerDocument>
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

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id);
            ReflectionHelper.ForceSetProperty(entity, nameof(ClassPartitionKey), ClassPartitionKey);

            return entity;
        }

        public ClassContainerDocument PopulateFromEntity(ClassContainer entity)
        {
            Id = entity.Id;
            ClassPartitionKey = entity.ClassPartitionKey;

            return this;
        }

        public static ClassContainerDocument? FromEntity(ClassContainer? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClassContainerDocument().PopulateFromEntity(entity);
        }
    }
}