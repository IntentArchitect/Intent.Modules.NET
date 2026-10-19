using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Domain
{
    public class Schedule : ValueObject
    {
        public Schedule(string name, IEnumerable<ScheduleSlot> slots, ScheduleSlot? primarySlot)
        {
            Name = name;
            Slots = slots;
            PrimarySlot = primarySlot;
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return Slots;
            yield return PrimarySlot;
        }
    }
}