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

    public class AA1_Composite_SingleDTO : IMapFrom<AA1_Composite_Single>
    {
        public AA1_Composite_SingleDTO()
        {
        }

        public static AA1_Composite_SingleDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new AA1_Composite_SingleDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA1_Composite_Single, AA1_Composite_SingleDTO>();
        }
    }
}