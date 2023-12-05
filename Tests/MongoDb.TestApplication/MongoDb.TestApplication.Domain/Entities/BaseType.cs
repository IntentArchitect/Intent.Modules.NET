using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities
{
    public abstract class BaseType
    {
        public string Id { get; set; }

        public string BaseAttribute { get; set; }
    }
}