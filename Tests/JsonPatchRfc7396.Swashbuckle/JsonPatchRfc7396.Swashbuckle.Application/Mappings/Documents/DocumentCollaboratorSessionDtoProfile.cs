using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentCollaboratorSessionDtoProfile : Profile
    {
        public DocumentCollaboratorSessionDtoProfile()
        {
            CreateMap<CollaboratorSession, DocumentCollaboratorSessionDto>();
        }
    }

    public static class DocumentCollaboratorSessionDtoMappingExtensions
    {
        public static DocumentCollaboratorSessionDto MapToDocumentCollaboratorSessionDto(
            this CollaboratorSession projectFrom,
            IMapper mapper) => mapper.Map<DocumentCollaboratorSessionDto>(projectFrom);

        public static List<DocumentCollaboratorSessionDto> MapToDocumentCollaboratorSessionDtoList(
            this IEnumerable<CollaboratorSession> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentCollaboratorSessionDto(mapper)).ToList();
    }
}