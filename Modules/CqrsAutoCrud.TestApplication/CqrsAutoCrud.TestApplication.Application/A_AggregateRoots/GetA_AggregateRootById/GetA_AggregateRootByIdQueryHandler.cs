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

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots.GetA_AggregateRootById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetA_AggregateRootByIdQueryHandler : IRequestHandler<GetA_AggregateRootByIdQuery, A_AggregateRootDTO>
    {
        private IA_AggregateRootRepository _a_AggregateRootRepository;
        private IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetA_AggregateRootByIdQueryHandler(IA_AggregateRootRepository a_AggregateRootRepository, IMapper mapper)
        {
            _a_AggregateRootRepository = a_AggregateRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<A_AggregateRootDTO> Handle(GetA_AggregateRootByIdQuery request, CancellationToken cancellationToken)
        {
            var a_AggregateRoot = await _a_AggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);
            return a_AggregateRoot.MapToA_AggregateRootDTO(_mapper);
        }
    }
}