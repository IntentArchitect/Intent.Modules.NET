using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class CreateCompositeManyAADTO : IMapFrom<CompositeManyAA>
    {
        public CreateCompositeManyAADTO()
        {
        }

        public static CreateCompositeManyAADTO Create(
            Guid id,
            string compositeAttr,
            Guid aCompositeSingleId)
        {
            return new CreateCompositeManyAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                ACompositeSingleId = aCompositeSingleId,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid ACompositeSingleId { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyAA, CreateCompositeManyAADTO>();
        }
    }
}