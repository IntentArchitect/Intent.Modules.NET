using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateUniqueAccountEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUniqueAccountEntityCommandHandler : IRequestHandler<CreateUniqueAccountEntityCommand>
    {
        private readonly IUniqueAccountEntityRepository _uniqueAccountEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreateUniqueAccountEntityCommandHandler(IUniqueAccountEntityRepository uniqueAccountEntityRepository)
        {
            _uniqueAccountEntityRepository = uniqueAccountEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateUniqueAccountEntityCommand request, CancellationToken cancellationToken)
        {
            var uniqueAccountEntity = new UniqueAccountEntity
            {
                Username = request.Username,
                Email = request.Email
            };

            _uniqueAccountEntityRepository.Add(uniqueAccountEntity);
        }
    }
}