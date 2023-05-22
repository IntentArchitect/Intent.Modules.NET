using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.EF.SqlServer.Application.Common.Mappings;
using Entities.PrivateSetters.EF.SqlServer.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Application
{
    public class LineDto : IMapFrom<Line>
    {
        public LineDto()
        {
        }

        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }

        public static LineDto Create(Guid id, string description, int quantity)
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