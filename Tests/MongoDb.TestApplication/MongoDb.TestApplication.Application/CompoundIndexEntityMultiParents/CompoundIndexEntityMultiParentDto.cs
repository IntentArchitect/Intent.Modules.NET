using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.CompoundIndexEntityMultiParents
{
    public class CompoundIndexEntityMultiParentDto : IMapFrom<CompoundIndexEntityMultiParent>
    {
        public CompoundIndexEntityMultiParentDto()
        {
            Id = null!;
            SomeField = null!;
        }

        public string Id { get; set; }
        public string SomeField { get; set; }

        public static CompoundIndexEntityMultiParentDto Create(string id, string someField)
        {
            return new CompoundIndexEntityMultiParentDto
            {
                Id = id,
                SomeField = someField
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompoundIndexEntityMultiParent, CompoundIndexEntityMultiParentDto>();
        }
    }
}