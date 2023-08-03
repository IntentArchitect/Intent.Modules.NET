using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapperRoot
    {
        [IntentManaged(Mode.Fully)]
        public MapperRoot()
        {
            Id = null!;
            No = null!;
            MapAggChildrenIds = null!;
            MapAggPeerId = null!;
            MapCompChild = null!;
            MapAggPeer = null!;
        }
        public string Id { get; set; }

        public string No { get; set; }

        public ICollection<string> MapAggChildrenIds { get; set; } = new List<string>();

        public string MapAggPeerId { get; set; }

        public virtual MapCompChild MapCompChild { get; set; }
    }
}