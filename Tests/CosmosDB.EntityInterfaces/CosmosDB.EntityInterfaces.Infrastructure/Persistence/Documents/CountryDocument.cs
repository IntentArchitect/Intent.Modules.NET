using System;
using System.Globalization;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class CountryDocument : ICountryDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public Country ToEntity(Country? entity = default)
        {
            entity ??= new Country();

            entity.Id = Id;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public CountryDocument PopulateFromEntity(ICountry entity)
        {
            Id = entity.Id;
            Name = entity.Name;

            return this;
        }

        public static CountryDocument? FromEntity(ICountry? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CountryDocument().PopulateFromEntity(entity);
        }
    }
}