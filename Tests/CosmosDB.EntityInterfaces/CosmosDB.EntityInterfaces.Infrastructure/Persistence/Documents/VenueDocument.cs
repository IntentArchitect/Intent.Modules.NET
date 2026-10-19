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
    public class VenueDocument : IVenueDocument
    {
        public string Name { get; set; } = default!;
        public CoordinatesDocument LocationAsAttribute { get; set; } = default!;
        ICoordinatesDocument IVenueDocument.LocationAsAttribute => LocationAsAttribute;
        public CoordinatesDocument Location { get; set; } = default!;
        ICoordinatesDocument IVenueDocument.Location => Location;
        public CoordinatesDocument? Backup { get; set; }
        ICoordinatesDocument? IVenueDocument.Backup => Backup;
        public List<CoordinatesDocument>? Entrances { get; set; }
        IReadOnlyList<ICoordinatesDocument>? IVenueDocument.Entrances => Entrances;

        public Venue ToEntity(Venue? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Venue>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(LocationAsAttribute), LocationAsAttribute.ToEntity() ?? throw new Exception($"{nameof(entity.LocationAsAttribute)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Location), Location.ToEntity() ?? throw new Exception($"{nameof(entity.Location)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Backup), Backup?.ToEntity());
            ReflectionHelper.ForceSetProperty(entity, nameof(Entrances), Entrances?.Select(x => x.ToEntity()).ToList());

            return entity;
        }

        public VenueDocument PopulateFromEntity(Venue entity)
        {
            Name = entity.Name;
            LocationAsAttribute = CoordinatesDocument.FromEntity(entity.LocationAsAttribute)!;
            Location = CoordinatesDocument.FromEntity(entity.Location)!;
            Backup = CoordinatesDocument.FromEntity(entity.Backup);
            Entrances = entity.Entrances?.Select(x => CoordinatesDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static VenueDocument? FromEntity(Venue? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VenueDocument().PopulateFromEntity(entity);
        }
    }
}