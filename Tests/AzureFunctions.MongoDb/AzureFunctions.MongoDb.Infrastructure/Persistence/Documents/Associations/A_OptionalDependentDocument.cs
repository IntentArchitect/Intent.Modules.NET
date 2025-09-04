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
    internal class A_OptionalDependentDocument : IA_OptionalDependentDocument
    {
        public string OptDepAttribute { get; set; } = default!;

        public A_OptionalDependent ToEntity(A_OptionalDependent? entity = default)
        {
            entity ??= new A_OptionalDependent();

            entity.OptDepAttribute = OptDepAttribute ?? throw new Exception($"{nameof(entity.OptDepAttribute)} is null");

            return entity;
        }

        public A_OptionalDependentDocument PopulateFromEntity(A_OptionalDependent entity)
        {
            OptDepAttribute = entity.OptDepAttribute;

            return this;
        }

        public static A_OptionalDependentDocument? FromEntity(A_OptionalDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new A_OptionalDependentDocument().PopulateFromEntity(entity);
        }
    }
}