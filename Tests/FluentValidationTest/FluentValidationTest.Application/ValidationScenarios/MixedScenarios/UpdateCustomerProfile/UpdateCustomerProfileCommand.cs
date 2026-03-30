using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.UpdateCustomerProfile
{
    public class UpdateCustomerProfileCommand : IRequest, ICommand
    {
        public UpdateCustomerProfileCommand(Guid id, string displayName, string email, string? optionalBio)
        {
            Id = id;
            DisplayName = displayName;
            Email = email;
            OptionalBio = optionalBio;
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string? OptionalBio { get; set; }
    }
}