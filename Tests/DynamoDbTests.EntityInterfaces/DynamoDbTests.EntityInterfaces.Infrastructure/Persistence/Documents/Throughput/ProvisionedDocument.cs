using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents.Throughput
{
    [DynamoDBTable("provisioneds")]
    internal class ProvisionedDocument : IDynamoDBDocument<IProvisioned, Provisioned, ProvisionedDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public Provisioned ToEntity(Provisioned? entity = default)
        {
            entity ??= new Provisioned();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");

            return entity;
        }

        public ProvisionedDocument PopulateFromEntity(IProvisioned entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static ProvisionedDocument? FromEntity(IProvisioned? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProvisionedDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}