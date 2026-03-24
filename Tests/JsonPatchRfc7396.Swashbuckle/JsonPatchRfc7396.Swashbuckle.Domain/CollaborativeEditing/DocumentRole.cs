using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing
{
    /// <summary>
    /// Roles supported for document collaboration.
    /// </summary>
    public enum DocumentRole
    {
        Owner,
        Editor,
        Commenter,
        Viewer
    }
}