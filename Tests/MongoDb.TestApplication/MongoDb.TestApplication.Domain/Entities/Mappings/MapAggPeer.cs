using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    public class MapAggPeer
    {
        public MapAggPeer()
        {
            Id = null!;
            PeerAtt = null!;
            MapAggPeerAggId = null!;
            MapMapMeId = null!;
            MapPeerCompChild = null!;
        }

        public string Id { get; set; }

        public string PeerAtt { get; set; }

        public string MapAggPeerAggId { get; set; }

        public string MapMapMeId { get; set; }

        public MapPeerCompChild MapPeerCompChild { get; set; }
    }
}