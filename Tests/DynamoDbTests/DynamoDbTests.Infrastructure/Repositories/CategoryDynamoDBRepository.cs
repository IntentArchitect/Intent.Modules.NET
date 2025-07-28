using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.Domain.Entities;
using DynamoDbTests.Domain.Repositories;
using DynamoDbTests.Infrastructure.Persistence;
using DynamoDbTests.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.Infrastructure.Repositories
{
    internal class CategoryDynamoDBRepository : DynamoDBRepositoryBase<Category, CategoryDocument, string, object>, ICategoryRepository
    {
        public CategoryDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}