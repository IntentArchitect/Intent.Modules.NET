using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
            Name = null!;
            Surname = null!;
            QuoteRefNo = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string QuoteRefNo { get; set; }

        public static UserDto Create(Guid id, string name, string surname, string quoteRefNo)
        {
            return new UserDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                QuoteRefNo = quoteRefNo
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.QuoteRefNo, opt => opt.MapFrom(src => src.Quote.RefNo));
        }
    }
}