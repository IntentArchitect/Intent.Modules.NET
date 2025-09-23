using System;
using System.Collections.Generic;
using System.Linq;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence.Documents
{
    internal class DynSiteDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<DynDepartmentDocument> Departments { get; set; } = default!;

        public DynSite ToEntity(DynSite? entity = default)
        {
            entity ??= new DynSite();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Departments = Departments.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public DynSiteDocument PopulateFromEntity(DynSite entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Departments = entity.Departments.Select(x => DynDepartmentDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static DynSiteDocument? FromEntity(DynSite? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DynSiteDocument().PopulateFromEntity(entity);
        }
    }
}