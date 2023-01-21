using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CqrsAutoCrud.TestApplication.Domain.Common;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using CqrsAutoCrud.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateImplicitKeyAggrRootCommandHandler : IRequestHandler<UpdateImplicitKeyAggrRootCommand>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateImplicitKeyAggrRootCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(UpdateImplicitKeyAggrRootCommand request, CancellationToken cancellationToken)
        {
            var existingImplicitKeyAggrRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.Id, cancellationToken);
            existingImplicitKeyAggrRoot.Attribute = request.Attribute;
            existingImplicitKeyAggrRoot.ImplicitKeyNestedCompositions.UpdateCollection(request.ImplicitKeyNestedCompositions, (e, d) => e.Id == d.Id, UpdateImplicitKeyNestedComposition);
            return Unit.Value;
        }

        [IntentManaged(Mode.Fully)]
        private static void UpdateImplicitKeyNestedComposition(ImplicitKeyNestedComposition entity, UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO dto)
        {
            entity.Attribute = dto.Attribute;
        }
    }
}