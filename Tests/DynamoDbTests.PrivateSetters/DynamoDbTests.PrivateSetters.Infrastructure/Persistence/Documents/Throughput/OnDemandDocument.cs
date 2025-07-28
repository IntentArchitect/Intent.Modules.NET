using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents.Throughput
{
    [DynamoDBTable("on-demands")]
    internal class OnDemandDocument : IDynamoDBDocument<OnDemand, OnDemandDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public OnDemand ToEntity(OnDemand? entity = default)
        {
            entity ??= new OnDemand();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));

            return entity;
        }

        public OnDemandDocument PopulateFromEntity(OnDemand entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static OnDemandDocument? FromEntity(OnDemand? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new OnDemandDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}