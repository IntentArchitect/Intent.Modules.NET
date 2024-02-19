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
    internal class GetAllImplementation.CustomerDocument : IGetAllImplementationCustomerDocument, ICosmosDBDocument<GetAllImplementationCustomer, GetAllImplementation.CustomerDocument>
    {
        private string? _type;
    [JsonProperty("_etag")]
    private string? _etag;
    [JsonProperty("type")]
    string IItem.Type
    {
        get => _type ??= GetType().GetNameForDocument();
        set => _type = value;
    }
    string? IItemWithEtag.Etag => _etag;
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public GetAllImplementationOrderDocument GetAllImplementation.Order { get; set; } = default!;
    IGetAllImplementationOrderDocument IGetAllImplementationCustomerDocument.GetAllImplementation.Order => GetAllImplementation.Order;

    public GetAllImplementationCustomer ToEntity(GetAllImplementationCustomer? entity = default)
    {
        entity ??= new GetAllImplementationCustomer();

        entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
        entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
        entity.GetAllImplementation.Order = GetAllImplementation.Order.ToEntity() ?? throw new Exception($"{nameof(entity.GetAllImplementation.Order)} is null");

        return entity;
    }

    public GetAllImplementation.CustomerDocument PopulateFromEntity(
        GetAllImplementationCustomer entity,
        string? etag = null)
    {
        Id = entity.Id;
        Name = entity.Name;
        GetAllImplementation.Order = GetAllImplementationOrderDocument.FromEntity(entity.GetAllImplementation.Order)!;

        _etag = etag;

        return this;
    }

    public static GetAllImplementation.CustomerDocument? FromEntity(
        GetAllImplementationCustomer? entity,
        string? etag = null)
    {
        if (entity is null)
        {
            return null;
        }

        return new GetAllImplementation.CustomerDocument().PopulateFromEntity(entity, etag);
    }
}
}