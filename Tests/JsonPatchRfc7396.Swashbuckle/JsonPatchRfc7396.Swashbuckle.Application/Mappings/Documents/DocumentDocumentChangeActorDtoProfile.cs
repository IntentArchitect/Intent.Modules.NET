using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentDocumentChangeActorDtoProfile : Profile
    {
        public DocumentDocumentChangeActorDtoProfile()
        {
            CreateMap<Actor, DocumentDocumentChangeActorDto>();
        }
    }

    public static class DocumentDocumentChangeActorDtoMappingExtensions
    {
        public static DocumentDocumentChangeActorDto MapToDocumentDocumentChangeActorDto(
            this Actor projectFrom,
            IMapper mapper) => mapper.Map<DocumentDocumentChangeActorDto>(projectFrom);

        public static List<DocumentDocumentChangeActorDto> MapToDocumentDocumentChangeActorDtoList(
            this IEnumerable<Actor> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentDocumentChangeActorDto(mapper)).ToList();
    }
}