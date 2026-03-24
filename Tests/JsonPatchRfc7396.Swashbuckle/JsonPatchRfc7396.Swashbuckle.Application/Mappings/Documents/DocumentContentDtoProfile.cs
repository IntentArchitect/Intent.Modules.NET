using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentContentDtoProfile : Profile
    {
        public DocumentContentDtoProfile()
        {
            CreateMap<DocumentContent, DocumentContentDto>();
        }
    }

    public static class DocumentContentDtoMappingExtensions
    {
        public static DocumentContentDto MapToDocumentContentDto(this DocumentContent projectFrom, IMapper mapper) => mapper.Map<DocumentContentDto>(projectFrom);

        public static List<DocumentContentDto> MapToDocumentContentDtoList(
            this IEnumerable<DocumentContent> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentContentDto(mapper)).ToList();
    }
}