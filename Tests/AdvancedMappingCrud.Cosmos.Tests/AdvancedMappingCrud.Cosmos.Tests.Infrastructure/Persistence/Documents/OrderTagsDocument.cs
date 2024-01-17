using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    public class OrderTagsDocument : IOrderTagsDocument
    {
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;

        public OrderTags ToEntity(OrderTags? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<OrderTags>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Value), Value ?? throw new Exception($"{nameof(entity.Value)} is null"));

            return entity;
        }

        public OrderTagsDocument PopulateFromEntity(OrderTags entity)
        {
            Name = entity.Name;
            Value = entity.Value;

            return this;
        }

        public static OrderTagsDocument? FromEntity(OrderTags? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderTagsDocument().PopulateFromEntity(entity);
        }
    }
}