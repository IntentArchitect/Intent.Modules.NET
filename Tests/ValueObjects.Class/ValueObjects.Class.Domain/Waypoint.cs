using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace ValueObjects.Class.Domain
{
    public class Waypoint : ValueObject
    {
        public Waypoint(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        protected Waypoint()
        {
        }

        public decimal Latitude { get; private set; }
        public decimal Longitude { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Latitude;
            yield return Longitude;
        }
    }
}