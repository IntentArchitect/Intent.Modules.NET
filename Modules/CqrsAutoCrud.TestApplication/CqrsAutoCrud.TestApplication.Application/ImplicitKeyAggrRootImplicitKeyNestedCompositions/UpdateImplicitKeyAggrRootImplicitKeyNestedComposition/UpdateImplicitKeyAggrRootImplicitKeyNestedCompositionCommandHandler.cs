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

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler : IRequestHandler<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand request, CancellationToken cancellationToken)
        {
            var aggregateRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.ImplicitKeyAggrRootId, cancellationToken);
            if (aggregateRoot == null)
            {
                throw new InvalidOperationException($"{nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}' could not be found");
            }
            var element = aggregateRoot.ImplicitKeyNestedCompositions.FirstOrDefault(p => p.Id == request.Id);
            if (element == null)
            {
                throw new InvalidOperationException($"{nameof(ImplicitKeyNestedComposition)} of Id '{request.Id}' could not be found associated with {nameof(ImplicitKeyAggrRoot)} of Id '{request.ImplicitKeyAggrRootId}'");
            }
#warning No matching field found for ImplicitKeyAggrRootId
            element.Attribute = request.Attribute;
            return Unit.Value;
        }
    }
}