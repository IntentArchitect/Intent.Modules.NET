using System;
using System.Globalization;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Infrastructure.Persistence.Documents
{
    internal class CountryDocument : ICountryDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public Country ToEntity(Country? entity = default)
        {
            entity ??= new Country();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id);
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));

            return entity;
        }

        public CountryDocument PopulateFromEntity(Country entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static CountryDocument? FromEntity(Country? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CountryDocument().PopulateFromEntity(entity);
        }
    }
}