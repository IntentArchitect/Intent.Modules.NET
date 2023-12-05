using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Validation.Validated
{
    public class ValidatedCommand : IRequest, ICommand
    {
        public ValidatedCommand(string field, string email, string cascaseTest)
        {
            Field = field;
            Email = email;
            CascaseTest = cascaseTest;
        }

        public string Field { get; set; }
        public string Email { get; set; }
        public string CascaseTest { get; set; }
    }
}