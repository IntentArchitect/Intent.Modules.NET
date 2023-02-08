using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class AggregateRootCompositeSingleADto : IMapFrom<CompositeSingleA>
    {
        public AggregateRootCompositeSingleADto()
        {
        }

        public static AggregateRootCompositeSingleADto Create(
            string compositeAttr,
            Guid id)
        {
            return new AggregateRootCompositeSingleADto
            {
                CompositeAttr = compositeAttr,
                Id = id,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleA, AggregateRootCompositeSingleADto>();
        }
    }
}