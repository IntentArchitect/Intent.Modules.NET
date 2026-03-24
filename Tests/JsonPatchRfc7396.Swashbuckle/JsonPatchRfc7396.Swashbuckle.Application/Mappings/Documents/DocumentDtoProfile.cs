using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentDtoProfile : Profile
    {
        public DocumentDtoProfile()
        {
            CreateMap<Document, DocumentDto>();
        }
    }

    public static class DocumentDtoMappingExtensions
    {
        public static DocumentDto MapToDocumentDto(this Document projectFrom, IMapper mapper) => mapper.Map<DocumentDto>(projectFrom);

        public static List<DocumentDto> MapToDocumentDtoList(this IEnumerable<Document> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToDocumentDto(mapper)).ToList();
    }
}