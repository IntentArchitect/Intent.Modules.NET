using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities.Throughput;
using DynamoDbTests.EnumAsStrings.Domain.Repositories.Throughput;
using DynamoDbTests.EnumAsStrings.Infrastructure.Persistence;
using DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Repositories.Throughput
{
    internal class ProvisionedDynamoDBRepository : DynamoDBRepositoryBase<Provisioned, ProvisionedDocument, string, object>, IProvisionedRepository
    {
        public ProvisionedDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}