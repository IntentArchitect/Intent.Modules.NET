using Intent.RoslynWeaver.Attributes;
using MassTransitFinbuckle.Test.Application.IntegrationServices.Contracts.Services.RequestResponse;
using MassTransitFinbuckle.Test.Application.RequestResponse.Test;
using MassTransitFinbuckle.Test.Infrastructure.Eventing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.RequestResponse.MapperRequestMessage", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Services.RequestResponse
{
    public class TestCommand : IMapperRequest
    {
        public TestCommand()
        {
        }

        public TestCommand(Application.IntegrationServices.Contracts.Services.RequestResponse.TestCommand dto)
        {
            Value = dto.Value;
        }

        public string Value { get; set; }

        public object CreateRequest()
        {
            return new Application.RequestResponse.Test.TestCommand(Value);
        }
    }
}