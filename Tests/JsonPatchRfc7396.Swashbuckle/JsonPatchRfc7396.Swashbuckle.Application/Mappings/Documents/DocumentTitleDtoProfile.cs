using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public class DocumentTitleDtoProfile : Profile
    {
        public DocumentTitleDtoProfile()
        {
            CreateMap<DocumentTitle, DocumentTitleDto>();
        }
    }

    public static class DocumentTitleDtoMappingExtensions
    {
        public static DocumentTitleDto MapToDocumentTitleDto(this DocumentTitle projectFrom, IMapper mapper) => mapper.Map<DocumentTitleDto>(projectFrom);

        public static List<DocumentTitleDto> MapToDocumentTitleDtoList(
            this IEnumerable<DocumentTitle> projectFrom,
            IMapper mapper) => projectFrom.Select(x => x.MapToDocumentTitleDto(mapper)).ToList();
    }
}