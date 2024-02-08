using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Redis.OM.Modeling;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    internal class LineItemDocument : ILineItemDocument
    {
        [Indexed]
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }
        public List<string> Tags { get; set; } = default!;
        IReadOnlyList<string> ILineItemDocument.Tags => Tags;
        public ProductDocument Product { get; set; } = default!;
        IProductDocument ILineItemDocument.Product => Product;

        public LineItem ToEntity(LineItem? entity = default)
        {
            entity ??= new LineItem();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Quantity = Quantity;
            entity.Tags = Tags ?? throw new Exception($"{nameof(entity.Tags)} is null");
            entity.Product = Product.ToEntity() ?? throw new Exception($"{nameof(entity.Product)} is null");

            return entity;
        }

        public LineItemDocument PopulateFromEntity(LineItem entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            Quantity = entity.Quantity;
            Tags = entity.Tags.ToList();
            Product = ProductDocument.FromEntity(entity.Product)!;

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