using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Entities
{
    public class Tag
    {
        public Tag(string name)
        {
            Name = name;
        }

        public string Id { get; private set; }

        public string Name { get; private set; }
    }
}