using System;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDb.TestApplication.Domain.Entities.Associations;
using MongoDb.TestApplication.Domain.Repositories.Documents.Associations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace MongoDb.TestApplication.Infrastructure.Persistence.Documents.Associations
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