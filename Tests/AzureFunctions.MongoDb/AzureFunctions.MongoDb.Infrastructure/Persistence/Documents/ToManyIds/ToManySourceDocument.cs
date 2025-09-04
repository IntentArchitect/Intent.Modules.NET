using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.ToManyIds;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.ToManyIds
{
    [BsonDiscriminator(nameof(ToManySource), Required = true)]
    internal class ToManySourceDocument : IToManySourceDocument, IMongoDbDocument<ToManySource, ToManySourceDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public IEnumerable<IToManyGuidDocument> ToManyGuids { get; set; }
        public IEnumerable<IToManyIntDocument> ToManyInts { get; set; }
        public IEnumerable<IToManyLongDocument> ToManyLongs { get; set; }

        public ToManySource ToEntity(ToManySource? entity = default)
        {
            entity ??= new ToManySource();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ToManyGuids = ToManyGuids.Select(x => (x as ToManyGuidDocument).ToEntity()).ToList();
            entity.ToManyInts = ToManyInts.Select(x => (x as ToManyIntDocument).ToEntity()).ToList();
            entity.ToManyLongs = ToManyLongs.Select(x => (x as ToManyLongDocument).ToEntity()).ToList();

            return entity;
        }

        public ToManySourceDocument PopulateFromEntity(ToManySource entity)
        {
            Id = entity.Id;
            ToManyGuids = entity.ToManyGuids.Select(x => ToManyGuidDocument.FromEntity(x)!).ToList();
            ToManyInts = entity.ToManyInts.Select(x => ToManyIntDocument.FromEntity(x)!).ToList();
            ToManyLongs = entity.ToManyLongs.Select(x => ToManyLongDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ToManySourceDocument? FromEntity(ToManySource? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ToManySourceDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<ToManySourceDocument> GetIdFilter(string id)
        {
            return Builders<ToManySourceDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<ToManySourceDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<ToManySourceDocument> GetIdsFilter(string[] ids)
        {
            return Builders<ToManySourceDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<ToManySourceDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<ToManySourceDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}