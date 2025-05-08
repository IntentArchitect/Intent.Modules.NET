using System;
using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    internal class EmbeddedParentDocument : IEmbeddedParentDocument, ICosmosDBDocument<EmbeddedParent, EmbeddedParentDocument>
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
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<EmbeddedChildDocument> Children { get; set; } = default!;
        IReadOnlyList<IEmbeddedChildDocument> IEmbeddedParentDocument.Children => Children;

        public EmbeddedParent ToEntity(EmbeddedParent? entity = default)
        {
            entity ??= new EmbeddedParent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Children = Children.Select(x => x.ToEntity()).ToList() ?? throw new Exception($"{nameof(entity.Children)} is null");

            return entity;
        }

        public EmbeddedParentDocument PopulateFromEntity(EmbeddedParent entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            Children = entity.Children.Select(x => EmbeddedChildDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static EmbeddedParentDocument? FromEntity(EmbeddedParent? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new EmbeddedParentDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}