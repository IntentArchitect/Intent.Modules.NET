using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain
{
    public class Venue : ValueObject
    {
        public Venue(string name,
            Coordinates locationAsAttribute,
            Coordinates location,
            Coordinates? backup,
            IEnumerable<Coordinates>? entrances)
        {
            Name = name;
            LocationAsAttribute = locationAsAttribute;
            Location = location;
            Backup = backup;
            Entrances = entrances;
        }

        [IntentMerge]
        protected Venue()
        {
            Name = null!;
            LocationAsAttribute = null!;
            Location = null!;
        }

        public string Name { get; private set; }
        public Coordinates LocationAsAttribute { get; private set; }
        public Coordinates Location { get; private set; }
        public Coordinates? Backup { get; private set; }
        public IEnumerable<Coordinates>? Entrances { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Collections are prefixed with their count so adjacent collections cannot run together
            yield return Name;
            yield return LocationAsAttribute;
            yield return Location;
            yield return Backup!;
            yield return Entrances?.Count() ?? 0;

            if (Entrances != null)
            {
                foreach (var item in Entrances)
                {
                    yield return item;
                }
            }
        }
    }
}