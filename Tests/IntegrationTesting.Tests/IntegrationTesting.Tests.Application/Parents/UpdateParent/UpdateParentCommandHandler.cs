using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Parents.UpdateParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateParentCommandHandler : IRequestHandler<UpdateParentCommand>
    {
        private readonly IParentRepository _parentRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateParentCommandHandler(IParentRepository parentRepository)
        {
            _parentRepository = parentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateParentCommand request, CancellationToken cancellationToken)
        {
            var parent = await _parentRepository.FindByIdAsync(request.Id, cancellationToken);
            if (parent is null)
            {
                throw new NotFoundException($"Could not find Parent '{request.Id}'");
            }

            parent.Name = request.Name;
        }
    }
}