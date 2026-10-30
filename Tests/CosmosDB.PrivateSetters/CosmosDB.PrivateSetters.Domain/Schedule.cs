using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain
{
    public class Schedule : ValueObject
    {
        public Schedule(string name, IEnumerable<ScheduleSlot> slots, ScheduleSlot? primarySlot, Venue? primaryVenue, IEnumerable<Coordinates>? waypoints)
        {
            Name = name;
            Slots = slots;
            PrimarySlot = primarySlot;
            PrimaryVenue = primaryVenue;
            Waypoints = waypoints;
        }

        [IntentMerge]
        protected Schedule()
        {
            Name = null!;
            Slots = null!;
        }

        public string Name { get; private set; }
        public IEnumerable<ScheduleSlot> Slots { get; private set; }
        public ScheduleSlot? PrimarySlot { get; private set; }
        public Venue? PrimaryVenue { get; private set; }
        public IEnumerable<Coordinates>? Waypoints { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Collections are prefixed with their count so adjacent collections cannot run together
            yield return Name;
            yield return Slots.Count();

            foreach (var item in Slots)
            {
                yield return item;
            }
            yield return PrimarySlot!;
            yield return PrimaryVenue!;
            yield return Waypoints?.Count() ?? 0;

            if (Waypoints != null)
            {
                foreach (var item in Waypoints)
                {
                    yield return item;
                }
            }
        }
    }
}