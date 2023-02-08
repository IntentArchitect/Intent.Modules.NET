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

    public class AggregateRootCompositeSingleACompositeSingleAADto : IMapFrom<CompositeSingleAA>
    {
        public AggregateRootCompositeSingleACompositeSingleAADto()
        {
        }

        public static AggregateRootCompositeSingleACompositeSingleAADto Create(
            string compositeAttr,
            Guid id)
        {
            return new AggregateRootCompositeSingleACompositeSingleAADto
            {
                CompositeAttr = compositeAttr,
                Id = id,
            };
        }

        public string CompositeAttr { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleAA, AggregateRootCompositeSingleACompositeSingleAADto>();
        }
    }
}