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
    internal class G_MultipleDependentDocument : IG_MultipleDependentDocument
    {
        public string Id { get; set; } = default!;
        public string Attribute { get; set; } = default!;
        public IG_RequiredCompositeNavDocument G_RequiredCompositeNav { get; set; } = default!;

        public G_MultipleDependent ToEntity(G_MultipleDependent? entity = default)
        {
            entity ??= new G_MultipleDependent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Attribute = Attribute ?? throw new Exception($"{nameof(entity.Attribute)} is null");
            entity.G_RequiredCompositeNav = (G_RequiredCompositeNav as G_RequiredCompositeNavDocument).ToEntity() ?? throw new Exception($"{nameof(entity.G_RequiredCompositeNav)} is null");

            return entity;
        }

        public G_MultipleDependentDocument PopulateFromEntity(G_MultipleDependent entity)
        {
            Id = entity.Id;
            Attribute = entity.Attribute;
            G_RequiredCompositeNav = G_RequiredCompositeNavDocument.FromEntity(entity.G_RequiredCompositeNav)!;

            return this;
        }

        public static G_MultipleDependentDocument? FromEntity(G_MultipleDependent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new G_MultipleDependentDocument().PopulateFromEntity(entity);
        }
    }
}