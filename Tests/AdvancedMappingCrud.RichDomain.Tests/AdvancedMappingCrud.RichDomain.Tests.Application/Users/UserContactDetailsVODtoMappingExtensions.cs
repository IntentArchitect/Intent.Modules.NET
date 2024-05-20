using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public static class UserContactDetailsVODtoMappingExtensions
    {
        public static UserContactDetailsVODto MapToUserContactDetailsVODto(this ContactDetailsVO projectFrom, IMapper mapper)
            => mapper.Map<UserContactDetailsVODto>(projectFrom);

        public static List<UserContactDetailsVODto> MapToUserContactDetailsVODtoList(this IEnumerable<ContactDetailsVO> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToUserContactDetailsVODto(mapper)).ToList();
    }
}