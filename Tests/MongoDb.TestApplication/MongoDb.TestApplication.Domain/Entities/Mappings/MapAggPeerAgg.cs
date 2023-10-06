using System;
using Intent.RoslynWeaver.Attributes;

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