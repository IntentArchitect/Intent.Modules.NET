using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("departments")]
    internal class DepartmentDocument : IDynamoDBDocument<IDepartment, Department, DepartmentDocument>
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; }
        public Guid? UniversityId { get; set; }
        public string Name { get; set; } = default!;
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public Department ToEntity(Department? entity = default)
        {
            entity ??= new Department();

            entity.Id = Id;
            entity.UniversityId = UniversityId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public DepartmentDocument PopulateFromEntity(IDepartment entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            UniversityId = entity.UniversityId;
            Name = entity.Name;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static DepartmentDocument? FromEntity(IDepartment? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new DepartmentDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}