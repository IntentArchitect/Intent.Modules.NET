using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities.Throughput;
using DynamoDbTests.EntityInterfaces.Domain.Repositories.Throughput;
using DynamoDbTests.EntityInterfaces.Infrastructure.Persistence;
using DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Repositories.Throughput
{
    internal class ProvisionedDynamoDBRepository : DynamoDBRepositoryBase<IProvisioned, Provisioned, ProvisionedDocument, string, object>, IProvisionedRepository
    {
        public ProvisionedDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}