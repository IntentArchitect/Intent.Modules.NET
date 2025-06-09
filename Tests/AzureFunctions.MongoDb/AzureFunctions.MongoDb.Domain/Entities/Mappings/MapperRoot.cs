using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapperRoot
    {
        public MapperRoot()
        {
            Id = null!;
            No = null!;
            MapAggPeerId = null!;
            MapCompChild = null!;
        }

        public string Id { get; set; }

        public string No { get; set; }

        public IList<string> MapAggChildrenIds { get; set; } = [];

        public string MapAggPeerId { get; set; }

        public IList<string> MapperM2MSIds { get; set; } = [];

        public MapCompChild MapCompChild { get; set; }

        public MapCompOptional? MapCompOptional { get; set; }
    }
}