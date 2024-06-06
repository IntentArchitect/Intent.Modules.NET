using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Comprehensive.Domain.Entities.BasicMappingMapToValueObjects;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects
{
    public static class SubmissionDtoMappingExtensions
    {
        public static SubmissionDto MapToSubmissionDto(this Submission projectFrom, IMapper mapper)
            => mapper.Map<SubmissionDto>(projectFrom);

        public static List<SubmissionDto> MapToSubmissionDtoList(this IEnumerable<Submission> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSubmissionDto(mapper)).ToList();
    }
}