using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.ClientContracts.DtoContract", Version = "2.0")]

namespace MassTransit.RabbitMQ.Application.IntegrationServices.Contracts.Services.RequestResponse.CQRS
{
    public class QueryResponseDtoReturn
    {
        public QueryResponseDtoReturn()
        {
            Input = null!;
        }

        public string Input { get; set; }

        public static QueryResponseDtoReturn Create(string input)
        {
            return new QueryResponseDtoReturn
            {
                Input = input
            };
        }
    }
}