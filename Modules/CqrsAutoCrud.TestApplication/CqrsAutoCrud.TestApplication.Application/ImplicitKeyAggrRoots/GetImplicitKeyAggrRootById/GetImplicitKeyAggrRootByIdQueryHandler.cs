using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootByIdQueryHandler : IRequestHandler<GetImplicitKeyAggrRootByIdQuery, ImplicitKeyAggrRootDTO>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetImplicitKeyAggrRootByIdQueryHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository, IMapper mapper)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ImplicitKeyAggrRootDTO> Handle(GetImplicitKeyAggrRootByIdQuery request, CancellationToken cancellationToken)
        {
            var implicitKeyAggrRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.Id, cancellationToken);
            return implicitKeyAggrRoot.MapToImplicitKeyAggrRootDTO(_mapper);
        }
    }
}