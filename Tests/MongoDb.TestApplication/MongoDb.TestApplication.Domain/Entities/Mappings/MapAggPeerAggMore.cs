using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapAggPeerAggMore
    {
        [IntentManaged(Mode.Fully)]
        public MapAggPeerAggMore()
        {
            Id = null!;
            MapAggPeerAggMoreAtt = null!;
        }
        public string Id { get; set; }

        public string MapAggPeerAggMoreAtt { get; set; }
    }
}