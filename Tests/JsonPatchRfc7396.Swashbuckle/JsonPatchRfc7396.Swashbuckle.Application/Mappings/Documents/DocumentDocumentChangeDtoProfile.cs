using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentDocumentChangeDtoProfile : Profile
    {
        public DocumentDocumentChangeDtoProfile()
        {
            CreateMap<DocumentChange, DocumentDocumentChangeDto>();
        }
    }

    public static class DocumentDocumentChangeDtoMappingExtensions
    {
        public static DocumentDocumentChangeDto MapToDocumentDocumentChangeDto(
            this DocumentChange projectFrom,
            IMapper mapper) => mapper.Map<DocumentDocumentChangeDto>(projectFrom);

        public static List<DocumentDocumentChangeDto> MapToDocumentDocumentChangeDtoList(
            this IEnumerable<DocumentChange> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentDocumentChangeDto(mapper)).ToList();
    }
}