using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    public class MapImplyOptional
    {
        public string Id { get; set; }

        public string Description { get; set; }
    }
}