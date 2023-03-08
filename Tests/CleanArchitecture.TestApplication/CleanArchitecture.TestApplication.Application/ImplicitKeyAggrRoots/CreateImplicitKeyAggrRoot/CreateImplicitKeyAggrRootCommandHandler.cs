using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateImplicitKeyAggrRootCommandHandler : IRequestHandler<CreateImplicitKeyAggrRootCommand, Guid>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Ignore)]
        public CreateImplicitKeyAggrRootCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateImplicitKeyAggrRootCommand request, CancellationToken cancellationToken)
        {
            var newImplicitKeyAggrRoot = new ImplicitKeyAggrRoot
            {
                Attribute = request.Attribute,
                ImplicitKeyNestedCompositions = request.ImplicitKeyNestedCompositions.Select(CreateImplicitKeyNestedComposition).ToList(),
            };

            _implicitKeyAggrRootRepository.Add(newImplicitKeyAggrRoot);
            await _implicitKeyAggrRootRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newImplicitKeyAggrRoot.Id;
        }

        [IntentManaged(Mode.Fully)]
        private static ImplicitKeyNestedComposition CreateImplicitKeyNestedComposition(CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto dto)
        {
            return new ImplicitKeyNestedComposition
            {
                Attribute = dto.Attribute,
            };
        }
    }
}