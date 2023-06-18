using AutoMapper;
using CosmosDB.Domain.Entities.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBDocumentMappingProfile", Version = "1.0")]

namespace CosmosDB.Infrastructure.Persistence.Documents.Folder
{
    internal class FolderContainerDocumentMappingProfile : Profile
    {
        public FolderContainerDocumentMappingProfile()
        {
            CreateMap<FolderContainerDocument, FolderContainer>().ReverseMap();
        }
    }
}