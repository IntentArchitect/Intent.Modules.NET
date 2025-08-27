using System;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Entities.PrivateSetters.MongoDb.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure.Persistence.Documents
{
    internal class LineDocument : ILineDocument
    {
        public string Id { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Quantity { get; set; }

        public Line ToEntity(Line? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Line>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Description), Description ?? throw new Exception($"{nameof(entity.Description)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Quantity), Quantity);

            return entity;
        }

        public LineDocument PopulateFromEntity(Line entity)
        {
            Id = entity.Id;
            Description = entity.Description;
            Quantity = entity.Quantity;

            return this;
        }

        public static LineDocument? FromEntity(Line? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new LineDocument().PopulateFromEntity(entity);
        }
    }
}