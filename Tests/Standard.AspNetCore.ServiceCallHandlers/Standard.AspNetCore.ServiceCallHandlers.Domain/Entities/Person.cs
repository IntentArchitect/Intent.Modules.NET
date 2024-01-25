using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Standard.AspNetCore.ServiceCallHandlers.Domain.Entities
{
    public class Person
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}