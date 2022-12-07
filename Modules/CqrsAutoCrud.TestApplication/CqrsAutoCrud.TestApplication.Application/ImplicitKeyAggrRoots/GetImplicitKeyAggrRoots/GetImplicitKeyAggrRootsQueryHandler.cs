using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRoots
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootsQueryHandler : IRequestHandler<GetImplicitKeyAggrRootsQuery, List<ImplicitKeyAggrRootDTO>>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetImplicitKeyAggrRootsQueryHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository, IMapper mapper)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ImplicitKeyAggrRootDTO>> Handle(GetImplicitKeyAggrRootsQuery request, CancellationToken cancellationToken)
        {
            var implicitKeyAggrRoots = await _implicitKeyAggrRootRepository.FindAllAsync(cancellationToken);
            return implicitKeyAggrRoots.MapToImplicitKeyAggrRootDTOList(_mapper);
        }
    }
}