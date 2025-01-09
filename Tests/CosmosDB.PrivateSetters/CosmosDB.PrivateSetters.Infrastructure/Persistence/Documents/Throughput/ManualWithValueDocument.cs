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
    internal class ManualWithValueDocument : IManualWithValueDocument, ICosmosDBDocument<ManualWithValue, ManualWithValueDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; } = default!;

        public ManualWithValue ToEntity(ManualWithValue? entity = default)
        {
            entity ??= new ManualWithValue();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));

            return entity;
        }

        public ManualWithValueDocument PopulateFromEntity(ManualWithValue entity)
        {
            Id = entity.Id;

            return this;
        }

        public static ManualWithValueDocument? FromEntity(ManualWithValue? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ManualWithValueDocument().PopulateFromEntity(entity);
        }
    }
}