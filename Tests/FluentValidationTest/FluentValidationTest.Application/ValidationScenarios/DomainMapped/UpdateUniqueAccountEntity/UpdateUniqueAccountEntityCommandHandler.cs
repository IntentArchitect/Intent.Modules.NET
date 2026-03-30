using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniqueAccountEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateUniqueAccountEntityCommandHandler : IRequestHandler<UpdateUniqueAccountEntityCommand>
    {
        private readonly IUniqueAccountEntityRepository _uniqueAccountEntityRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateUniqueAccountEntityCommandHandler(IUniqueAccountEntityRepository uniqueAccountEntityRepository)
        {
            _uniqueAccountEntityRepository = uniqueAccountEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateUniqueAccountEntityCommand request, CancellationToken cancellationToken)
        {
            var uniqueAccountEntity = await _uniqueAccountEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (uniqueAccountEntity is null)
            {
                throw new NotFoundException($"Could not find UniqueAccountEntity '{request.Id}'");
            }

            uniqueAccountEntity.Username = request.Username;
            uniqueAccountEntity.Email = request.Email;
        }
    }
}