using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.UpdateUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
    {
        private readonly IUserAccountRepository _userAccountRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateUserCommandHandler(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userAccount = await _userAccountRepository.FindByIdAsync(request.Id, cancellationToken);
            if (userAccount is null)
            {
                throw new NotFoundException($"Could not find UserAccount '{request.Id}'");
            }

            userAccount.Email = request.Email;
        }
    }
}