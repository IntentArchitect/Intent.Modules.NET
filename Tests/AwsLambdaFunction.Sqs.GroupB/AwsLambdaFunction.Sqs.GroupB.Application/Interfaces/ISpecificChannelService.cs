using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Application.Interfaces
{
    public interface ISpecificChannelService
    {
        Task SendSpecificTopicOne(PayloadDto dto, CancellationToken cancellationToken = default);
        Task SendSpecificTopicTwo(PayloadDto dto, CancellationToken cancellationToken = default);
    }
}