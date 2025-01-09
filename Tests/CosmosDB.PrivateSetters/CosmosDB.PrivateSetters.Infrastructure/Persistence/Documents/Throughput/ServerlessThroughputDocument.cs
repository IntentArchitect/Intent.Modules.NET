using System;
using CosmosDB.PrivateSetters.Domain.Entities.Throughput;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents.Throughput
{
    internal class ServerlessThroughputDocument : IServerlessThroughputDocument, ICosmosDBDocument<ServerlessThroughput, ServerlessThroughputDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;

        public ServerlessThroughput ToEntity(ServerlessThroughput? entity = default)
        {
            entity ??= new ServerlessThroughput();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));

            return entity;
        }

        public ServerlessThroughputDocument PopulateFromEntity(ServerlessThroughput entity)
        {
            Id = entity.Id;

            return this;
        }

        public static ServerlessThroughputDocument? FromEntity(ServerlessThroughput? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ServerlessThroughputDocument().PopulateFromEntity(entity);
        }
    }
}