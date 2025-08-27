using System;
using AzureFunctions.MongoDb.Domain.Entities.Mappings;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Mappings;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Mappings
{
    internal class MapPeerCompChildDocument : IMapPeerCompChildDocument
    {
        public string PeerCompChildAtt { get; set; } = default!;
        public string MapPeerCompChildAggId { get; set; } = default!;

        public MapPeerCompChild ToEntity(MapPeerCompChild? entity = default)
        {
            entity ??= new MapPeerCompChild();

            entity.PeerCompChildAtt = PeerCompChildAtt ?? throw new Exception($"{nameof(entity.PeerCompChildAtt)} is null");
            entity.MapPeerCompChildAggId = MapPeerCompChildAggId ?? throw new Exception($"{nameof(entity.MapPeerCompChildAggId)} is null");

            return entity;
        }

        public MapPeerCompChildDocument PopulateFromEntity(MapPeerCompChild entity)
        {
            PeerCompChildAtt = entity.PeerCompChildAtt;
            MapPeerCompChildAggId = entity.MapPeerCompChildAggId;

            return this;
        }

        public static MapPeerCompChildDocument? FromEntity(MapPeerCompChild? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MapPeerCompChildDocument().PopulateFromEntity(entity);
        }
    }
}