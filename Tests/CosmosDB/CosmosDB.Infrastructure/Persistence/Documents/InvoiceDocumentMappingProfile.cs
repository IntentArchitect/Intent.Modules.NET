using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocumentMappingProfile", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class InvoiceDocumentMappingProfile : Profile
    {
        public InvoiceDocumentMappingProfile()
        {
            CreateMap<InvoiceDocument, Invoice>().ReverseMap();
        }
    }
}