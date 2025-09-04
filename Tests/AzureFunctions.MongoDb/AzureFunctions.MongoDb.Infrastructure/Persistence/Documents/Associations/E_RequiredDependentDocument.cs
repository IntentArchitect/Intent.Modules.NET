using System;
using System.Linq;
using System.Linq.Expressions;
using AzureFunctions.MongoDb.Domain.Entities.Associations;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Associations;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Persistence.Documents.Associations
{
    [BsonDiscriminator(nameof(E_RequiredDependent), Required = true)]
    internal class E_RequiredDependentDocument : IE_RequiredDependentDocument
    {
        public string Attribute { get; set; } = default!;
        public IE_RequiredCompositeNavDocument E_RequiredCompositeNav { get; set; } = default!;

        public E_RequiredDependent ToEntity(E_RequiredDependent? entity = default)
        {
            entity ??= new E_RequiredDependent();

            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.E_RequiredCompositeNav = (E_RequiredCompositeNav as E_RequiredCompositeNavDocument).ToEntity() ?? throw new Exception($"{nameof(entity.E_RequiredCompositeNav)} is null");

            return entity;
        }

        public E_RequiredDependentDocument PopulateFromEntity(E_RequiredDependent entity)
        {
            Attribute = entity.Attribute;
            E_RequiredCompositeNav = E_RequiredCompositeNavDocument.FromEntity(entity.E_RequiredCompositeNav)!;

            return this;
        }

        public static E_RequiredDependentDocument? FromEntity(E_RequiredDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new E_RequiredDependentDocument().PopulateFromEntity(entity);
        }
    }
}