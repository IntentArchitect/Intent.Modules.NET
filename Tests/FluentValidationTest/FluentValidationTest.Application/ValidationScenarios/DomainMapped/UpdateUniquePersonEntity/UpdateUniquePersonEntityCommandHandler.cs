using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.IdentityConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniquePersonEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateUniquePersonEntityCommandHandler : IRequestHandler<UpdateUniquePersonEntityCommand>
    {
        private readonly IUniquePersonEntityRepository _uniquePersonEntityRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateUniquePersonEntityCommandHandler(IUniquePersonEntityRepository uniquePersonEntityRepository)
        {
            _uniquePersonEntityRepository = uniquePersonEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateUniquePersonEntityCommand request, CancellationToken cancellationToken)
        {
            var uniquePersonEntity = await _uniquePersonEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (uniquePersonEntity is null)
            {
                throw new NotFoundException($"Could not find UniquePersonEntity '{request.Id}'");
            }

            uniquePersonEntity.FirstName = request.FirstName;
            uniquePersonEntity.LastName = request.LastName;
            uniquePersonEntity.ContactNumber = request.ContactNumber;
        }
    }
}