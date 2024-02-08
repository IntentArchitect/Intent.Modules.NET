using System;
using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    internal class ProductDocument : IProductDocument
    {
        public string Name { get; set; } = default!;
        public List<CategoryDocument> Categories { get; set; } = default!;
        IReadOnlyList<ICategoryDocument> IProductDocument.Categories => Categories;

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Categories = Categories.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ProductDocument PopulateFromEntity(Product entity)
        {
            Name = entity.Name;
            Categories = entity.Categories.Select(x => CategoryDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ProductDocument? FromEntity(Product? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductDocument().PopulateFromEntity(entity);
        }
    }
}