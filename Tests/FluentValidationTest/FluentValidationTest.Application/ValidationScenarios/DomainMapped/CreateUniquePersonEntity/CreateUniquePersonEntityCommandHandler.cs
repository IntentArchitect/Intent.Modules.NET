using FluentValidationTest.Domain.Entities.ValidationScenarios.IdentityConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateUniquePersonEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUniquePersonEntityCommandHandler : IRequestHandler<CreateUniquePersonEntityCommand>
    {
        private readonly IUniquePersonEntityRepository _uniquePersonEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreateUniquePersonEntityCommandHandler(IUniquePersonEntityRepository uniquePersonEntityRepository)
        {
            _uniquePersonEntityRepository = uniquePersonEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateUniquePersonEntityCommand request, CancellationToken cancellationToken)
        {
            var uniquePersonEntity = new UniquePersonEntity
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                ContactNumber = request.ContactNumber
            };

            _uniquePersonEntityRepository.Add(uniquePersonEntity);
        }
    }
}