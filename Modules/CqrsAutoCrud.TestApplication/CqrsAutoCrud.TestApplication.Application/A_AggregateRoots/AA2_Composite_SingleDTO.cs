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

    public class AA2_Composite_SingleDTO : IMapFrom<AA2_Composite_Single>
    {
        public AA2_Composite_SingleDTO()
        {
        }

        public static AA2_Composite_SingleDTO Create(
            Guid id,
            string compositeAttr)
        {
            return new AA2_Composite_SingleDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AA2_Composite_Single, AA2_Composite_SingleDTO>();
        }
    }
}