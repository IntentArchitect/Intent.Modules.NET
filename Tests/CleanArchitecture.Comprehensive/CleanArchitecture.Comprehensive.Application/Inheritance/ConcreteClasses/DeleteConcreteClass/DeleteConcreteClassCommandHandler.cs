using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.DeleteConcreteClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteConcreteClassCommandHandler : IRequestHandler<DeleteConcreteClassCommand>
    {
        private readonly IConcreteClassRepository _concreteClassRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteConcreteClassCommandHandler(IConcreteClassRepository concreteClassRepository)
        {
            _concreteClassRepository = concreteClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteConcreteClassCommand request, CancellationToken cancellationToken)
        {
            var existingConcreteClass = await _concreteClassRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingConcreteClass is null)
            {
                throw new NotFoundException($"Could not find ConcreteClass '{request.Id}'");
            }

            _concreteClassRepository.Remove(existingConcreteClass);
        }
    }
}