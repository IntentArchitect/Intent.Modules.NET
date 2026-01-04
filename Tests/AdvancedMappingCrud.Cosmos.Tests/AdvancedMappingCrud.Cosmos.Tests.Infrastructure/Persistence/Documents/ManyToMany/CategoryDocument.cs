using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities.ManyToMany;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents.ManyToMany;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents.ManyToMany
{
    internal class CategoryDocument : ICategoryDocument, ICosmosDBDocument<Category, CategoryDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string Name { get; set; } = default!;

        public Category ToEntity(Category? entity = default)
        {
            entity ??= new Category();

            entity.Id = Guid.Parse(Id);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public CategoryDocument PopulateFromEntity(Category entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static CategoryDocument? FromEntity(Category? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new CategoryDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}