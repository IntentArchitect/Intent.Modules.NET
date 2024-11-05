using System;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class DepartmentDocument : IDepartmentDocument, ICosmosDBDocument<IDepartment, Department, DepartmentDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public Guid? UniversityId { get; set; }
        public string Name { get; set; } = default!;

        public Department ToEntity(Department? entity = default)
        {
            entity ??= new Department();

            entity.Id = Guid.Parse(Id);
            entity.UniversityId = UniversityId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public DepartmentDocument PopulateFromEntity(IDepartment entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            UniversityId = entity.UniversityId;
            Name = entity.Name;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static DepartmentDocument? FromEntity(IDepartment? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new DepartmentDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}