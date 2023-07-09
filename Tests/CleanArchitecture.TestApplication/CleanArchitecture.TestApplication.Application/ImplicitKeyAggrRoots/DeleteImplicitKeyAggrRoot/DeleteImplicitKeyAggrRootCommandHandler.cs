using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.DeleteImplicitKeyAggrRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteImplicitKeyAggrRootCommandHandler : IRequestHandler<DeleteImplicitKeyAggrRootCommand>
    {
        private readonly IImplicitKeyAggrRootRepository _implicitKeyAggrRootRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteImplicitKeyAggrRootCommandHandler(IImplicitKeyAggrRootRepository implicitKeyAggrRootRepository)
        {
            _implicitKeyAggrRootRepository = implicitKeyAggrRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteImplicitKeyAggrRootCommand request, CancellationToken cancellationToken)
        {
            var existingImplicitKeyAggrRoot = await _implicitKeyAggrRootRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingImplicitKeyAggrRoot is null)
            {
                throw new NotFoundException($"Could not find ImplicitKeyAggrRoot '{request.Id}' ");
            }
            _implicitKeyAggrRootRepository.Remove(existingImplicitKeyAggrRoot);
            return Unit.Value;
        }
    }
}