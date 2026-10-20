using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace ValueObjects.Record.Domain
{
    public record Route(string Name,
        Waypoint OneToOne,
        Waypoint? OneToZeroOrOne,
        IEnumerable<Waypoint> OneToMany,
        Waypoint ManyToOne,
        Waypoint? ManyToZeroOrOne,
        IEnumerable<Waypoint> ManyToMany,
        Waypoint ZeroOrOneToOne,
        IEnumerable<Waypoint> ZeroOrOneToMany,
        Waypoint? ZeroOrOneToZeroOrOne);

}