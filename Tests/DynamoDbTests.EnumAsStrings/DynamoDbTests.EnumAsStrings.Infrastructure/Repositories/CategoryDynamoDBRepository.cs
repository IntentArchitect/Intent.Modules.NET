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
    internal class CategoryDynamoDBRepository : DynamoDBRepositoryBase<Category, CategoryDocument, string, object>, ICategoryRepository
    {
        public CategoryDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}