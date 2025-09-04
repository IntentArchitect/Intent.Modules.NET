using System;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Mappings;
using MongoDb.TestApplication.Domain.Repositories.Documents.Mappings;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Mappings
{
    [BsonDiscriminator(nameof(MapPeerCompChild), Required = true)]
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