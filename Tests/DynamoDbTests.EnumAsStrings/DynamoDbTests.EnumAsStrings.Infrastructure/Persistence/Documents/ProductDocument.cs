using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("products")]
    internal class ProductDocument : IDynamoDBDocument<Product, ProductDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<string> CategoriesIds { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.CategoriesIds = CategoriesIds ?? throw new Exception($"{nameof(entity.CategoriesIds)} is null");

            return entity;
        }

        public ProductDocument PopulateFromEntity(Product entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Name = entity.Name;
            CategoriesIds = entity.CategoriesIds.ToList();
            Version ??= getVersion(GetKey());

            return this;
        }

        public static ProductDocument? FromEntity(Product? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}