using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Customers
{
    public class CreateQuoteCommand
    {
        public CreateQuoteCommand()
        {
            RefNo = null!;
        }

        public string RefNo { get; set; }
        public Guid PersonId { get; set; }
        public string? PersonEmail { get; set; }

        public static CreateQuoteCommand Create(string refNo, Guid personId, string? personEmail)
        {
            return new CreateQuoteCommand
            {
                RefNo = refNo,
                PersonId = personId,
                PersonEmail = personEmail
            };
        }
    }
}