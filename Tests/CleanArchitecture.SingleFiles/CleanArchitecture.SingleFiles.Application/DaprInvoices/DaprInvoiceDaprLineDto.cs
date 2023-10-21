using AutoMapper;
using CleanArchitecture.SingleFiles.Application.Common.Mappings;
using CleanArchitecture.SingleFiles.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Application.DaprInvoices
{
    public class DaprInvoiceDaprLineDto : IMapFrom<DaprLine>
    {
        public DaprInvoiceDaprLineDto()
        {
            DaprInvoiceId = null!;
            Id = null!;
            Name = null!;
        }

        public string DaprInvoiceId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        public static DaprInvoiceDaprLineDto Create(string daprInvoiceId, string id, string name)
        {
            return new DaprInvoiceDaprLineDto
            {
                DaprInvoiceId = daprInvoiceId,
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DaprLine, DaprInvoiceDaprLineDto>();
        }
    }
}