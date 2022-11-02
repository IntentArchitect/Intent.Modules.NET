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

    public class AA4_Composite_SingleDTO : IMapFrom<AA4_Composite_Single>
    {
        public AA4_Composite_SingleDTO()
        {
        }

        public static AA4_Composite_SingleDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new AA4_Composite_SingleDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA4_Composite_Single, AA4_Composite_SingleDTO>();
        }
    }
}