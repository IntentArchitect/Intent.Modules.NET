using System;
using System.Collections.Generic;
using AutoMapper;
using Entities.PrivateSetters.EF.CosmosDb.Application.Common.Mappings;
using Entities.PrivateSetters.EF.CosmosDb.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.EF.CosmosDb.Application
{
    public class LineDto : IMapFrom<Line>
    {
        public LineDto()
        {
        }

        public Guid InvoiceId { get; set; }
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }

        public static LineDto Create(Guid invoiceId, string description, int quantity)
        {
            return new LineDto
            {
                InvoiceId = invoiceId,
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