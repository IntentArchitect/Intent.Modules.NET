using AutoMapper;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocumentMappingProfile", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents
{
    internal class IdTestingDocumentMappingProfile : Profile
    {
        public IdTestingDocumentMappingProfile()
        {
            CreateMap<IdTestingDocument, IdTesting>().ReverseMap();
        }
    }
}