using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Redis.Om.Repositories.Application.Common.Mappings;
using Redis.Om.Repositories.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes
{
    public class DerivedTypeDto : IMapFrom<DerivedType>
    {
        public DerivedTypeDto()
        {
            Id = null!;
            DerivedName = null!;
            BaseName = null!;
        }

        public string Id { get; set; }
        public string DerivedName { get; set; }
        public string BaseName { get; set; }

        public static DerivedTypeDto Create(string id, string derivedName, string baseName)
        {
            return new DerivedTypeDto
            {
                Id = id,
                DerivedName = derivedName,
                BaseName = baseName
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DerivedType, DerivedTypeDto>();
        }
    }
}