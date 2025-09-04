using System;
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
    [BsonDiscriminator(nameof(C_MultipleDependent), Required = true)]
    internal class C_MultipleDependentDocument : IC_MultipleDependentDocument
    {
        public string Id { get; set; } = default!;
        public string Attribute { get; set; } = default!;

        public C_MultipleDependent ToEntity(C_MultipleDependent? entity = default)
        {
            entity ??= new C_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");

            return entity;
        }

        public C_MultipleDependentDocument PopulateFromEntity(C_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;

            return this;
        }

        public static C_MultipleDependentDocument? FromEntity(C_MultipleDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new C_MultipleDependentDocument().PopulateFromEntity(entity);
        }
    }
}