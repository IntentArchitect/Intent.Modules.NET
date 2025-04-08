using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds
{
    public static class CompanyContactSecondDtoMappingExtensions
    {
        public static CompanyContactSecondDto MapToCompanyContactSecondDto(this CompanyContactSecond projectFrom, IMapper mapper)
            => mapper.Map<CompanyContactSecondDto>(projectFrom);

        public static List<CompanyContactSecondDto> MapToCompanyContactSecondDtoList(this IEnumerable<CompanyContactSecond> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompanyContactSecondDto(mapper)).ToList();
    }
}