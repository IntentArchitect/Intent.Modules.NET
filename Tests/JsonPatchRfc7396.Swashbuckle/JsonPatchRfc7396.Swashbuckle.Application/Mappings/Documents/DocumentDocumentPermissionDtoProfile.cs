using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.Entities.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentDocumentPermissionDtoProfile : Profile
    {
        public DocumentDocumentPermissionDtoProfile()
        {
            CreateMap<DocumentPermission, DocumentDocumentPermissionDto>();
        }
    }

    public static class DocumentDocumentPermissionDtoMappingExtensions
    {
        public static DocumentDocumentPermissionDto MapToDocumentDocumentPermissionDto(
            this DocumentPermission projectFrom,
            IMapper mapper) => mapper.Map<DocumentDocumentPermissionDto>(projectFrom);

        public static List<DocumentDocumentPermissionDto> MapToDocumentDocumentPermissionDtoList(
            this IEnumerable<DocumentPermission> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentDocumentPermissionDto(mapper)).ToList();
    }
}