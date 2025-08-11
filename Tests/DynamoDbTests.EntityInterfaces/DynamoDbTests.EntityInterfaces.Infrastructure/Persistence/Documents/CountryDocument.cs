using DynamoDbTests.EntityInterfaces.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents
{
    internal class CountryDocument
    {
        public Guid Id { get; set; }
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