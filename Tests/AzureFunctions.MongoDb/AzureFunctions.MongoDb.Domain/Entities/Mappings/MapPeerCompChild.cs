using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities.Mappings
{
    public class MapPeerCompChild
    {
        public MapPeerCompChild()
        {
            PeerCompChildAtt = null!;
            MapPeerCompChildAggId = null!;
        }

        public string PeerCompChildAtt { get; set; }

        public string MapPeerCompChildAggId { get; set; }
    }
}