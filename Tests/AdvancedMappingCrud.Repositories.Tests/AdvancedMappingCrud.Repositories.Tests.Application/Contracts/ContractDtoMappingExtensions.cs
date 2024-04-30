using System.Collections.Generic;
using System.Linq;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.MappingExtensions", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts
{
    public static class ContractDtoMappingExtensions
    {
        public static ContractDto MapToContractDto(this Contract projectFrom, IMapper mapper)
            => mapper.Map<ContractDto>(projectFrom);

        public static List<ContractDto> MapToContractDtoList(this IEnumerable<Contract> projectFrom, IMapper mapper)
            => projectFrom.Select(x => x.MapToContractDto(mapper)).ToList();
    }
}