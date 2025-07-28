using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities;
using DynamoDbTests.PrivateSetters.Domain.Repositories;
using DynamoDbTests.PrivateSetters.Infrastructure.Persistence;
using DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Repositories
{
    internal class CategoryDynamoDBRepository : DynamoDBRepositoryBase<Category, CategoryDocument, string, object>, ICategoryRepository
    {
        public CategoryDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}