using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapAggPeer
    {
        [IntentManaged(Mode.Fully)]
        public MapAggPeer()
        {
            Id = null!;
            PeerAtt = null!;
            MapAggPeerAggId = null!;
            MapMapMeId = null!;
            MapPeerCompChild = null!;
            MapAggPeerAgg = null!;
            MapMapMe = null!;
        }
        public string Id { get; set; }

        public string PeerAtt { get; set; }

        public string MapAggPeerAggId { get; set; }

        public string MapMapMeId { get; set; }

        public virtual MapPeerCompChild MapPeerCompChild { get; set; }
    }
}