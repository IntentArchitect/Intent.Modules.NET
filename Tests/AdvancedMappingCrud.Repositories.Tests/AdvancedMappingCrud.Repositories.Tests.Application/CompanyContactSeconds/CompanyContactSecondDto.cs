using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.CompanyContactSeconds
{
    public class CompanyContactSecondDto : IMapFrom<CompanyContactSecond>
    {
        public CompanyContactSecondDto()
        {
        }

        public Guid Id { get; set; }
        public Guid ContactSecondId { get; set; }

        public static CompanyContactSecondDto Create(Guid id, Guid contactSecondId)
        {
            return new CompanyContactSecondDto
            {
                Id = id,
                ContactSecondId = contactSecondId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompanyContactSecond, CompanyContactSecondDto>();
        }
    }
}