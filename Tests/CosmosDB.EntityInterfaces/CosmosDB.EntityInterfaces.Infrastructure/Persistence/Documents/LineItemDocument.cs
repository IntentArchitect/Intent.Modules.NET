using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class LineItemDocument : ILineItemDocument
    {
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }

        public LineItem ToEntity(LineItem? entity = default)
        {
            entity ??= new LineItem();

            entity.Id = Id;
            entity.Description = Description;
            entity.Quantity = Quantity;

            return entity;
        }

        public LineItemDocument PopulateFromEntity(ILineItem entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            Quantity = entity.Quantity;

            return this;
        }

        public static LineItemDocument? FromEntity(ILineItem? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new LineItemDocument().PopulateFromEntity(entity);
        }
    }
}