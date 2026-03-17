using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.DtoContract", Version = "2.0")]

namespace MassTransit.RequestResponse.Client.Application.IntegrationServices.Contracts.AzureServiceBus.Services.RequestResponse.CQRS
{
    public class QueryGuidReturn
    {
        public QueryGuidReturn()
        {
            Input = null!;
        }

        public string Input { get; set; }

        public static QueryGuidReturn Create(string input)
        {
            return new QueryGuidReturn
            {
                Input = input
            };
        }
    }
}