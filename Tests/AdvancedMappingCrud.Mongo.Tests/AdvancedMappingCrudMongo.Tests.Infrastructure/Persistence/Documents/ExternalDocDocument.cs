using System;
using System.Linq;
using System.Linq.Expressions;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Documents
{
    [BsonDiscriminator(nameof(ExternalDoc), Required = true)]
    internal class ExternalDocDocument : IExternalDocDocument, IMongoDbDocument<ExternalDoc, ExternalDocDocument, long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Thing { get; set; }

        public ExternalDoc ToEntity(ExternalDoc? entity = default)
        {
            entity ??= new ExternalDoc();

            entity.Id = Id;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Thing = Thing ?? throw new Exception($"{nameof(entity.Thing)} is null");

            return entity;
        }

        public ExternalDocDocument PopulateFromEntity(ExternalDoc entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Thing = entity.Thing;

            return this;
        }

        public static ExternalDocDocument? FromEntity(ExternalDoc? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ExternalDocDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<ExternalDocDocument> GetIdFilter(long id)
        {
            return Builders<ExternalDocDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<ExternalDocDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<ExternalDocDocument> GetIdsFilter(long[] ids)
        {
            return Builders<ExternalDocDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<ExternalDocDocument, bool>> GetIdFilterPredicate(long id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<ExternalDocDocument, bool>> GetIdsFilterPredicate(long[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}