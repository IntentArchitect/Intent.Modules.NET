using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users
{
    public static class TestCompanyDtoMappingExtensions
    {
        public static TestCompanyDto MapToTestCompanyDto(this Company projectFrom, IMapper mapper)
            => mapper.Map<TestCompanyDto>(projectFrom);

        public static List<TestCompanyDto> MapToTestCompanyDtoList(this IEnumerable<Company> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestCompanyDto(mapper)).ToList();
    }
}