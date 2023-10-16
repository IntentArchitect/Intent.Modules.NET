using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class BaseTypeDocument : IBaseTypeDocument, ICosmosDBDocument<IBaseType, BaseType, BaseTypeDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;

        public BaseType ToEntity(BaseType? entity = default)
        {
            entity ??= new BaseType();

            entity.Id = Id;

            return entity;
        }

        public BaseTypeDocument PopulateFromEntity(IBaseType entity)
        {
            Id = entity.Id;

            return this;
        }

        public static BaseTypeDocument? FromEntity(IBaseType? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new BaseTypeDocument().PopulateFromEntity(entity);
        }
    }
}