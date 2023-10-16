using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class DerivedOfTDocument : BaseOfTDocument<int>, IDerivedOfTDocument, ICosmosDBDocument<DerivedOfT, DerivedOfTDocument>
    {
        public string DerivedAttribute { get; set; } = default!;

        public DerivedOfT ToEntity(DerivedOfT? entity = default)
        {
            entity ??= new DerivedOfT();

            ReflectionHelper.ForceSetProperty(entity, nameof(DerivedAttribute), DerivedAttribute);
            base.ToEntity(entity);

            return entity;
        }

        public DerivedOfTDocument PopulateFromEntity(DerivedOfT entity)
        {
            DerivedAttribute = entity.DerivedAttribute;
            base.PopulateFromEntity(entity);

            return this;
        }

        public static DerivedOfTDocument? FromEntity(DerivedOfT? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedOfTDocument().PopulateFromEntity(entity);
        }
    }
}