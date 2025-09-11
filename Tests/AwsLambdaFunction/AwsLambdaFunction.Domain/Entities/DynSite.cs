using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AwsLambdaFunction.Domain.Entities
{
    public class DynSite
    {
        private string? _id;

        public DynSite()
        {
            Id = null!;
            Name = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public ICollection<DynDepartment> Departments { get; set; } = [];
    }
}