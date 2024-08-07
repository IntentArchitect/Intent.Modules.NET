using System;
using BasicAuditing.CustomUserId.Tests.Domain.Entities;
using BasicAuditing.CustomUserId.Tests.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace BasicAuditing.CustomUserId.Tests.Infrastructure.Persistence.Documents
{
    internal class AccountDocument : IAccountDocument, ICosmosDBDocument<Account, AccountDocument>
    {
        private string? _type;
        [JsonProperty("_etag")]
        protected string? _etag;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }

        public Account ToEntity(Account? entity = default)
        {
            entity ??= new Account();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.CreatedBy = CreatedBy;
            entity.CreatedDate = CreatedDate;
            entity.UpdatedBy = UpdatedBy;
            entity.UpdatedDate = UpdatedDate;

            return entity;
        }

        public AccountDocument PopulateFromEntity(Account entity, Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            UpdatedBy = entity.UpdatedBy;
            UpdatedDate = entity.UpdatedDate;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static AccountDocument? FromEntity(Account? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new AccountDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}