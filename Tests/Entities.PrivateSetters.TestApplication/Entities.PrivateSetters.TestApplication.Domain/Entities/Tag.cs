using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities
{
    public class Tag
    {
        public Tag()
        {
            Name = null!;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }
    }
}