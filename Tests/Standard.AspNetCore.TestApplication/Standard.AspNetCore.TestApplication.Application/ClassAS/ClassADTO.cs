using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.Common.Mappings;
using Standard.AspNetCore.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.ClassAS
{

    public class ClassADTO : IMapFrom<ClassA>
    {
        public ClassADTO()
        {
        }

        public static ClassADTO Create(Guid id, string attribute)
        {
            return new ClassADTO
            {
                Id = id,
                Attribute = attribute
            };
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; } = null!;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ClassA, ClassADTO>();
        }
    }
}