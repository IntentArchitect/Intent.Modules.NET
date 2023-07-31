using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Validation.Validated
{
    public class ValidatedCommand : IRequest, ICommand
    {
        public ValidatedCommand(string field)
        {
            Field = field;
        }

        public string Field { get; set; }
    }
}