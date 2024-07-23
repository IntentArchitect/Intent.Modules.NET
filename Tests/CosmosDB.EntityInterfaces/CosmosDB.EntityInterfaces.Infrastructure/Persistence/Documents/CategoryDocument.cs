using System;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class CategoryDocument : ICategoryDocument, ICosmosDBDocument<ICategory, Category, CategoryDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        protected string? _etag;
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

        public CategoryDocument PopulateFromEntity(ICategory entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static CategoryDocument? FromEntity(ICategory? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new CategoryDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}