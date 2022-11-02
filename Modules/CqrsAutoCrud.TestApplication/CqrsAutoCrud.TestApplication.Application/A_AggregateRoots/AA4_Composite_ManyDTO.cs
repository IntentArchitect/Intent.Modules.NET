using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots
{

    public class AA4_Composite_ManyDTO : IMapFrom<AA4_Composite_Many>
    {
        public AA4_Composite_ManyDTO()
        {
        }

        public static AA4_Composite_ManyDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new AA4_Composite_ManyDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA4_Composite_Many, AA4_Composite_ManyDTO>();
        }
    }
}