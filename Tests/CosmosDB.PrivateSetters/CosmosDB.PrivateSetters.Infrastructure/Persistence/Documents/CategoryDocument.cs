using System;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class CategoryDocument : ICategoryDocument, ICosmosDBDocument<Category, CategoryDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public Category ToEntity(Category? entity = default)
        {
            entity ??= new Category();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public CategoryDocument PopulateFromEntity(Category entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static CategoryDocument? FromEntity(Category? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CategoryDocument().PopulateFromEntity(entity);
        }
    }
}