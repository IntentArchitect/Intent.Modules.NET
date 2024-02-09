using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocumentInterface", Version = "1.0")]

namespace Redis.Om.Repositories.Domain.Repositories.Documents
{
    public interface ICategoryDocument
    {
        string Id { get; }
        string Name { get; }
    }
}