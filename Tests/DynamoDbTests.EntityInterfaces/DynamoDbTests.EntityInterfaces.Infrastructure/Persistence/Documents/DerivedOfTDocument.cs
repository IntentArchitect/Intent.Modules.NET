using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("derived-of-ts")]
    internal class DerivedOfTDocument : BaseOfTDocument<int>, IDynamoDBDocument<IDerivedOfT, DerivedOfT, DerivedOfTDocument>
    {
        public string DerivedAttribute { get; set; } = default!;

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public DerivedOfT ToEntity(DerivedOfT? entity = default)
        {
            entity ??= new DerivedOfT();

            entity.DerivedAttribute = DerivedAttribute ?? throw new Exception($"{nameof(entity.DerivedAttribute)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedOfTDocument PopulateFromEntity(IDerivedOfT entity, Func<object, int?> getVersion)
        {
            DerivedAttribute = entity.DerivedAttribute;
            base.PopulateFromEntity(entity, getVersion);

            return this;
        }

        public static DerivedOfTDocument? FromEntity(IDerivedOfT? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedOfTDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}