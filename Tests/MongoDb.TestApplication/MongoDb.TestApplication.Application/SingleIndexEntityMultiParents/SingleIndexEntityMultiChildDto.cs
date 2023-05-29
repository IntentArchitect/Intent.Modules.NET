using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.SingleIndexEntityMultiParents
{
    public class SingleIndexEntityMultiChildDto : IMapFrom<SingleIndexEntityMultiChild>
    {
        public SingleIndexEntityMultiChildDto()
        {
            SingleIndex = null!;
        }

        public string SingleIndex { get; set; }
        public Guid Id { get; set; }

        public static SingleIndexEntityMultiChildDto Create(string singleIndex, Guid id)
        {
            return new SingleIndexEntityMultiChildDto
            {
                SingleIndex = singleIndex,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<SingleIndexEntityMultiChild, SingleIndexEntityMultiChildDto>();
        }
    }
}