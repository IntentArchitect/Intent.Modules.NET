using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TextIndexEntity
    {
        [IntentManaged(Mode.Fully)]
        public TextIndexEntity()
        {
            Id = null!;
            FullText = null!;
            SomeField = null!;
        }
        public string Id { get; set; }

        public string FullText { get; set; }

        public string SomeField { get; set; }
    }
}