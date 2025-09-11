using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AwsLambdaFunction.Domain.Entities
{
    public class EfSite
    {
        public EfSite()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ClientId { get; set; }

        public virtual ICollection<EfDepartment> Departments { get; set; } = [];
    }
}