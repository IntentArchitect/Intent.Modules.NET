using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Contracts.DataContracts;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.DataContracts
{
    public class DerivedDto : IMapFrom<DerivedDataContract>
    {
        public DerivedDto()
        {
            AttributeOnDerived = null!;
            AttributeOnBase = null!;
        }

        public string AttributeOnDerived { get; set; }
        public string AttributeOnBase { get; set; }

        public static DerivedDto Create(string attributeOnDerived, string attributeOnBase)
        {
            return new DerivedDto
            {
                AttributeOnDerived = attributeOnDerived,
                AttributeOnBase = attributeOnBase
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<DerivedDataContract, DerivedDto>();
        }
    }
}