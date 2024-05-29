using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.Pagination;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Pagination
{
    public class LogEntryDto : IMapFrom<LogEntry>
    {
        public LogEntryDto()
        {
            Message = null!;
        }

        public Guid Id { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }

        public static LogEntryDto Create(Guid id, string message, DateTime timeStamp)
        {
            return new LogEntryDto
            {
                Id = id,
                Message = message,
                TimeStamp = timeStamp
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<LogEntry, LogEntryDto>();
        }
    }
}