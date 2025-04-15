using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ServiceToServiceInvocations;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ServiceToServiceInvocation
{
    public class GetDataEntryDto : IMapFrom<GetDataEntry>
    {
        public GetDataEntryDto()
        {
            Data = null!;
        }

        public string Data { get; set; }

        public static GetDataEntryDto Create(string data)
        {
            return new GetDataEntryDto
            {
                Data = data
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<GetDataEntry, GetDataEntryDto>();
        }
    }
}