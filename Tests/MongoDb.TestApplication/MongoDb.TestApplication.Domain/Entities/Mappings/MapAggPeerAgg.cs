using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    public class MapAggPeerAgg
    {
        public MapAggPeerAgg()
        {
            Id = null!;
            MapAggPeerAggAtt = null!;
            MapAggPeerAggMoreId = null!;
        }

        public string Id { get; set; }

        public string MapAggPeerAggAtt { get; set; }

        public string MapAggPeerAggMoreId { get; set; }
    }
}