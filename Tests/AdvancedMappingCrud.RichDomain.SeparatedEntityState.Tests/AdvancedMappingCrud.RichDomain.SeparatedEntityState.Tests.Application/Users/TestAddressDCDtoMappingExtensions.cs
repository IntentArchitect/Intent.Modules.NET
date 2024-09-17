using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Contracts;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users
{
    public static class TestAddressDCDtoMappingExtensions
    {
        public static TestAddressDCDto MapToTestAddressDCDto(this AddressDC projectFrom, IMapper mapper)
            => mapper.Map<TestAddressDCDto>(projectFrom);

        public static List<TestAddressDCDto> MapToTestAddressDCDtoList(this IEnumerable<AddressDC> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestAddressDCDto(mapper)).ToList();
    }
}