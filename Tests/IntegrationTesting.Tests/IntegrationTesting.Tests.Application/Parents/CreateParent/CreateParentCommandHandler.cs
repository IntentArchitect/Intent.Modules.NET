using System;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTesting.Tests.Domain.Entities;
using IntegrationTesting.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.Parents.CreateParent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateParentCommandHandler : IRequestHandler<CreateParentCommand, Guid>
    {
        private readonly IParentRepository _parentRepository;

        [IntentManaged(Mode.Merge)]
        public CreateParentCommandHandler(IParentRepository parentRepository)
        {
            _parentRepository = parentRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateParentCommand request, CancellationToken cancellationToken)
        {
            var parent = new Parent
            {
                Name = request.Name
            };

            _parentRepository.Add(parent);
            await _parentRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return parent.Id;
        }
    }
}