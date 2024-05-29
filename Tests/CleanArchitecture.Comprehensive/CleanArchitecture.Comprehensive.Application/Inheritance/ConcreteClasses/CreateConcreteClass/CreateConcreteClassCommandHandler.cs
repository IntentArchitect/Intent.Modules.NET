using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.CreateConcreteClass
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateConcreteClassCommandHandler : IRequestHandler<CreateConcreteClassCommand, Guid>
    {
        private readonly IConcreteClassRepository _concreteClassRepository;

        [IntentManaged(Mode.Merge)]
        public CreateConcreteClassCommandHandler(IConcreteClassRepository concreteClassRepository)
        {
            _concreteClassRepository = concreteClassRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateConcreteClassCommand request, CancellationToken cancellationToken)
        {
            var newConcreteClass = new ConcreteClass
            {
                ConcreteAttr = request.ConcreteAttr,
                BaseAttr = request.BaseAttr,
            };

            _concreteClassRepository.Add(newConcreteClass);
            await _concreteClassRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newConcreteClass.Id;
        }
    }
}