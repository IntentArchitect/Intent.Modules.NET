using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateConstructedConstrainedEntity
{
    public class CreateConstructedConstrainedEntityCommand : IRequest, ICommand
    {
        public CreateConstructedConstrainedEntityCommand(string title, string code, string? optionalComment)
        {
            Title = title;
            Code = code;
            OptionalComment = optionalComment;
        }

        public string Title { get; set; }
        public string Code { get; set; }
        public string? OptionalComment { get; set; }
    }
}