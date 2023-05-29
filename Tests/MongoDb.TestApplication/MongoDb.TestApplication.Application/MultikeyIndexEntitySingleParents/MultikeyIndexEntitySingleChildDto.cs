using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MongoDb.TestApplication.Application.Common.Mappings;
using MongoDb.TestApplication.Domain.Entities.Indexes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.MultikeyIndexEntitySingleParents
{
    public class MultikeyIndexEntitySingleChildDto : IMapFrom<MultikeyIndexEntitySingleChild>
    {
        public MultikeyIndexEntitySingleChildDto()
        {
            MultiKey = null!;
        }

        public IEnumerable<string> MultiKey { get; set; }
        public Guid Id { get; set; }

        public static MultikeyIndexEntitySingleChildDto Create(IEnumerable<string> multiKey, Guid id)
        {
            return new MultikeyIndexEntitySingleChildDto
            {
                MultiKey = multiKey,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<MultikeyIndexEntitySingleChild, MultikeyIndexEntitySingleChildDto>();
        }
    }
}