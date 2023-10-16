using System;
using CosmosDB.PrivateSetters.Domain;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class ClientDocument : IClientDocument, ICosmosDBDocument<Client, ClientDocument>
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
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Identifier { get; set; } = default!;
        [JsonProperty("@type")]
        public ClientType Type { get; set; }
        public string Name { get; set; } = default!;

        public Client ToEntity(Client? entity = default)
        {
            entity ??= new Client();

            ReflectionHelper.ForceSetProperty(entity, nameof(Identifier), Identifier ?? throw new Exception($"{nameof(entity.Identifier)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Type), Type);
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public ClientDocument PopulateFromEntity(Client entity)
        {
            Identifier = entity.Identifier;
            Type = entity.Type;
            Name = entity.Name;

            return this;
        }

        public static ClientDocument? FromEntity(Client? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClientDocument().PopulateFromEntity(entity);
        }
    }
}