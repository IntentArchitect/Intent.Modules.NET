using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AwsLambdaFunction.Domain.Entities
{
    public class EfDepartment
    {
        public EfDepartment()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid SiteId { get; set; }
    }
}