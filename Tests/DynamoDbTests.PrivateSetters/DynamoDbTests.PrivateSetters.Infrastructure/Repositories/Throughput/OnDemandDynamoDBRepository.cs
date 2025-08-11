using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities.Throughput;
using DynamoDbTests.PrivateSetters.Domain.Repositories.Throughput;
using DynamoDbTests.PrivateSetters.Infrastructure.Persistence;
using DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents.Throughput;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Repositories.Throughput
{
    internal class OnDemandDynamoDBRepository : DynamoDBRepositoryBase<OnDemand, OnDemandDocument, string, object>, IOnDemandRepository
    {
        public OnDemandDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}