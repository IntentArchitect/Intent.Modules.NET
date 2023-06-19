using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.GetImplicitKeyAggrRootImplicitKeyNestedCompositionById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQueryHandler : IRequestHandler<GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery, ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>
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
        public async Task<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto> Handle(
            GetImplicitKeyAggrRootImplicitKeyNestedCompositionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.ImplicitKeyAggrRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}' could not be found");
            }

            var element = aggregateRoot.ImplicitKeyNestedCompositions.FirstOrDefault(p => p.Id == request.Id);

            if (element is null)
            {
                throw new NotFoundException($"Could not find ImplicitKeyNestedComposition {request.Id}");
            }
            return element.MapToImplicitKeyAggrRootImplicitKeyNestedCompositionDto(_mapper);
        }
    }
}