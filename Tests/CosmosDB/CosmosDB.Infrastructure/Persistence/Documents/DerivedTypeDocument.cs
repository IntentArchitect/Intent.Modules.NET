using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class DerivedTypeDocument : BaseTypeDocument, IDerivedTypeDocument, ICosmosDBDocument<DerivedType, DerivedTypeDocument>
    {
        public string DerivedTypeAggregateId { get; set; } = default!;
        public DerivedType ToEntity(DerivedType? entity = default)
        {
            entity ??= new DerivedType();

            entity.DerivedTypeAggregateId = DerivedTypeAggregateId;
            base.ToEntity(entity);

            return entity;
        }

        public DerivedTypeDocument PopulateFromEntity(DerivedType entity)
        {
            DerivedTypeAggregateId = entity.DerivedTypeAggregateId;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedTypeDocument? FromEntity(DerivedType? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeDocument().PopulateFromEntity(entity);
        }
    }
}