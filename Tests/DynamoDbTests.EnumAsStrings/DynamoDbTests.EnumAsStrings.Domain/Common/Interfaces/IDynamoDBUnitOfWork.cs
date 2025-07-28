using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBUnitOfWorkInterface", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Domain.Common.Interfaces
{
    public interface IDynamoDBUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}