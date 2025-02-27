using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Farmers.GetMachines
{
    public class GetMachinesQuery : IRequest<List<MachinesDto>>, IQuery
    {
        public GetMachinesQuery(Guid farmerId)
        {
            FarmerId = farmerId;
        }

        public Guid FarmerId { get; set; }
    }
}