using System;
using System.Linq;
using AutoMapper;
using CosmosDB.PrivateSetters.Application.Common.Mappings;
using CosmosDB.PrivateSetters.Domain.Entities;
using CosmosDB.PrivateSetters.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Departments
{
    public class DepartmentDto : IMapFrom<Department>
    {
        public DepartmentDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public Guid? UniversityId { get; set; }
        public string Name { get; set; }
        public DepartmentUniversityDto? University { get; set; }

        public static DepartmentDto Create(Guid id, Guid? universityId, string name, DepartmentUniversityDto? university)
        {
            return new DepartmentDto
            {
                Id = id,
                UniversityId = universityId,
                Name = name,
                University = university
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Department, DepartmentDto>()
                .AfterMap<MappingAction>();
        }

        internal class MappingAction : IMappingAction<Department, DepartmentDto>
        {
            private readonly IUniversityRepository _universityRepository;
            private readonly IMapper _mapper;

            public MappingAction(IUniversityRepository universityRepository, IMapper mapper)
            {
                _universityRepository = universityRepository;
                _mapper = mapper;
            }

            public void Process(Department source, DepartmentDto destination, ResolutionContext context)
            {
                var university = source.UniversityId != null ? _universityRepository.FindByIdAsync(source.UniversityId.Value).Result : null;
                destination.University = university != null ? university.MapToDepartmentUniversityDto(_mapper) : null;
            }
        }
    }
}