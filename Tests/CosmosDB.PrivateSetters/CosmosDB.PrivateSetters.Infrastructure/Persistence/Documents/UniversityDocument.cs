using System;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class UniversityDocument : IUniversityDocument, ICosmosDBDocument<University, UniversityDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; }
        public string Name { get; set; } = default!;

        public University ToEntity(University? entity = default)
        {
            entity ??= new University();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Guid.Parse(Id));
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public UniversityDocument PopulateFromEntity(University entity)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;

            return this;
        }

        public static UniversityDocument? FromEntity(University? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new UniversityDocument().PopulateFromEntity(entity);
        }
    }
}