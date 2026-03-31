using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdatePatternConstrainedEntity
{
    public class UpdatePatternConstrainedEntityCommand : IRequest, ICommand
    {
        public UpdatePatternConstrainedEntityCommand(Guid id, string emailAddress, string? websiteUrl, string slug, string referenceNumber, string? optionalPatternText, string base64)
        {
            Id = id;
            EmailAddress = emailAddress;
            WebsiteUrl = websiteUrl;
            Slug = slug;
            ReferenceNumber = referenceNumber;
            OptionalPatternText = optionalPatternText;
            Base64 = base64;
        }

        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public string? WebsiteUrl { get; set; }
        public string Slug { get; set; }
        public string ReferenceNumber { get; set; }
        public string? OptionalPatternText { get; set; }
        public string Base64 { get; set; }
    }
}