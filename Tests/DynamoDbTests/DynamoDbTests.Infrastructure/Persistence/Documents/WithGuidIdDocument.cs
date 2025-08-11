using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("with-guid-ids")]
    internal class WithGuidIdDocument : IDynamoDBDocument<WithGuidId, WithGuidIdDocument>
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }
        public string Field { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public WithGuidId ToEntity(WithGuidId? entity = default)
        {
            entity ??= new WithGuidId();

            entity.Id = Id;
            entity.Field = Field ?? throw new Exception($"{nameof(entity.Field)} is null");

            return entity;
        }

        public WithGuidIdDocument PopulateFromEntity(WithGuidId entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Field = entity.Field;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static WithGuidIdDocument? FromEntity(WithGuidId? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithGuidIdDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}