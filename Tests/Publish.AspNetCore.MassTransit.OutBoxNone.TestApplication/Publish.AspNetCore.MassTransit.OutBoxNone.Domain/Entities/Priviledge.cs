using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Priviledge
    {
        [IntentManaged(Mode.Fully)]
        public Priviledge()
        {
            Name = null!;
        }
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public string Name { get; set; }
    }
}