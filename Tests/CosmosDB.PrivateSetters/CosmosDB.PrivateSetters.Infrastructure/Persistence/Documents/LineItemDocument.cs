using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class LineItemDocument : ILineItemDocument
    {
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }

        public LineItem ToEntity(LineItem? entity = default)
        {
            entity ??= new LineItem();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id);
            ReflectionHelper.ForceSetProperty(entity, nameof(Description), Description);
            ReflectionHelper.ForceSetProperty(entity, nameof(Quantity), Quantity);

            return entity;
        }

        public LineItemDocument PopulateFromEntity(LineItem entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            Quantity = entity.Quantity;

            return this;
        }

        public static LineItemDocument? FromEntity(LineItem? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new LineItemDocument().PopulateFromEntity(entity);
        }
    }
}