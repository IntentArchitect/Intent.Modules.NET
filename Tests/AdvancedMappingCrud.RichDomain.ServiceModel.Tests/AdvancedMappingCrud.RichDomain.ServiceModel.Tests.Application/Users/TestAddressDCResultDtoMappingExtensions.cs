using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public static class TestAddressDCResultDtoMappingExtensions
    {
        public static TestAddressDCResultDto MapToTestAddressDCResultDto(this AddressDC projectFrom, IMapper mapper)
            => mapper.Map<TestAddressDCResultDto>(projectFrom);

        public static List<TestAddressDCResultDto> MapToTestAddressDCResultDtoList(this IEnumerable<AddressDC> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestAddressDCResultDto(mapper)).ToList();
    }
}