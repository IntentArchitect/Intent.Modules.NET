using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.Documents;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata
{
    public class SimpleOdataDto : IMapFrom<Domain.Entities.SimpleOdata>
    {
        public SimpleOdataDto()
        {
            Id = null!;
            Name = null!;
            Surname = null!;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static SimpleOdataDto Create(string id, string name, string surname)
        {
            return new SimpleOdataDto
            {
                Id = id,
                Name = name,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.SimpleOdata, SimpleOdataDto>();

            profile.CreateMap<ISimpleOdataDocument, SimpleOdataDto>();
        }
    }
}