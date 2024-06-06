using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace ProxyServiceTests.OriginalServices.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}