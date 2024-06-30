using Intent.RoslynWeaver.Attributes;
using Solace.Tests.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace Solace.Tests.Eventing.Messages
{
    public record NotMappedIC : BaseMessage
    {
    }
}