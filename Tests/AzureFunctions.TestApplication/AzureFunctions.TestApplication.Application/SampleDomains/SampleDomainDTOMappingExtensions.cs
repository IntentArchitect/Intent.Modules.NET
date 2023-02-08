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
    public static class SampleDomainDTOMappingExtensions
    {
        public static SampleDomainDTO MapToSampleDomainDTO(this SampleDomain projectFrom, IMapper mapper)
        {
            return mapper.Map<SampleDomainDTO>(projectFrom);
        }

        public static List<SampleDomainDTO> MapToSampleDomainDTOList(this IEnumerable<SampleDomain> projectFrom, IMapper mapper)
        {
            return projectFrom.Select(x => x.MapToSampleDomainDTO(mapper)).ToList();
        }
    }
}