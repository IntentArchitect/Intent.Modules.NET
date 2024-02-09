using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.DeleteChild
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteChildCommandHandler : IRequestHandler<DeleteChildCommand>
    {
        private readonly IChildRepository _childRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteChildCommandHandler(IChildRepository childRepository)
        {
            _childRepository = childRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteChildCommand request, CancellationToken cancellationToken)
        {
            var child = await _childRepository.FindByIdAsync(request.Id, cancellationToken);
            if (child is null)
            {
                throw new NotFoundException($"Could not find Child '{request.Id}'");
            }

            _childRepository.Remove(child);
        }
    }
}