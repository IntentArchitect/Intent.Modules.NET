using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace IntegrationTesting.Tests.Domain.Entities
{
    public class CNCCChild
    {
        public CNCCChild()
        {
            Description = null!;
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public Guid CheckNewCompChildCrudId { get; set; }
    }
}