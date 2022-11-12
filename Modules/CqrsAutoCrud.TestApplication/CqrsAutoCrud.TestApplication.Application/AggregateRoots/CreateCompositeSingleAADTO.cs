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

    public class CreateCompositeSingleAADTO : IMapFrom<CompositeSingleAA>
    {
        public CreateCompositeSingleAADTO()
        {
        }

        public static CreateCompositeSingleAADTO Create(
            Guid id,
            string compositeAttr)
        {
            return new CreateCompositeSingleAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleAA, CreateCompositeSingleAADTO>();
        }
    }
}