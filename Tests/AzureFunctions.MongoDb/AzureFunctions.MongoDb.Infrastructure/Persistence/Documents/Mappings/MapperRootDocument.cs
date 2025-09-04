using System;
using System.Collections.Generic;
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
    internal class MapperRootDocument : IMapperRootDocument, IMongoDbDocument<MapperRoot, MapperRootDocument, string>
    {
        [BsonId]
        public string Id { get; set; }
        public string No { get; set; }
        public IEnumerable<string> MapAggChildrenIds { get; set; }
        public string MapAggPeerId { get; set; }
        public IEnumerable<string> MapperM2MSIds { get; set; }
        public IMapCompChildDocument MapCompChild { get; set; }
        public IMapCompOptionalDocument? MapCompOptional { get; set; }

        public MapperRoot ToEntity(MapperRoot? entity = default)
        {
            entity ??= new MapperRoot();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.No = No ?? throw new Exception($"{nameof(entity.No)} is null");
            entity.MapAggChildrenIds = MapAggChildrenIds.ToList() ?? throw new Exception($"{nameof(entity.MapAggChildrenIds)} is null");
            entity.MapAggPeerId = MapAggPeerId ?? throw new Exception($"{nameof(entity.MapAggPeerId)} is null");
            entity.MapperM2MSIds = MapperM2MSIds.ToList() ?? throw new Exception($"{nameof(entity.MapperM2MSIds)} is null");
            entity.MapCompChild = (MapCompChild as MapCompChildDocument).ToEntity() ?? throw new Exception($"{nameof(entity.MapCompChild)} is null");
            entity.MapCompOptional = (MapCompOptional as MapCompOptionalDocument)?.ToEntity();

            return entity;
        }

        public MapperRootDocument PopulateFromEntity(MapperRoot entity)
        {
            Id = entity.Id;
            No = entity.No;
            MapAggChildrenIds = entity.MapAggChildrenIds.ToList();
            MapAggPeerId = entity.MapAggPeerId;
            MapperM2MSIds = entity.MapperM2MSIds.ToList();
            MapCompChild = MapCompChildDocument.FromEntity(entity.MapCompChild)!;
            MapCompOptional = MapCompOptionalDocument.FromEntity(entity.MapCompOptional);

            return this;
        }

        public static MapperRootDocument? FromEntity(MapperRoot? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapperRootDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MapperRootDocument> GetIdFilter(string id)
        {
            return Builders<MapperRootDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MapperRootDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MapperRootDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MapperRootDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MapperRootDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MapperRootDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}