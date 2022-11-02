using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.GetA_AggregateRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetA_AggregateRootsQueryHandler : IRequestHandler<GetA_AggregateRootsQuery, List<A_AggregateRootDTO>>
    {
        private IA_AggregateRootRepository _a_AggregateRootRepository;
        private IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetA_AggregateRootsQueryHandler(IA_AggregateRootRepository a_AggregateRootRepository, IMapper mapper)
        {
            _a_AggregateRootRepository = a_AggregateRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<A_AggregateRootDTO>> Handle(GetA_AggregateRootsQuery request, CancellationToken cancellationToken)
        {
            var a_AggregateRoots = await _a_AggregateRootRepository.FindAllAsync(cancellationToken);
            return a_AggregateRoots.MapToA_AggregateRootDTOList(_mapper);
        }
    }
}