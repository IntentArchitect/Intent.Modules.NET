using System;
using System.Collections.Generic;
using CosmosDBMultiTenancy.Domain.Common;
using CosmosDBMultiTenancy.Domain.Common.Interfaces;
using CosmosDBMultiTenancy.Domain.Entities;
using CosmosDBMultiTenancy.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace CosmosDBMultiTenancy.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocument : IInvoiceDocument, ICosmosDBDocument<Invoice, InvoiceDocument>
    {
        private string? _type;
        public string Id { get; set; } = default!;
        public string Number { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTimeOffset CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }

        public Invoice ToEntity(Invoice? entity = default)
        {
            entity ??= new Invoice();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Number = Number ?? throw new Exception($"{nameof(entity.Number)} is null");
            entity.TenantId = TenantId ?? throw new Exception($"{nameof(entity.TenantId)} is null");
            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.CreatedDate = CreatedDate;
            entity.UpdatedBy = UpdatedBy;
            entity.UpdatedDate = UpdatedDate;

            return entity;
        }
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? ICosmosDBDocument.PartitionKey
        {
            get => TenantId;
            set => TenantId = value!;
        }

        public InvoiceDocument PopulateFromEntity(Invoice entity)
        {
            Id = entity.Id;
            Number = entity.Number;
            TenantId = entity.TenantId;
            CreatedBy = entity.CreatedBy;
            CreatedDate = entity.CreatedDate;
            UpdatedBy = entity.UpdatedBy;
            UpdatedDate = entity.UpdatedDate;

            return this;
        }

        public static InvoiceDocument? FromEntity(Invoice? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InvoiceDocument().PopulateFromEntity(entity);
        }
    }
}