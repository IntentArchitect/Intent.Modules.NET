using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users
{
    public static class PreferenceDtoMappingExtensions
    {
        public static PreferenceDto MapToPreferenceDto(this Preference projectFrom, IMapper mapper)
            => mapper.Map<PreferenceDto>(projectFrom);

        public static List<PreferenceDto> MapToPreferenceDtoList(this IEnumerable<Preference> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToPreferenceDto(mapper)).ToList();
    }
}