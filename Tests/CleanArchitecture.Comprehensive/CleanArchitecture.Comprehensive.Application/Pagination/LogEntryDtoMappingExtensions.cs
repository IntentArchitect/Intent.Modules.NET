using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination
{
    public static class LogEntryDtoMappingExtensions
    {
        public static LogEntryDto MapToLogEntryDto(this LogEntry projectFrom, IMapper mapper)
            => mapper.Map<LogEntryDto>(projectFrom);

        public static List<LogEntryDto> MapToLogEntryDtoList(this IEnumerable<LogEntry> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToLogEntryDto(mapper)).ToList();
    }
}