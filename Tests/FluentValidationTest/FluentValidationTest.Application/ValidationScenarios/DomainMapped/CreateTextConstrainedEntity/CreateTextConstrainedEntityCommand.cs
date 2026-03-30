using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateTextConstrainedEntity
{
    public class CreateTextConstrainedEntityCommand : IRequest, ICommand
    {
        public CreateTextConstrainedEntityCommand(string shortCode, string displayName, string? description, string requiredName, string? optionalNotes, string? nullButRequired, int defaultIntButRequired)
        {
            ShortCode = shortCode;
            DisplayName = displayName;
            Description = description;
            RequiredName = requiredName;
            OptionalNotes = optionalNotes;
            NullButRequired = nullButRequired;
            DefaultIntButRequired = defaultIntButRequired;
        }

        public string ShortCode { get; set; }
        public string DisplayName { get; set; }
        public string RequiredName { get; set; }
        public string? Description { get; set; }
        public string? OptionalNotes { get; set; }
        public string? NullButRequired { get; set; }
        public int DefaultIntButRequired { get; set; }
    }
}