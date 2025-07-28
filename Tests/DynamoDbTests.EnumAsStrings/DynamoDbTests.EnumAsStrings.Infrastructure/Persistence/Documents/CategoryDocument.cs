using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("categories")]
    internal class CategoryDocument : IDynamoDBDocument<Category, CategoryDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public Category ToEntity(Category? entity = default)
        {
            entity ??= new Category();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public CategoryDocument PopulateFromEntity(Category entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Name = entity.Name;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static CategoryDocument? FromEntity(Category? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new CategoryDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}