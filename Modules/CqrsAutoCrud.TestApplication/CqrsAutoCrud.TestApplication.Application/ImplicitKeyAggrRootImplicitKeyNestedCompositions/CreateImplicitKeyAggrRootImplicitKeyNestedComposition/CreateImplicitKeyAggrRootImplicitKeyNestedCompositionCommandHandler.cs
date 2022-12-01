using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions.CreateImplicitKeyAggrRootImplicitKeyNestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler : IRequestHandler<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand, Guid>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.ImplicitKeyAggrRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}' could not be found");
            }
            var newImplicitKeyNestedComposition = new ImplicitKeyNestedComposition
            {
#warning No matching field found for ImplicitKeyAggrRootId
                Attribute = request.Attribute,
            };

            aggregateRoot.ImplicitKeyNestedCompositions.Add(newImplicitKeyNestedComposition);
            await _implicitKeyAggrRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newImplicitKeyNestedComposition.Id;
        }
    }
}