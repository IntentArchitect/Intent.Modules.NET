using System;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Entities.PrivateSetters.MongoDb.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Tag = Entities.PrivateSetters.MongoDb.Domain.Entities.Tag;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure.Persistence.Documents
{
    internal class TagDocument : ITagDocument, IMongoDbDocument<Tag, TagDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public Tag ToEntity(Tag? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Tag>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public TagDocument PopulateFromEntity(Tag entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static TagDocument? FromEntity(Tag? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TagDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<TagDocument> GetIdFilter(string id)
        {
            return Builders<TagDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<TagDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<TagDocument> GetIdsFilter(string[] ids)
        {
            return Builders<TagDocument>.Filter.In(d => d.Id, ids);
        }
    }
}