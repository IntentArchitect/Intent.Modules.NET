using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapCompChild
    {
        public MapCompChild()
        {
            CompChildAtt = null!;
            MapCompChildAggId = null!;
        }

        public string CompChildAtt { get; set; }

        public string MapCompChildAggId { get; set; }
    }
}