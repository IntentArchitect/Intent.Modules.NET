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
    internal class BaseTypeDocument : IBaseTypeDocument, ICosmosDBDocument<BaseType, BaseTypeDocument>
    {
        private string? _type;
        public string Id { get; set; } = default!;

        public BaseType ToEntity(BaseType? entity = default)
        {
            entity ??= new BaseType();

            entity.Id = Id;

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }

        public BaseTypeDocument PopulateFromEntity(BaseType entity)
        {
            Id = entity.Id;

            return this;
        }

        public static BaseTypeDocument? FromEntity(BaseType? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new BaseTypeDocument().PopulateFromEntity(entity);
        }
    }
}