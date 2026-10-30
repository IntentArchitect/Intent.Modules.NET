using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain
{
    public class Itinerary : ValueObject
    {
        public Itinerary(IEnumerable<Coordinates> legs, IEnumerable<Coordinates>? extraLegs)
        {
            Legs = legs;
            ExtraLegs = extraLegs;
        }

        [IntentMerge]
        protected Itinerary()
        {
            Legs = null!;
        }

        public IEnumerable<Coordinates> Legs { get; private set; }
        public IEnumerable<Coordinates>? ExtraLegs { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Collections are prefixed with their count so adjacent collections cannot run together
            yield return Legs.Count();

            foreach (var item in Legs)
            {
                yield return item;
            }
            yield return ExtraLegs?.Count() ?? 0;

            if (ExtraLegs != null)
            {
                foreach (var item in ExtraLegs)
                {
                    yield return item;
                }
            }
        }
    }
}