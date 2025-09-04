using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
{
    [BsonDiscriminator(nameof(H_OptionalAggregateNav), Required = true)]
    internal class H_OptionalAggregateNavDocument : IH_OptionalAggregateNavDocument, IMongoDbDocument<H_OptionalAggregateNav, H_OptionalAggregateNavDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Attribute { get; set; }
        public IEnumerable<string> HMultipledependentsIds { get; set; }

        public H_OptionalAggregateNav ToEntity(H_OptionalAggregateNav? entity = default)
        {
            entity ??= new H_OptionalAggregateNav();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.HMultipledependentsIds = HMultipledependentsIds.ToList() ?? throw new Exception($"{nameof(entity.HMultipledependentsIds)} is null");

            return entity;
        }

        public H_OptionalAggregateNavDocument PopulateFromEntity(H_OptionalAggregateNav entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            HMultipledependentsIds = entity.HMultipledependentsIds.ToList();

            return this;
        }

        public static H_OptionalAggregateNavDocument? FromEntity(H_OptionalAggregateNav? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new H_OptionalAggregateNavDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<H_OptionalAggregateNavDocument> GetIdFilter(string id)
        {
            return Builders<H_OptionalAggregateNavDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<H_OptionalAggregateNavDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<H_OptionalAggregateNavDocument> GetIdsFilter(string[] ids)
        {
            return Builders<H_OptionalAggregateNavDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<H_OptionalAggregateNavDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<H_OptionalAggregateNavDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}