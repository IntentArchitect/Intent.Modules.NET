using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBValueObjectDocument", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Persistence.Documents
{
    public class AddressDocument : IAddressDocument
    {
        public string Line1 { get; set; } = default!;
        public string Line2 { get; set; } = default!;
        public string City { get; set; } = default!;
        public string PostalCode { get; set; } = default!;

        public Address ToEntity(Address? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<Address>();

            ReflectionHelper.ForceSetProperty(entity, nameof(Line1), Line1 ?? throw new Exception($"{nameof(entity.Line1)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Line2), Line2 ?? throw new Exception($"{nameof(entity.Line2)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(City), City ?? throw new Exception($"{nameof(entity.City)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(PostalCode), PostalCode ?? throw new Exception($"{nameof(entity.PostalCode)} is null"));

            return entity;
        }

        public AddressDocument PopulateFromEntity(Address entity)
        {
            Line1 = entity.Line1;
            Line2 = entity.Line2;
            City = entity.City;
            PostalCode = entity.PostalCode;

            return this;
        }

        public static AddressDocument? FromEntity(Address? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AddressDocument().PopulateFromEntity(entity);
        }
    }
}