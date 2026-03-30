using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateUniqueAccountEntity
{
    public class UpdateUniqueAccountEntityCommand : IRequest, ICommand
    {
        public UpdateUniqueAccountEntityCommand(Guid id, string username, string email)
        {
            Id = id;
            Username = username;
            Email = email;
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}