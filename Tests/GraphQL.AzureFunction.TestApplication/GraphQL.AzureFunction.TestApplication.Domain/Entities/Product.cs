using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace GraphQL.AzureFunction.TestApplication.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}