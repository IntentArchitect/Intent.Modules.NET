using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.AnemicChild;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ParentWithAnemicChildren
{
    public class ParentWithAnemicChildDto : IMapFrom<ParentWithAnemicChild>
    {
        public ParentWithAnemicChildDto()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static ParentWithAnemicChildDto Create(Guid id, string name, string surname)
        {
            return new ParentWithAnemicChildDto
            {
                Id = id,
                Name = name,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ParentWithAnemicChild, ParentWithAnemicChildDto>();
        }
    }
}