using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapImplyOptional
    {
        public MapImplyOptional()
        {
            Id = null!;
            Description = null!;
        }

        public string Id { get; set; }

        public string Description { get; set; }
    }
}