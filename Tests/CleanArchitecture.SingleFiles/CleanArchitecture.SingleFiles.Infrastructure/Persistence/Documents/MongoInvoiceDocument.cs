using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents
{
    [BsonDiscriminator(nameof(MongoInvoice), Required = true)]
    internal class MongoInvoiceDocument : IMongoInvoiceDocument, IMongoDbDocument<MongoInvoice, MongoInvoiceDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Description { get; set; }
        public IEnumerable<IMongoLineDocument> MongoLines { get; set; }

        public MongoInvoice ToEntity(MongoInvoice? entity = default)
        {
            entity ??= new MongoInvoice();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.MongoLines = MongoLines.Select(x => (x as MongoLineDocument).ToEntity()).ToList();

            return entity;
        }

        public MongoInvoiceDocument PopulateFromEntity(MongoInvoice entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            MongoLines = entity.MongoLines.Select(x => MongoLineDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static MongoInvoiceDocument? FromEntity(MongoInvoice? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MongoInvoiceDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<MongoInvoiceDocument> GetIdFilter(string id)
        {
            return Builders<MongoInvoiceDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<MongoInvoiceDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<MongoInvoiceDocument> GetIdsFilter(string[] ids)
        {
            return Builders<MongoInvoiceDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<MongoInvoiceDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<MongoInvoiceDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}