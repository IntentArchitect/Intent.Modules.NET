using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.StressSuite.UpdateUser
{
    public class UpdateUserCommand : IRequest, ICommand
    {
        public UpdateUserCommand(Guid id, string email)
        {
            Id = id;
            Email = email;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}