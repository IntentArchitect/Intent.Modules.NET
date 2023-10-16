using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class DerivedTypeDocument : BaseTypeDocument, IDerivedTypeDocument, ICosmosDBDocument<IDerivedType, DerivedType, DerivedTypeDocument>
    {
        public string DerivedTypeAggregateId { get; set; } = default!;

        public DerivedType ToEntity(DerivedType? entity = default)
        {
            entity ??= new DerivedType();

            entity.DerivedTypeAggregateId = DerivedTypeAggregateId;
            base.ToEntity(entity);

            return entity;
        }

        public DerivedTypeDocument PopulateFromEntity(IDerivedType entity)
        {
            DerivedTypeAggregateId = entity.DerivedTypeAggregateId;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedTypeDocument? FromEntity(IDerivedType? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeDocument().PopulateFromEntity(entity);
        }
    }
}