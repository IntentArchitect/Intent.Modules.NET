using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("customers")]
    internal class CustomerDocument : IDynamoDBDocument<Customer, CustomerDocument>
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<string>? Tags { get; set; }
        [DynamoDBVersion]
        public int? Version { get; set; }
        public AddressDocument DeliveryAddress { get; set; } = default!;
        public AddressDocument? BillingAddress { get; set; }

        public object GetKey() => Id;

        public int? GetVersion() => Version;

        public Customer ToEntity(Customer? entity = default)
        {
            entity ??= new Customer();

            ReflectionHelper.ForceSetProperty(entity, nameof(Id), Id ?? throw new Exception($"{nameof(entity.Id)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(Tags), Tags);
            ReflectionHelper.ForceSetProperty(entity, nameof(DeliveryAddress), DeliveryAddress.ToEntity() ?? throw new Exception($"{nameof(entity.DeliveryAddress)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(BillingAddress), BillingAddress?.ToEntity());

            return entity;
        }

        public CustomerDocument PopulateFromEntity(Customer entity, Func<object, int?> getVersion)
        {
            Id = entity.Id;
            Name = entity.Name;
            Tags = entity.Tags?.ToList();
            DeliveryAddress = AddressDocument.FromEntity(entity.DeliveryAddress)!;
            BillingAddress = AddressDocument.FromEntity(entity.BillingAddress);
            Version ??= getVersion(GetKey());

            return this;
        }

        public static CustomerDocument? FromEntity(Customer? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new CustomerDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}