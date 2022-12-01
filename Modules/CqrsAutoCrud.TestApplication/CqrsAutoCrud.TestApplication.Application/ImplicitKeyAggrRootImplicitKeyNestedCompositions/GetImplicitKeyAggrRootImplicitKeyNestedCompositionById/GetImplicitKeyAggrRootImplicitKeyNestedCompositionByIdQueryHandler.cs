using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions.GetImplicitKeyAggrRootImplicitKeyNestedCompositionById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler : IRequestHandler<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery, ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository, IMapper mapper)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO> Handle(GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.ImplicitKeyAggrRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}' could not be found");
            }

            var element = aggregateRoot.ImplicitKeyNestedCompositions.FirstOrDefault(p => p.Id == request.Id);
            return element == null ? null : element.MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDTO(_mapper);
        }
    }
}