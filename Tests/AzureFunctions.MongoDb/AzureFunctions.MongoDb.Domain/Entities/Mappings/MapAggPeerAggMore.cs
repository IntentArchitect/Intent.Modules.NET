using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapAggPeerAggMore
    {
        public MapAggPeerAggMore()
        {
            Id = null!;
            MapAggPeerAggMoreAtt = null!;
        }

        public string Id { get; set; }

        public string MapAggPeerAggMoreAtt { get; set; }
    }
}