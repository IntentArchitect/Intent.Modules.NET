using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContacts
{
    public static class CompanyContactDtoMappingExtensions
    {
        public static CompanyContactDto MapToCompanyContactDto(this CompanyContact projectFrom, IMapper mapper)
            => mapper.Map<CompanyContactDto>(projectFrom);

        public static List<CompanyContactDto> MapToCompanyContactDtoList(this IEnumerable<CompanyContact> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToCompanyContactDto(mapper)).ToList();
    }
}