using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapCompChild
    {
        [IntentManaged(Mode.Fully)]
        public MapCompChild()
        {
            CompChildAtt = null!;
            MapCompChildAggId = null!;
            MapCompChildAgg = null!;
        }

        public string CompChildAtt { get; set; }

        public string MapCompChildAggId { get; set; }
    }
}