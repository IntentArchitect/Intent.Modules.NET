using System;
using CosmosDB.Domain;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    public class ScheduleSlotDocument : IScheduleSlotDocument
    {
        public string Name { get; set; } = default!;

        public ScheduleSlot ToEntity(ScheduleSlot? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<ScheduleSlot>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public ScheduleSlotDocument PopulateFromEntity(ScheduleSlot entity)
        {
            Name = entity.Name;

            return this;
        }

        public static ScheduleSlotDocument? FromEntity(ScheduleSlot? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ScheduleSlotDocument().PopulateFromEntity(entity);
        }
    }
}