using AutoMapper;
using CqrsSpike.Publisher.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace CqrsSpike.Publisher.Application.AuditEntries
{
    public class AuditEntryDtoProfile : Profile
    {
        public AuditEntryDtoProfile()
        {
            CreateMap<AuditEntry, AuditEntryDto>();
        }
    }

    public static class AuditEntryDtoMappingExtensions
    {
        public static AuditEntryDto MapToAuditEntryDto(this AuditEntry projectFrom, IMapper mapper) => mapper.Map<AuditEntryDto>(projectFrom);

        public static List<AuditEntryDto> MapToAuditEntryDtoList(this IEnumerable<AuditEntry> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToAuditEntryDto(mapper)).ToList();
    }
}