using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing
{
    /// <summary>
    /// Lifecycle state of a document.
    /// </summary>
    public enum DocumentStatus
    {
        Draft,
        Published,
        Archived
    }
}