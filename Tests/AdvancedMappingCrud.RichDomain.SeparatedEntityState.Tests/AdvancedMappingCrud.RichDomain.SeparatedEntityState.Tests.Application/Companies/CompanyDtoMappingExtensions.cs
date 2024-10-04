using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Companies
{
    public static class CompanyDtoMappingExtensions
    {
        public static CompanyDto MapToCompanyDto(this Company projectFrom, IMapper mapper)
            => mapper.Map<CompanyDto>(projectFrom);

        public static List<CompanyDto> MapToCompanyDtoList(this IEnumerable<Company> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompanyDto(mapper)).ToList();
    }
}