using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.RecursiveDtos.ValidateRecursiveNode
{
    public class ValidateRecursiveNodeCommand : IRequest, ICommand
    {
        public ValidateRecursiveNodeCommand(RecursiveNodeDto root)
        {
            Root = root;
        }

        public RecursiveNodeDto Root { get; set; }
    }
}