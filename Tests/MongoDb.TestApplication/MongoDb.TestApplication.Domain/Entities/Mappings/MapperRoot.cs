using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapperRoot
    {
        public string Id { get; set; }

        public string No { get; set; }

        public ICollection<string> MapAggChildrenIds { get; set; } = new List<string>();

        public string MapAggPeerId { get; set; }

        public MapCompChild MapCompChild { get; set; }
    }
}