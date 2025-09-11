using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBUnitOfWorkInterface", Version = "1.0")]

namespace AwsLambdaFunction.Domain.Common.Interfaces
{
    public interface IDynamoDBUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}