using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Common.Exceptions;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.UpdateChild
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateChildCommandHandler : IRequestHandler<UpdateChildCommand>
    {
        private readonly IChildRepository _childRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateChildCommandHandler(IChildRepository childRepository)
        {
            _childRepository = childRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateChildCommand request, CancellationToken cancellationToken)
        {
            var child = await _childRepository.FindByIdAsync(request.Id, cancellationToken);
            if (child is null)
            {
                throw new NotFoundException($"Could not find Child '{request.Id}'");
            }

            child.Name = request.Name;
            child.MyParentId = request.MyParentId;
        }
    }
}