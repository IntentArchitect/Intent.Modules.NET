using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    public class MapCompOptional
    {
        public string Name { get; set; }

        public string MapImplyOptionalId { get; set; }
    }
}