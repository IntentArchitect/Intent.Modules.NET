using FluentValidationTest.Domain.Entities.ValidationScenarios.StressSuite;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.StressSuite;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.CreateUser
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
    {
        private readonly IUserAccountRepository _userAccountRepository;
        [IntentManaged(Mode.Merge)]
        public CreateUserCommandHandler(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userAccount = new UserAccount
            {
                Email = request.Email
            };

            _userAccountRepository.Add(userAccount);
        }
    }
}