using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace ValueObjects.Class.Domain
{
    public class Route : ValueObject
    {
        public Route(string name,
            Waypoint oneToOne,
            Waypoint? oneToZeroOrOne,
            IEnumerable<Waypoint> oneToMany,
            Waypoint manyToOne,
            Waypoint? manyToZeroOrOne,
            IEnumerable<Waypoint> manyToMany,
            Waypoint zeroOrOneToOne,
            IEnumerable<Waypoint> zeroOrOneToMany,
            Waypoint? zeroOrOneToZeroOrOne)
        {
            Name = name;
            OneToOne = oneToOne;
            OneToZeroOrOne = oneToZeroOrOne;
            OneToMany = oneToMany;
            ManyToOne = manyToOne;
            ManyToZeroOrOne = manyToZeroOrOne;
            ManyToMany = manyToMany;
            ZeroOrOneToOne = zeroOrOneToOne;
            ZeroOrOneToMany = zeroOrOneToMany;
            ZeroOrOneToZeroOrOne = zeroOrOneToZeroOrOne;
        }

        [IntentMerge]
        protected Route()
        {
            Name = null!;
            OneToOne = null!;
            OneToMany = null!;
            ManyToOne = null!;
            ManyToMany = null!;
            ZeroOrOneToOne = null!;
            ZeroOrOneToMany = null!;
        }

        public string Name { get; private set; }
        public Waypoint OneToOne { get; private set; }
        public Waypoint? OneToZeroOrOne { get; private set; }
        public IEnumerable<Waypoint> OneToMany { get; private set; }
        public Waypoint ManyToOne { get; private set; }
        public Waypoint? ManyToZeroOrOne { get; private set; }
        public IEnumerable<Waypoint> ManyToMany { get; private set; }
        public Waypoint ZeroOrOneToOne { get; private set; }
        public IEnumerable<Waypoint> ZeroOrOneToMany { get; private set; }
        public Waypoint? ZeroOrOneToZeroOrOne { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Collections are prefixed with their count so adjacent collections cannot run together
            yield return Name;
            yield return OneToOne;
            yield return OneToZeroOrOne!;
            yield return OneToMany.Count();

            foreach (var item in OneToMany)
            {
                yield return item;
            }
            yield return ManyToOne;
            yield return ManyToZeroOrOne!;
            yield return ManyToMany.Count();

            foreach (var item in ManyToMany)
            {
                yield return item;
            }
            yield return ZeroOrOneToOne;
            yield return ZeroOrOneToMany.Count();

            foreach (var item in ZeroOrOneToMany)
            {
                yield return item;
            }
            yield return ZeroOrOneToZeroOrOne!;
        }
    }
}