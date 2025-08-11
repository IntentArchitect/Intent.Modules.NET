using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain;
using DynamoDbTests.PrivateSetters.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBDocument", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents
{
    [DynamoDBTable("clients")]
    internal class ClientDocument : IDynamoDBDocument<Client, ClientDocument>
    {
        [DynamoDBHashKey]
        public string Identifier { get; set; } = default!;
        public ClientType ClientType { get; set; }
        public string Name { get; set; } = default!;
        public bool IsDeleted { get; set; }
        [DynamoDBVersion]
        public int? Version { get; set; }

        public object GetKey() => Identifier;

        public int? GetVersion() => Version;

        public Client ToEntity(Client? entity = default)
        {
            entity ??= new Client();

            ReflectionHelper.ForceSetProperty(entity, nameof(Identifier), Identifier ?? throw new Exception($"{nameof(entity.Identifier)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(ClientType), ClientType);
            ReflectionHelper.ForceSetProperty(entity, nameof(Name), Name ?? throw new Exception($"{nameof(entity.Name)} is null"));
            ReflectionHelper.ForceSetProperty(entity, nameof(IsDeleted), IsDeleted);

            return entity;
        }

        public ClientDocument PopulateFromEntity(Client entity, Func<object, int?> getVersion)
        {
            Identifier = entity.Identifier;
            ClientType = entity.ClientType;
            Name = entity.Name;
            IsDeleted = entity.IsDeleted;
            Version ??= getVersion(GetKey());

            return this;
        }

        public static ClientDocument? FromEntity(Client? entity, Func<object, int?> getVersion)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClientDocument().PopulateFromEntity(entity, getVersion);
        }
    }
}