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
    internal class ParentDocument : IParentDocument, ICosmosDBDocument<Parent, ParentDocument>
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
        public List<ChildDocument>? Children { get; set; }
        IReadOnlyList<IChildDocument> IParentDocument.Children => Children;
        public ParentDetailsDocument? ParentDetails { get; set; }
        IParentDetailsDocument IParentDocument.ParentDetails => ParentDetails;

        public Parent ToEntity(Parent? entity = default)
        {
            entity ??= new Parent();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Children = Children?.Select(x => x.ToEntity()).ToList();
            entity.ParentDetails = ParentDetails?.ToEntity();

            return entity;
        }

        public ParentDocument PopulateFromEntity(Parent entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            Children = entity.Children?.Select(x => ChildDocument.FromEntity(x)!).ToList();
            ParentDetails = ParentDetailsDocument.FromEntity(entity.ParentDetails);

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ParentDocument? FromEntity(Parent? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ParentDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}