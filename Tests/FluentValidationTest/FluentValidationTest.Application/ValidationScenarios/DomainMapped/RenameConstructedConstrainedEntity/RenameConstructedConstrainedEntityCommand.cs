using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.RenameConstructedConstrainedEntity
{
    public class RenameConstructedConstrainedEntityCommand : IRequest, ICommand
    {
        public RenameConstructedConstrainedEntityCommand(Guid id, string newTitle, string newCode)
        {
            Id = id;
            NewTitle = newTitle;
            NewCode = newCode;
        }

        public Guid Id { get; set; }
        public string NewTitle { get; set; }
        public string NewCode { get; set; }
    }
}