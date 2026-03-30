using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateTextConstrainedEntity
{
    public class UpdateTextConstrainedEntityCommand : IRequest, ICommand
    {
        public UpdateTextConstrainedEntityCommand(Guid id, string displayName, string shortCode, string? description, string requiredName, string? optionalNotes)
        {
            Id = id;
            DisplayName = displayName;
            ShortCode = shortCode;
            Description = description;
            RequiredName = requiredName;
            OptionalNotes = optionalNotes;
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string ShortCode { get; set; }
        public string? Description { get; set; }
        public string RequiredName { get; set; }
        public string? OptionalNotes { get; set; }
    }
}