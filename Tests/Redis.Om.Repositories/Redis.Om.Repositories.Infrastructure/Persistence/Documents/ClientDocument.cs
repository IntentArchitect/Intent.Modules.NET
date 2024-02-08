using System;
using Intent.RoslynWeaver.Attributes;
using Redis.OM.Modeling;
using Redis.Om.Repositories.Domain;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocument", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    [Document(StorageType = StorageType.Json, Prefixes = new[] { "Client" })]
    internal class ClientDocument : IClientDocument, IRedisOmDocument<Client, ClientDocument>
    {
        [RedisIdField]
        [Indexed]
        public string Id { get; set; } = default!;
        public ClientType Type { get; set; }
        [Indexed]
        public string Name { get; set; } = default!;

        public Client ToEntity(Client? entity = default)
        {
            entity ??= new Client();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Type = Type;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");

            return entity;
        }

        public ClientDocument PopulateFromEntity(Client entity)
        {
            Id = entity.Id;
            Type = entity.Type;
            Name = entity.Name;

            return this;
        }

        public static ClientDocument? FromEntity(Client? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClientDocument().PopulateFromEntity(entity);
        }
    }
}