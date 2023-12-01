using System;
using System.Collections.Generic;
using System.Linq;
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
        public string ProductId { get; set; } = default!;
        public List<string> Tags { get; set; } = default!;
        IReadOnlyList<string> ILineItemDocument.Tags => Tags;

        public LineItem ToEntity(LineItem? entity = default)
        {
            entity ??= new LineItem();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Description), Description ?? throw new Exception($"{nameof(entity.Description)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Quantity), Quantity);
            ReflectionHelper.ForceSetProperty(entity, nameof(ProductId), ProductId ?? throw new Exception($"{nameof(entity.ProductId)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Tags), Tags ?? throw new Exception($"{nameof(entity.Tags)} is null"));

            return entity;
        }

        public LineItemDocument PopulateFromEntity(LineItem entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            Quantity = entity.Quantity;
            ProductId = entity.ProductId;
            Tags = entity.Tags.ToList();

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