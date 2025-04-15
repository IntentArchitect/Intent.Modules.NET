using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ServiceToServiceInvocations;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ServiceToServiceInvocation
{
    public static class GetDataEntryDtoMappingExtensions
    {
        public static GetDataEntryDto MapToGetDataEntryDto(this GetDataEntry projectFrom, IMapper mapper)
            => mapper.Map<GetDataEntryDto>(projectFrom);

        public static List<GetDataEntryDto> MapToGetDataEntryDtoList(this IEnumerable<GetDataEntry> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToGetDataEntryDto(mapper)).ToList();
    }
}