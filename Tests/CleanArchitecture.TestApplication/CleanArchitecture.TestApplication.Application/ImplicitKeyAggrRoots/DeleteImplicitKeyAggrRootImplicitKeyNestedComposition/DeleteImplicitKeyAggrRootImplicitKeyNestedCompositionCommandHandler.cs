using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRootImplicitKeyNestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler : IRequestHandler<DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(
            DeleteImplicitKeyAggrRootImplicitKeyNestedCompositionCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.ImplicitKeyAggrRootId, cancellationToken);

            if (aggregateRoot is null)
            {
                throw new NotFoundException($"{nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}' could not be found");
            }

            var element = aggregateRoot.ImplicitKeyNestedCompositions.FirstOrDefault(p => p.Id == request.Id);

            if (element is null)
            {
                throw new NotFoundException($"{nameof(ImplicitKeyNestedComposition)} of Id '{request.Id}' could not be found associated with {nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}'");
            }
            aggregateRoot.ImplicitKeyNestedCompositions.Remove(element);
            return Unit.Value;
        }
    }
}