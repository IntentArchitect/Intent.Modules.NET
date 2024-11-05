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
    internal class UniversityDocument : IUniversityDocument, ICosmosDBDocument<IUniversity, University, UniversityDocument>
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

        public University ToEntity(University? entity = default)
        {
            entity ??= new University();

            entity.Id = Guid.Parse(Id);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public UniversityDocument PopulateFromEntity(IUniversity entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static UniversityDocument? FromEntity(IUniversity? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new UniversityDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}