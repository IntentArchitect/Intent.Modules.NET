using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("regions")]
    internal class RegionDocument : IDynamoDBDocument<Region, RegionDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }
        public List<CountryDocument> Countries { get; set; } = default!;

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public Region ToEntity(Region? entity = default)
        {
            entity ??= new Region();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Countries), Countries.Select(x => x.ToEntity()).ToList());

            return entity;
        }

        public RegionDocument PopulateFromEntity(Region entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Name = entity.Name;
            Countries = entity.Countries.Select(x => CountryDocument.FromEntity(x)!).ToList();
            Version ??= getVersion(GetKey());

            return this;
        }

        public static RegionDocument? FromEntity(Region? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new RegionDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}