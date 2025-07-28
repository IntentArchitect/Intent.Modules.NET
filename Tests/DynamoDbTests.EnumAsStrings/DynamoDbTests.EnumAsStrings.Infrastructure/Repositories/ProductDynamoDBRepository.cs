using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EnumAsStrings.Domain.Entities;
using DynamoDbTests.EnumAsStrings.Domain.Repositories;
using DynamoDbTests.EnumAsStrings.Infrastructure.Persistence;
using DynamoDbTests.EnumAsStrings.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Infrastructure.Repositories
{
    internal class ProductDynamoDBRepository : DynamoDBRepositoryBase<Product, ProductDocument, string, object>, IProductRepository
    {
        public ProductDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}