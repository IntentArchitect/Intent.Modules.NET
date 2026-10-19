using System;
using System.Collections.Generic;
using System.Linq;
using CosmosDB.Domain;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    public class ItineraryDocument : IItineraryDocument
    {
        public List<CoordinatesDocument> Legs { get; set; } = default!;
        IReadOnlyList<ICoordinatesDocument> IItineraryDocument.Legs => Legs;
        public List<CoordinatesDocument>? ExtraLegs { get; set; }
        IReadOnlyList<ICoordinatesDocument>? IItineraryDocument.ExtraLegs => ExtraLegs;

        public Itinerary ToEntity(Itinerary? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Itinerary>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Legs), Legs.Select(x => x.ToEntity()).ToList() ?? throw new Exception($"{nameof(entity.Legs)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(ExtraLegs), ExtraLegs?.Select(x => x.ToEntity()).ToList());

            return entity;
        }

        public ItineraryDocument PopulateFromEntity(Itinerary entity)
        {
            Legs = entity.Legs.Select(x => CoordinatesDocument.FromEntity(x)!).ToList();
            ExtraLegs = entity.ExtraLegs?.Select(x => CoordinatesDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ItineraryDocument? FromEntity(Itinerary? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ItineraryDocument().PopulateFromEntity(entity);
        }
    }
}