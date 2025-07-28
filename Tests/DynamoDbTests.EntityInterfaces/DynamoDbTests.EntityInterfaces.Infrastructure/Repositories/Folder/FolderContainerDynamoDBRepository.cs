using Amazon.DynamoDBv2.DataModel;
using DynamoDbTests.EntityInterfaces.Domain.Entities.Folder;
using DynamoDbTests.EntityInterfaces.Domain.Repositories.Folder;
using DynamoDbTests.EntityInterfaces.Infrastructure.Persistence;
using DynamoDbTests.EntityInterfaces.Infrastructure.Persistence.Documents.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBRepository", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Repositories.Folder
{
    internal class FolderContainerDynamoDBRepository : DynamoDBRepositoryBase<IFolderContainer, FolderContainer, FolderContainerDocument, string, string>, IFolderContainerRepository
    {
        public FolderContainerDynamoDBRepository(IDynamoDBContext context, DynamoDBUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}