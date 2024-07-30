using System;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class GetAllImplementationCustomerDocument : IGetAllImplementationCustomerDocument, ICosmosDBDocument<GetAllImplementationCustomer, GetAllImplementationCustomerDocument>
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
        public GetAllImplementationOrderDocument GetAllImplementationOrder { get; set; } = default!;
        IGetAllImplementationOrderDocument IGetAllImplementationCustomerDocument.GetAllImplementationOrder => GetAllImplementationOrder;

        public GetAllImplementationCustomer ToEntity(GetAllImplementationCustomer? entity = default)
        {
            entity ??= new GetAllImplementationCustomer();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.GetAllImplementationOrder = GetAllImplementationOrder.ToEntity() ?? throw new Exception($"{nameof(entity.GetAllImplementationOrder)} is null");

            return entity;
        }

        public GetAllImplementationCustomerDocument PopulateFromEntity(
            GetAllImplementationCustomer entity,
            Func<string, string?> getEtag)
        {
            Id = entity.Id;
            Name = entity.Name;
            GetAllImplementationOrder = GetAllImplementationOrderDocument.FromEntity(entity.GetAllImplementationOrder)!;

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static GetAllImplementationCustomerDocument? FromEntity(
            GetAllImplementationCustomer? entity,
            Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new GetAllImplementationCustomerDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}