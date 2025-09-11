using System;
using Amazon.DynamoDBv2.DataModel;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("dyn-affiliates")]
    internal class DynAffiliateDocument : IDynamoDBDocument<DynAffiliate, DynAffiliateDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public DynAffiliate ToEntity(DynAffiliate? entity = default)
        {
            entity ??= new DynAffiliate();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public DynAffiliateDocument PopulateFromEntity(DynAffiliate entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Name = entity.Name;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static DynAffiliateDocument? FromEntity(DynAffiliate? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new DynAffiliateDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}