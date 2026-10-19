using CosmosDB.Domain;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    public class CoordinatesDocument : ICoordinatesDocument
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public Coordinates ToEntity(Coordinates? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Coordinates>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Latitude), Latitude);
            ReflectionHelper.ForceSetProperty(entity, nameof(Longitude), Longitude);

            return entity;
        }

        public CoordinatesDocument PopulateFromEntity(Coordinates entity)
        {
            Latitude = entity.Latitude;
            Longitude = entity.Longitude;

            return this;
        }

        public static CoordinatesDocument? FromEntity(Coordinates? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CoordinatesDocument().PopulateFromEntity(entity);
        }
    }
}