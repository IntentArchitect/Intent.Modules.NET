using System;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootComposite
{

    public class CompositeSingleAADTO : IMapFrom<CompositeSingleAA>
    {
        public CompositeSingleAADTO()
        {
        }

        public static CompositeSingleAADTO Create(
            Guid id,
            string compositeAttr)
        {
            return new CompositeSingleAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleAA, CompositeSingleAADTO>();
        }
    }
}