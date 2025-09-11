using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("dyn-clients")]
    internal class DynClientDocument : IDynamoDBDocument<DynClient, DynClientDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string AffiliateId { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }
        public List<DynSiteDocument> Sites { get; set; } = default!;

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public DynClient ToEntity(DynClient? entity = default)
        {
            entity ??= new DynClient();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.AffiliateId = AffiliateId ?? throw new Exception($"{nameof(entity.AffiliateId)} is null");
            entity.Sites = Sites.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public DynClientDocument PopulateFromEntity(DynClient entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Name = entity.Name;
            AffiliateId = entity.AffiliateId;
            Sites = entity.Sites.Select(x => DynSiteDocument.FromEntity(x)!).ToList();
            Version ??= getVersion(GetKey());

            return this;
        }

        public static DynClientDocument? FromEntity(DynClient? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new DynClientDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}