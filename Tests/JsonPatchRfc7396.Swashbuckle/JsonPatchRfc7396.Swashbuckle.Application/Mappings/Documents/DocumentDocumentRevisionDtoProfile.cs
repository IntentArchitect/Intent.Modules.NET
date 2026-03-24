using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentDocumentRevisionDtoProfile : Profile
    {
        public DocumentDocumentRevisionDtoProfile()
        {
            CreateMap<DocumentRevision, DocumentDocumentRevisionDto>();
        }
    }

    public static class DocumentDocumentRevisionDtoMappingExtensions
    {
        public static DocumentDocumentRevisionDto MapToDocumentDocumentRevisionDto(
            this DocumentRevision projectFrom,
            IMapper mapper) => mapper.Map<DocumentDocumentRevisionDto>(projectFrom);

        public static List<DocumentDocumentRevisionDto> MapToDocumentDocumentRevisionDtoList(
            this IEnumerable<DocumentRevision> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentDocumentRevisionDto(mapper)).ToList();
    }
}