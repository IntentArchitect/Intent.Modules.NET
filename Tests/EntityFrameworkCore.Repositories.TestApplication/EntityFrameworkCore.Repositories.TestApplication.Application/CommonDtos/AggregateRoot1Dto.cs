using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Mappings;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.CommonDtos
{
    public class AggregateRoot1Dto : IMapFrom<AggregateRoot1>
    {
        public AggregateRoot1Dto()
        {
            Tag = null!;
        }

        public string Tag { get; set; }

        public static AggregateRoot1Dto Create(string tag)
        {
            return new AggregateRoot1Dto
            {
                Tag = tag
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRoot1, AggregateRoot1Dto>();
        }
    }
}