using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.NestedAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class Internode
    {
        public Guid Id { get; set; }

        public string InternodeAttribute { get; set; }
    }
}