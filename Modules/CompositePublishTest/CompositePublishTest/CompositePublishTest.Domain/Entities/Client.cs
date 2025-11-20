using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CompositePublishTest.Domain.Entities
{
    public class Client
    {
        public Client()
        {
            Name = null!;
            Location = null!;
            Description = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }
    }
}