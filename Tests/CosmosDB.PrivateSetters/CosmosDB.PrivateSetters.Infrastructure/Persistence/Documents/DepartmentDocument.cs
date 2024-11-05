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
    internal class DepartmentDocument : IDepartmentDocument, ICosmosDBDocument<Department, DepartmentDocument>
    {
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        public string Id { get; set; }
        public Guid? UniversityId { get; set; }
        public string Name { get; set; } = default!;

        public Department ToEntity(Department? entity = default)
        {
            entity ??= new Department();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Guid.Parse(Id));
            ReflectionHelper.ForceSetProperty(entity, nameof(UniversityId), UniversityId);
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public DepartmentDocument PopulateFromEntity(Department entity)
        {
            Id = entity.Id.ToString();
            UniversityId = entity.UniversityId;
            Name = entity.Name;

            return this;
        }

        public static DepartmentDocument? FromEntity(Department? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DepartmentDocument().PopulateFromEntity(entity);
        }
    }
}