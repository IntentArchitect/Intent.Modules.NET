using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users
{
    public class UserContactDetailsVODto : IMapFrom<ContactDetailsVO>
    {
        public UserContactDetailsVODto()
        {
            Cell = null!;
            Email = null!;
        }

        public string Cell { get; set; }
        public string Email { get; set; }

        public static UserContactDetailsVODto Create(string cell, string email)
        {
            return new UserContactDetailsVODto
            {
                Cell = cell,
                Email = email
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ContactDetailsVO, UserContactDetailsVODto>();
        }
    }
}