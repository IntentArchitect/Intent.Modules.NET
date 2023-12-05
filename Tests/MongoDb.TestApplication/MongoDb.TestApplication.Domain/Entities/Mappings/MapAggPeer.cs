using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapAggPeer
    {
        public string Id { get; set; }

        public string PeerAtt { get; set; }

        public string MapAggPeerAggId { get; set; }

        public string MapMapMeId { get; set; }

        public MapPeerCompChild MapPeerCompChild { get; set; }
    }
}