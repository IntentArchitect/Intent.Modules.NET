using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.PrivateSetters.Domain.Entities.Folder;
using DynamoDbTests.PrivateSetters.Domain.Repositories.Folder;
using DynamoDbTests.PrivateSetters.Infrastructure.Persistence;
using DynamoDbTests.PrivateSetters.Infrastructure.Persistence.Documents.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.PrivateSetters.Infrastructure.Repositories.Folder
{
    internal class FolderContainerDynamoDBRepository : DynamoDBRepositoryBase<FolderContainer, FolderContainerDocument, string, string>, IFolderContainerRepository
    {
        public FolderContainerDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}