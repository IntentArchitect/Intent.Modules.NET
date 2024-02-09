using System;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class CategoryDocument : ICategoryDocument, ICosmosDBDocument<Category, CategoryDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        private string? _etag;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;

        public Category ToEntity(Category? entity = default)
        {
            entity ??= new Category();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public CategoryDocument PopulateFromEntity(Category entity, string? etag = null)
        {
            Id = entity.Id;
            Name = entity.Name;

            _etag = etag;

            return this;
        }

        public static CategoryDocument? FromEntity(Category? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new CategoryDocument().PopulateFromEntity(entity, etag);
        }
    }
}