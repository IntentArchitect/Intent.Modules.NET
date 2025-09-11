using System;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence.Documents
{
    internal class DynDepartmentDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public DynDepartment ToEntity(DynDepartment? entity = default)
        {
            entity ??= new DynDepartment();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public DynDepartmentDocument PopulateFromEntity(DynDepartment entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static DynDepartmentDocument? FromEntity(DynDepartment? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DynDepartmentDocument().PopulateFromEntity(entity);
        }
    }
}