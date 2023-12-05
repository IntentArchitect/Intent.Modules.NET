using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapAggPeerAgg
    {
        public string Id { get; set; }

        public string MapAggPeerAggAtt { get; set; }

        public string MapAggPeerAggMoreId { get; set; }
    }
}