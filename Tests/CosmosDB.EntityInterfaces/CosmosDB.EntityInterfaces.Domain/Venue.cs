using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain
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
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return LocationAsAttribute;
            yield return Location;
            yield return Backup!;

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