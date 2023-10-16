using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TableStorage.Tests.Application.Common.Mappings;
using TableStorage.Tests.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TableStorage.Tests.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            PartitionKey = null!;
            RowKey = null!;
            OrderPartitionKey = null!;
            OrderRowKey = null!;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime IssuedData { get; set; }
        public string OrderPartitionKey { get; set; }
        public string OrderRowKey { get; set; }

        public static InvoiceDto Create(
            string partitionKey,
            string rowKey,
            DateTime issuedData,
            string orderPartitionKey,
            string orderRowKey)
        {
            return new InvoiceDto
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                IssuedData = issuedData,
                OrderPartitionKey = orderPartitionKey,
                OrderRowKey = orderRowKey
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>();
        }
    }
}