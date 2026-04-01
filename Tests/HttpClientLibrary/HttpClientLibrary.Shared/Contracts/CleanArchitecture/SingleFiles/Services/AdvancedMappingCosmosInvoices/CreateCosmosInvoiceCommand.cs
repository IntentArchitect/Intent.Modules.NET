using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace HttpClientLibrary.Shared.Contracts.CleanArchitecture.SingleFiles.Services.AdvancedMappingCosmosInvoices
{
    public class CreateCosmosInvoiceCommand
    {
        public CreateCosmosInvoiceCommand()
        {
            Description = null!;
        }

        public string Description { get; set; }

        public static CreateCosmosInvoiceCommand Create(string description)
        {
            return new CreateCosmosInvoiceCommand
            {
                Description = description
            };
        }
    }
}