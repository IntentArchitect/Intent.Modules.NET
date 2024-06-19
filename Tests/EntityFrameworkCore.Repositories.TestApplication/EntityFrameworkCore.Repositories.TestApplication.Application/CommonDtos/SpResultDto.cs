using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Mappings;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos
{
    public class SpResultDto : IMapFrom<SpResult>
    {
        public SpResultDto()
        {
            Data = null!;
        }

        public string Data { get; set; }

        public static SpResultDto Create(string data)
        {
            return new SpResultDto
            {
                Data = data
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SpResult, SpResultDto>();
        }
    }
}