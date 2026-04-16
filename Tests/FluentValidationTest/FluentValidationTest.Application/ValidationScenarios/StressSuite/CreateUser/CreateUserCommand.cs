using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.CreateUser
{
    public class CreateUserCommand : IRequest, ICommand
    {
        public CreateUserCommand(string email)
        {
            Email = email;
        }

        public string Email { get; set; }
    }
}