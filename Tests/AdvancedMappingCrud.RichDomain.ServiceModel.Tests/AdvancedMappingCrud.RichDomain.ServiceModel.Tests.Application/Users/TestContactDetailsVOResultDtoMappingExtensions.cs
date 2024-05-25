using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public static class TestContactDetailsVOResultDtoMappingExtensions
    {
        public static TestContactDetailsVOResultDto MapToTestContactDetailsVOResultDto(this ContactDetailsVO projectFrom, IMapper mapper)
            => mapper.Map<TestContactDetailsVOResultDto>(projectFrom);

        public static List<TestContactDetailsVOResultDto> MapToTestContactDetailsVOResultDtoList(this IEnumerable<ContactDetailsVO> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestContactDetailsVOResultDto(mapper)).ToList();
    }
}