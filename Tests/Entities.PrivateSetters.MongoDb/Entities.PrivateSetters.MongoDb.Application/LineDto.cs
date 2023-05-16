using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.MongoDb.Application.Common.Mappings;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Application
{
    public class LineDto : IMapFrom<Line>
    {
        public LineDto()
        {
        }

        public string Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public static LineDto Create(string id, string description, int quantity)
        {
            return new LineDto
            {
                Id = id,
                Description = description,
                Quantity = quantity
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Line, LineDto>();
        }
    }
}