using System;
using System.Collections.Generic;
using CosmosDB.Domain;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class ClientDocument : IClientDocument, ICosmosDBDocument<Client, ClientDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        protected string? _etag;
        [JsonProperty("id")]
        string IItem.Id
        {
            get => Identifier;
            set => Identifier = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Identifier { get; set; } = default!;
        public ClientType ClientType { get; set; }
        public string Name { get; set; } = default!;
        public bool IsDeleted { get; set; }

        public Client ToEntity(Client? entity = default)
        {
            entity ??= new Client();

            entity.Identifier = Identifier ?? throw new Exception($"{nameof(entity.Identifier)} is null");
            entity.ClientType = ClientType;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.IsDeleted = IsDeleted;

            return entity;
        }

        public ClientDocument PopulateFromEntity(Client entity, Func<string, string?> getEtag)
        {
            Identifier = entity.Identifier;
            ClientType = entity.ClientType;
            Name = entity.Name;
            IsDeleted = entity.IsDeleted;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ClientDocument? FromEntity(Client? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClientDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}
