using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AutoMapper;
using AzureFunctions.NET6.Domain.Entities;
using AzureFunctions.NET8.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AzureFunctions.NET8.Application.SampleDomains
{
    public static class SampleDomainDtoMappingExtensions
    {
        public static SampleDomainDto MapToSampleDomainDto(this SampleDomain projectFrom, IMapper mapper)
            => mapper.Map<SampleDomainDto>(projectFrom);

        public static List<SampleDomainDto> MapToSampleDomainDtoList(this IEnumerable<SampleDomain> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToSampleDomainDto(mapper)).ToList();
    }
}