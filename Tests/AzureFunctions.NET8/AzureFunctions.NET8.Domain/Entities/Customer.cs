using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.NET8.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}