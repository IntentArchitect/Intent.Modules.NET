using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Mappings
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class MapAggChild
    {
        public string Id { get; set; }

        public string ChildName { get; set; }
    }
}