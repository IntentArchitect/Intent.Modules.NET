using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities;
using DynamoDbTests.EntityInterfaces.Domain.Repositories;
using DynamoDbTests.EntityInterfaces.Infrastructure.Persistence;
using DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Repositories
{
    internal class CustomerDynamoDBRepository : DynamoDBRepositoryBase<ICustomer, Customer, CustomerDocument, string, object>, ICustomerRepository
    {
        public CustomerDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}