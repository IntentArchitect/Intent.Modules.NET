using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.BasicOrderBies
{
    public class BasicOrderByDto : IMapFrom<BasicOrderBy>
    {
        public BasicOrderByDto()
        {
            Id = null!;
            Name = null!;
            Surname = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static BasicOrderByDto Create(string id, string name, string surname)
        {
            return new BasicOrderByDto
            {
                Id = id,
                Name = name,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<BasicOrderBy, BasicOrderByDto>();

            profile.CreateMap<IBasicOrderByDocument, BasicOrderByDto>();
        }
    }
}