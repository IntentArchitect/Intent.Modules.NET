using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Children.CreateChild
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateChildCommandHandler : IRequestHandler<CreateChildCommand, Guid>
    {
        private readonly IChildRepository _childRepository;

        [IntentManaged(Mode.Merge)]
        public CreateChildCommandHandler(IChildRepository childRepository)
        {
            _childRepository = childRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateChildCommand request, CancellationToken cancellationToken)
        {
            var child = new Child
            {
                Name = request.Name,
                MyParentId = request.MyParentId
            };

            _childRepository.Add(child);
            await _childRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return child.Id;
        }
    }
}