using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntityMultiParents
{
    public class MultikeyIndexEntityMultiChildDto : IMapFrom<MultikeyIndexEntityMultiChild>
    {
        public MultikeyIndexEntityMultiChildDto()
        {
            MultiKey = null!;
        }

        public List<string> MultiKey { get; set; }
        public Guid Id { get; set; }

        public static MultikeyIndexEntityMultiChildDto Create(List<string> multiKey, Guid id)
        {
            return new MultikeyIndexEntityMultiChildDto
            {
                MultiKey = multiKey,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MultikeyIndexEntityMultiChild, MultikeyIndexEntityMultiChildDto>();
        }
    }
}