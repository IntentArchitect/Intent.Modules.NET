using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.RegisterCustomer
{
    public class RegisterCustomerCommand : IRequest, ICommand
    {
        public RegisterCustomerCommand(CustomerRegistrationDto customerRegistration)
        {
            CustomerRegistration = customerRegistration;
        }

        public CustomerRegistrationDto CustomerRegistration { get; set; }
    }
}