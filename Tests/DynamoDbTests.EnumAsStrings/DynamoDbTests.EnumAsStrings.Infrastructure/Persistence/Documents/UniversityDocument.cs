using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("universities")]
    internal class UniversityDocument : IDynamoDBDocument<University, UniversityDocument>
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public University ToEntity(University? entity = default)
        {
            entity ??= new University();

            entity.Id = Id;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public UniversityDocument PopulateFromEntity(University entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Name = entity.Name;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static UniversityDocument? FromEntity(University? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new UniversityDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}