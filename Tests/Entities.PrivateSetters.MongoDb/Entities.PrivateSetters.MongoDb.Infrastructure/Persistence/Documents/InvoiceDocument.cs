using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Entities.PrivateSetters.MongoDb.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure.Persistence.Documents
{
    [BsonDiscriminator(nameof(Invoice), Required = true)]
    internal class InvoiceDocument : IInvoiceDocument, IMongoDbDocument<Invoice, InvoiceDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<string> TagsIds { get; set; }
        public IEnumerable<ILineDocument> Lines { get; set; }

        public Invoice ToEntity(Invoice? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Invoice>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Date), Date);
            ReflectionHelper.ForceSetProperty(entity, nameof(TagsIds), TagsIds.ToList() ?? throw new Exception($"{nameof(entity.TagsIds)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Lines), Lines.Select(x => (x as LineDocument).ToEntity()).ToList());

            return entity;
        }

        public InvoiceDocument PopulateFromEntity(Invoice entity)
        {
            Id = entity.Id;
            Date = entity.Date;
            TagsIds = entity.TagsIds.ToList();
            Lines = entity.Lines.Select(x => LineDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static InvoiceDocument? FromEntity(Invoice? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<InvoiceDocument> GetIdFilter(string id)
        {
            return Builders<InvoiceDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<InvoiceDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<InvoiceDocument> GetIdsFilter(string[] ids)
        {
            return Builders<InvoiceDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<InvoiceDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<InvoiceDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}