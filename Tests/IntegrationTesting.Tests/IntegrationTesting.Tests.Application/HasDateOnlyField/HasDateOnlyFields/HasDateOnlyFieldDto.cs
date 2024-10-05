using System;
using AutoMapper;
using IntegrationTesting.Tests.Application.Common.Mappings;
using IntegrationTesting.Tests.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasDateOnlyField.HasDateOnlyFields
{
    public class HasDateOnlyFieldDto : IMapFrom<Domain.Entities.HasDateOnlyField>
    {
        public HasDateOnlyFieldDto()
        {
        }

        public Guid Id { get; set; }
        public DateOnly MyDate { get; set; }

        public static HasDateOnlyFieldDto Create(Guid id, DateOnly myDate)
        {
            return new HasDateOnlyFieldDto
            {
                Id = id,
                MyDate = myDate
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Domain.Entities.HasDateOnlyField, HasDateOnlyFieldDto>();
        }
    }
}