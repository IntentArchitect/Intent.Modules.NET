using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmValueObjectDocumentInterface", Version = "1.0")]

namespace Redis.Om.Repositories.Domain.Repositories.Documents
{
    public interface IAddressDocument
    {
        string Line1 { get; }
        string Line2 { get; }
        string City { get; }
        string PostalAddress { get; }
    }
}