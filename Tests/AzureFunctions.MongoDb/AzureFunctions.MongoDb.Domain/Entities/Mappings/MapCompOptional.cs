using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapCompOptional
    {
        public MapCompOptional()
        {
            Name = null!;
            MapImplyOptionalId = null!;
        }

        public string Name { get; set; }

        public string MapImplyOptionalId { get; set; }
    }
}