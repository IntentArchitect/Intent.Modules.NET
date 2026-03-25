using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentDocumentRevisionAuthorDtoProfile : Profile
    {
        public DocumentDocumentRevisionAuthorDtoProfile()
        {
            CreateMap<Actor, DocumentDocumentRevisionAuthorDto>();
        }
    }

    public static class DocumentDocumentRevisionAuthorDtoMappingExtensions
    {
        public static DocumentDocumentRevisionAuthorDto MapToDocumentDocumentRevisionAuthorDto(
            this Actor projectFrom,
            IMapper mapper) => mapper.Map<DocumentDocumentRevisionAuthorDto>(projectFrom);

        public static List<DocumentDocumentRevisionAuthorDto> MapToDocumentDocumentRevisionAuthorDtoList(
            this IEnumerable<Actor> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentDocumentRevisionAuthorDto(mapper)).ToList();
    }
}