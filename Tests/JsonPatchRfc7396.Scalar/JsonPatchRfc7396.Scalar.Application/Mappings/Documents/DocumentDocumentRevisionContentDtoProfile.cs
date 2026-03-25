using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public class DocumentDocumentRevisionContentDtoProfile : Profile
    {
        public DocumentDocumentRevisionContentDtoProfile()
        {
            CreateMap<DocumentContent, DocumentDocumentRevisionContentDto>();
        }
    }

    public static class DocumentDocumentRevisionContentDtoMappingExtensions
    {
        public static DocumentDocumentRevisionContentDto MapToDocumentDocumentRevisionContentDto(
            this DocumentContent projectFrom,
            IMapper mapper) => mapper.Map<DocumentDocumentRevisionContentDto>(projectFrom);

        public static List<DocumentDocumentRevisionContentDto> MapToDocumentDocumentRevisionContentDtoList(
            this IEnumerable<DocumentContent> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentDocumentRevisionContentDto(mapper)).ToList();
    }
}