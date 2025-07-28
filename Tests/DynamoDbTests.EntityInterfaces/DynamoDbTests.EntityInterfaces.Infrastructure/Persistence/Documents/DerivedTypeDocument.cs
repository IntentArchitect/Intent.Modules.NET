using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("derived-types")]
    internal class DerivedTypeDocument : BaseTypeDocument, IDynamoDBDocument<IDerivedType, DerivedType, DerivedTypeDocument>
    {
        public string DerivedTypeAggregateId { get; set; } = default!;

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public DerivedType ToEntity(DerivedType? entity = default)
        {
            entity ??= new DerivedType();

            entity.DerivedTypeAggregateId = DerivedTypeAggregateId ?? throw new Exception($"{nameof(entity.DerivedTypeAggregateId)} is null");
            base.ToEntity(entity);

            return entity;
        }

        public DerivedTypeDocument PopulateFromEntity(IDerivedType entity, Func<object, int?> getVersion)
        {
            DerivedTypeAggregateId = entity.DerivedTypeAggregateId;
            base.PopulateFromEntity(entity, getVersion);

            return this;
        }

        public static DerivedTypeDocument? FromEntity(IDerivedType? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new DerivedTypeDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}