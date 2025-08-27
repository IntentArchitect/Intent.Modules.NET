using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories.Documents.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Mappings
{
    internal class MapCompOptionalDocument : IMapCompOptionalDocument
    {
        public string Name { get; set; } = default!;
        public string MapImplyOptionalId { get; set; } = default!;

        public MapCompOptional ToEntity(MapCompOptional? entity = default)
        {
            entity ??= new MapCompOptional();

            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.MapImplyOptionalId = MapImplyOptionalId ?? throw new Exception($"{nameof(entity.MapImplyOptionalId)} is null");

            return entity;
        }

        public MapCompOptionalDocument PopulateFromEntity(MapCompOptional entity)
        {
            Name = entity.Name;
            MapImplyOptionalId = entity.MapImplyOptionalId;

            return this;
        }

        public static MapCompOptionalDocument? FromEntity(MapCompOptional? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapCompOptionalDocument().PopulateFromEntity(entity);
        }
    }
}