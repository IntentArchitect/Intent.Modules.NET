using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootLongs
{

    public class CreateCompositeOfAggrLongDTO
    {
        public CreateCompositeOfAggrLongDTO()
        {
        }

        public static CreateCompositeOfAggrLongDTO Create(
            long id,
            string attribute)
        {
            return new CreateCompositeOfAggrLongDTO
            {
                Id = id,
                Attribute = attribute,
            };
        }

        public long Id { get; set; }

        public string Attribute { get; set; }
    }
}