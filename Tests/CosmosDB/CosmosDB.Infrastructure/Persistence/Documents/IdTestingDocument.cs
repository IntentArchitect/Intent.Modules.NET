using System;
using System.Collections.Generic;
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
    internal class IdTestingDocument : IIdTestingDocument, ICosmosDBDocument<IdTesting, IdTestingDocument>
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
        [JsonProperty("@id")]
        public string Id { get; set; } = default!;

        public IdTesting ToEntity(IdTesting? entity = default)
        {
            entity ??= new IdTesting();

            entity.Identifier = Identifier;
            entity.Id = Id;

            return entity;
        }

        public IdTestingDocument PopulateFromEntity(IdTesting entity)
        {
            Identifier = entity.Identifier;
            Id = entity.Id;

            return this;
        }

        public static IdTestingDocument? FromEntity(IdTesting? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new IdTestingDocument().PopulateFromEntity(entity);
        }
    }
}