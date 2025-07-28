using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.Domain.Entities.Throughput;
using DynamoDbTests.Domain.Repositories.Throughput;
using DynamoDbTests.Infrastructure.Persistence;
using DynamoDbTests.Infrastructure.Persistence.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.Infrastructure.Repositories.Throughput
{
    internal class ProvisionedDynamoDBRepository : DynamoDBRepositoryBase<Provisioned, ProvisionedDocument, string, object>, IProvisionedRepository
    {
        public ProvisionedDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}