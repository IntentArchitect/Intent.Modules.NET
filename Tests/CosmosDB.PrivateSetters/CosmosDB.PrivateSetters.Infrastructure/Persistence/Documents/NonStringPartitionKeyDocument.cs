using System;
using System.Globalization;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class NonStringPartitionKeyDocument : INonStringPartitionKeyDocument, ICosmosDBDocument<NonStringPartitionKey, NonStringPartitionKeyDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? ICosmosDBDocument.PartitionKey
        {
            get => PartInt;
            set => PartInt = value!;
        }
        public string Id { get; set; } = default!;
        public string PartInt { get; set; }
        public string Name { get; set; } = default!;

        public NonStringPartitionKey ToEntity(NonStringPartitionKey? entity = default)
        {
            entity ??= new NonStringPartitionKey();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(PartInt), int.Parse(PartInt, CultureInfo.InvariantCulture));
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public NonStringPartitionKeyDocument PopulateFromEntity(NonStringPartitionKey entity)
        {
            Id = entity.Id;
            PartInt = entity.PartInt.ToString(CultureInfo.InvariantCulture);
            Name = entity.Name;

            return this;
        }

        public static NonStringPartitionKeyDocument? FromEntity(NonStringPartitionKey? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new NonStringPartitionKeyDocument().PopulateFromEntity(entity);
        }
    }
}