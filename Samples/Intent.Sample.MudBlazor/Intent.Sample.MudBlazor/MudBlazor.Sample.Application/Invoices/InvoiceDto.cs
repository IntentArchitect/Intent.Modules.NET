using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Application.Common.Mappings;
using MudBlazor.Sample.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.Sample.Application.Invoices
{
    public class InvoiceDto : IMapFrom<Invoice>
    {
        public InvoiceDto()
        {
            InvoiceNo = null!;
            OrderLines = null!;
            CustomerName = null!;
            AddressLine1 = null!;
            AddressCity = null!;
            AddressCountry = null!;
            AddressPostal = null!;
        }

        public Guid Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public List<InvoiceInvoiceLineDto> OrderLines { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerAccountNo { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressPostal { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Reference { get; set; }

        public static InvoiceDto Create(
            Guid id,
            string invoiceNo,
            DateTime? invoiceDate,
            List<InvoiceInvoiceLineDto> orderLines,
            Guid? customerId,
            string customerName,
            string? customerAccountNo,
            string addressLine1,
            string? addressLine2,
            string addressCity,
            string addressCountry,
            string addressPostal,
            DateTime? dueDate,
            string? reference)
        {
            return new InvoiceDto
            {
                Id = id,
                InvoiceNo = invoiceNo,
                InvoiceDate = invoiceDate,
                OrderLines = orderLines,
                CustomerId = customerId,
                CustomerName = customerName,
                CustomerAccountNo = customerAccountNo,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                AddressCity = addressCity,
                AddressCountry = addressCountry,
                AddressPostal = addressPostal,
                DueDate = dueDate,
                Reference = reference
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.InvoiceDate, opt => opt.MapFrom(src => (DateTime?)src.IssuedDate))
                .ForMember(d => d.OrderLines, opt => opt.MapFrom(src => src.OrderLines))
                .ForMember(d => d.CustomerId, opt => opt.MapFrom(src => (Guid?)src.CustomerId))
                .ForMember(d => d.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(d => d.CustomerAccountNo, opt => opt.MapFrom(src => src.Customer.AccountNo))
                .ForMember(d => d.AddressLine1, opt => opt.MapFrom(src => src.Customer.Address.Line1))
                .ForMember(d => d.AddressLine2, opt => opt.MapFrom(src => src.Customer.Address.Line2))
                .ForMember(d => d.AddressCity, opt => opt.MapFrom(src => src.Customer.Address.City))
                .ForMember(d => d.AddressCountry, opt => opt.MapFrom(src => src.Customer.Address.Country))
                .ForMember(d => d.AddressPostal, opt => opt.MapFrom(src => src.Customer.Address.Postal))
                .ForMember(d => d.DueDate, opt => opt.MapFrom(src => (DateTime?)src.DueDate));
        }
    }
}