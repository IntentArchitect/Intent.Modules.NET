using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("id-testings")]
    internal class IdTestingDocument : IDynamoDBDocument<IdTesting, IdTestingDocument>
    {
        [DynamoDBHashKey]
        public string Identifier { get; set; } = default!;
        public string Id { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Identifier;

        public int? GetVersion() => Version;

        public IdTesting ToEntity(IdTesting? entity = default)
        {
            entity ??= new IdTesting();

            entity.Identifier = Identifier ?? throw new Exception($"{nameof(entity.Identifier)} is null");
            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public IdTestingDocument PopulateFromEntity(IdTesting entity, Func<object, int?> getVersion)
        {
            Identifier = entity.Identifier;
            Id = entity.Id;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static IdTestingDocument? FromEntity(IdTesting? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new IdTestingDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}