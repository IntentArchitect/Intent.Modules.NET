using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Application.Common.Mappings;
using GraphQL.CQRS.TestApplication.Application.Interfaces;
using GraphQL.CQRS.TestApplication.Application.Products;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using HotChocolate;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Invoices
{
    public class InvoiceInvoiceLineDto : IMapFrom<InvoiceLine>
    {
        public InvoiceInvoiceLineDto()
        {
        }

        public int No { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Guid ProductId { get; set; }
        public Guid InvoiceId { get; set; }

        public static InvoiceInvoiceLineDto Create(int no, int quantity, decimal amount, Guid productId, Guid invoiceId)
        {
            return new InvoiceInvoiceLineDto
            {
                No = no,
                Quantity = quantity,
                Amount = amount,
                ProductId = productId,
                InvoiceId = invoiceId
            };
        }

        [GraphQLDescription("Retrieves the product associated with this invoice")]
        public async Task<ProductDto> GetProduct([Service] IProductsService service)
        {
            return await service.FindProductById(ProductId);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InvoiceLine, InvoiceInvoiceLineDto>();
        }
    }
}