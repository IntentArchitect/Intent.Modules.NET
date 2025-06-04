using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Application.Common.Mappings;
using MudBlazor.Sample.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.Sample.Application.Invoices
{
    public class InvoiceInvoiceLineDto : IMapFrom<InvoiceLine>
    {
        public InvoiceInvoiceLineDto()
        {
            ProductName = null!;
            ProductDescription = null!;
        }

        public Guid InvoiceId { get; set; }
        public Guid? ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string? ProductImageUrl { get; set; }

        public static InvoiceInvoiceLineDto Create(
            Guid invoiceId,
            Guid? productId,
            int units,
            decimal unitPrice,
            decimal? discount,
            Guid id,
            string productName,
            string productDescription,
            string? productImageUrl)
        {
            return new InvoiceInvoiceLineDto
            {
                InvoiceId = invoiceId,
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount,
                Id = id,
                ProductName = productName,
                ProductDescription = productDescription,
                ProductImageUrl = productImageUrl
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<InvoiceLine, InvoiceInvoiceLineDto>()
                .ForMember(d => d.ProductId, opt => opt.MapFrom(src => (Guid?)src.ProductId))
                .ForMember(d => d.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(d => d.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(d => d.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));
        }
    }
}