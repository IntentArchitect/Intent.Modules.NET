using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class DerivedOfTDocument : BaseOfTDocument<int>, IDerivedOfTDocument, ICosmosDBDocument<IDerivedOfT, DerivedOfT, DerivedOfTDocument>
    {
        public string DerivedAttribute { get; set; } = default!;

        public DerivedOfT ToEntity(DerivedOfT? entity = default)
        {
            entity ??= new DerivedOfT();

            entity.DerivedAttribute = DerivedAttribute;
            base.ToEntity(entity);

            return entity;
        }

        public DerivedOfTDocument PopulateFromEntity(IDerivedOfT entity)
        {
            DerivedAttribute = entity.DerivedAttribute;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedOfTDocument? FromEntity(IDerivedOfT? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedOfTDocument().PopulateFromEntity(entity);
        }
    }
}