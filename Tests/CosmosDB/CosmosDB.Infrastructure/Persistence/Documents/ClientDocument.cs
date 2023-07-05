using System;
using CosmosDB.Domain;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class ClientDocument : Client, ICosmosDBDocument<ClientDocument, Client>
    {
        private string? _type;
        [JsonProperty("id")]
        string IItem.Id
        {
            get => Identifier;
            set => Identifier = value;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().Name;
            set => _type = value;
        }
        [JsonProperty("@type")]
        public new ClientType Type
        {
            get => base.Type;
            set => base.Type = value;
        }

        public ClientDocument PopulateFromEntity(Client entity)
        {
            Identifier = entity.Identifier;
            Type = entity.Type;
            Name = entity.Name;

            return this;
        }
    }
}