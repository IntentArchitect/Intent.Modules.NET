using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public CustomerDto()
        {
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
        public bool PreferencesNewsletter { get; set; }
        public bool PreferencesSpecials { get; set; }

        public static CustomerDto Create(
            Guid id,
            string name,
            string surname,
            bool isActive,
            bool preferencesNewsletter,
            bool preferencesSpecials)
        {
            return new CustomerDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                IsActive = isActive,
                PreferencesNewsletter = preferencesNewsletter,
                PreferencesSpecials = preferencesSpecials
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>()
                .ForPath(d => d.PreferencesNewsletter, opt => opt.MapFrom(src => src.Preferences!.Newsletter))
                .ForPath(d => d.PreferencesSpecials, opt => opt.MapFrom(src => src.Preferences!.Specials));
        }
    }
}