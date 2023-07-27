using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class IntegrationTriggering : IHasDomainEvent
    {
        public IntegrationTriggering()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public IntegrationTriggering(string value)
        {
            Value = value;
        }

        public Guid Id { get; set; }

        public string Value { get; set; }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Update(string value)
        {
            Value = value;
        }
    }
}