using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users
{
    public static class TestCompanyResultDtoMappingExtensions
    {
        public static TestCompanyResultDto MapToTestCompanyResultDto(this Company projectFrom, IMapper mapper)
            => mapper.Map<TestCompanyResultDto>(projectFrom);

        public static List<TestCompanyResultDto> MapToTestCompanyResultDtoList(this IEnumerable<Company> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToTestCompanyResultDto(mapper)).ToList();
    }
}