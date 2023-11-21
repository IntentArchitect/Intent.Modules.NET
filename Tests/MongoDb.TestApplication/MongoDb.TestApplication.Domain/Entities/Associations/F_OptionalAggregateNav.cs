using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class F_OptionalAggregateNav
    {
        public string Id { get; set; }

        public string Attribute { get; set; }

        public string? FOptionaldependentId { get; set; }
    }
}