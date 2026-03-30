using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreatePatternConstrainedEntity
{
    public class CreatePatternConstrainedEntityCommand : IRequest, ICommand
    {
        public CreatePatternConstrainedEntityCommand(string emailAddress, string slug, string? websiteUrl, string referenceNumber, string? optionalPatternText)
        {
            EmailAddress = emailAddress;
            Slug = slug;
            WebsiteUrl = websiteUrl;
            ReferenceNumber = referenceNumber;
            OptionalPatternText = optionalPatternText;
        }

        public string EmailAddress { get; set; }
        public string Slug { get; set; }
        public string? WebsiteUrl { get; set; }
        public string ReferenceNumber { get; set; }
        public string? OptionalPatternText { get; set; }
    }
}