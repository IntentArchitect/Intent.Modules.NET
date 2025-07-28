using DynamoDbTests.EnumAsStrings.Domain.Entities.Folder;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Domain.Repositories.Folder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFolderContainerRepository : IDynamoDBRepository<FolderContainer, string, string>
    {
    }
}