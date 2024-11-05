using System;
using AutoMapper;
using CosmosDB.Application.Common.Mappings;
using CosmosDB.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.Application.Departments
{
    public class DepartmentUniversityDto : IMapFrom<University>
    {
        public DepartmentUniversityDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }

        public static DepartmentUniversityDto Create(string name, Guid id)
        {
            return new DepartmentUniversityDto
            {
                Name = name,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<University, DepartmentUniversityDto>();
        }
    }
}