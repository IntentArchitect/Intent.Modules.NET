using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.EntityInterfaces.Domain;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    public class ScheduleDocument : IScheduleDocument
    {
        public string Name { get; set; } = default!;
        public List<ScheduleSlotDocument> Slots { get; set; } = default!;
        IReadOnlyList<IScheduleSlotDocument> IScheduleDocument.Slots => Slots;
        public ScheduleSlotDocument? PrimarySlot { get; set; }
        IScheduleSlotDocument? IScheduleDocument.PrimarySlot => PrimarySlot;

        public Schedule ToEntity(Schedule? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Schedule>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Slots), Slots.Select(x => x.ToEntity()).ToList() ?? throw new Exception($"{nameof(entity.Slots)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(PrimarySlot), PrimarySlot?.ToEntity());

            return entity;
        }

        public ScheduleDocument PopulateFromEntity(Schedule entity)
        {
            Name = entity.Name;
            Slots = entity.Slots.Select(x => ScheduleSlotDocument.FromEntity(x)!).ToList();
            PrimarySlot = ScheduleSlotDocument.FromEntity(entity.PrimarySlot);

            return this;
        }

        public static ScheduleDocument? FromEntity(Schedule? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ScheduleDocument().PopulateFromEntity(entity);
        }
    }
}