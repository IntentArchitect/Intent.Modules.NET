using System;
using AspNetCore.Controllers.Secured.Application.Common.Mappings;
using AspNetCore.Controllers.Secured.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AspNetCore.Controllers.Secured.Application.Buyers
{
    public class BuyerDto : IMapFrom<Buyer>
    {
        public BuyerDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public static BuyerDto Create(Guid id, string name, string surname, string email)
        {
            return new BuyerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Buyer, BuyerDto>();
        }
    }
}