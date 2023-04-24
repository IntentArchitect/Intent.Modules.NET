using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using AzureFunctions.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.SampleDomains
{
    public static class SampleDomainDtoMappingExtensions
    {
        public static SampleDomainDto MapToSampleDomainDto(this SampleDomain projectFrom, IMapper mapper)
        {
            return mapper.Map<SampleDomainDto>(projectFrom);
        }

        public static List<SampleDomainDto> MapToSampleDomainDtoList(this IEnumerable<SampleDomain> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToSampleDomainDto(mapper)).ToList();
        }
    }
}