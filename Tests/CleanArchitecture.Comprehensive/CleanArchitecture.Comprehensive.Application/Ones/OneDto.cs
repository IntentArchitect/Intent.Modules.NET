using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Ones
{
    public class OneDto : IMapFrom<One>
    {
        public OneDto()
        {
        }

        public Guid Id { get; set; }
        public int OneId { get; set; }

        public static OneDto Create(Guid id, int oneId)
        {
            return new OneDto
            {
                Id = id,
                OneId = oneId
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<One, OneDto>();
        }
    }
}