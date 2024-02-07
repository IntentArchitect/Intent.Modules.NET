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
        private string? etag;
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
        string? IItemWithEtag.Etag => etag;
        public string Identifier { get; set; } = default!;
        [JsonProperty("@type")]
        public ClientType Type { get; set; }
        public string Name { get; set; } = default!;

        public Client ToEntity(Client? entity = default)
        {
            entity ??= new Client();

            entity.Identifier = Identifier ?? throw new Exception($"{nameof(entity.Identifier)} is null");
            entity.Type = Type;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public ClientDocument PopulateFromEntity(Client entity, string? etag = null)
        {
            Identifier = entity.Identifier;
            Type = entity.Type;
            Name = entity.Name;

            this.etag = etag;

            return this;
        }

        public static ClientDocument? FromEntity(Client? entity, string? etag = null)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClientDocument().PopulateFromEntity(entity, etag);
        }
    }
}