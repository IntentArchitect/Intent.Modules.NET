using System;
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
    internal class MongoLineDocument : IMongoLineDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public MongoLine ToEntity(MongoLine? entity = default)
        {
            entity ??= new MongoLine();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public MongoLineDocument PopulateFromEntity(MongoLine entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static MongoLineDocument? FromEntity(MongoLine? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MongoLineDocument().PopulateFromEntity(entity);
        }
    }
}