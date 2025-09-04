using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Mappings
{
    [BsonDiscriminator(nameof(MapCompChild), Required = true)]
    internal class MapCompChildDocument : IMapCompChildDocument
    {
        public string CompChildAtt { get; set; } = default!;
        public string MapCompChildAggId { get; set; } = default!;

        public MapCompChild ToEntity(MapCompChild? entity = default)
        {
            entity ??= new MapCompChild();

            entity.CompChildAtt = CompChildAtt ?? throw new Exception($"{nameof(entity.CompChildAtt)} is null");
            entity.MapCompChildAggId = MapCompChildAggId ?? throw new Exception($"{nameof(entity.MapCompChildAggId)} is null");

            return entity;
        }

        public MapCompChildDocument PopulateFromEntity(MapCompChild entity)
        {
            CompChildAtt = entity.CompChildAtt;
            MapCompChildAggId = entity.MapCompChildAggId;

            return this;
        }

        public static MapCompChildDocument? FromEntity(MapCompChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapCompChildDocument().PopulateFromEntity(entity);
        }
    }
}