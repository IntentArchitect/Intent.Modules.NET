using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Domain.CollaborativeEditing
{
    public class Actor : ValueObject
    {
        public Actor(string userId, string displayName)
        {
            UserId = userId;
            DisplayName = displayName;
        }

        [IntentMerge]
        protected Actor()
        {
            UserId = null!;
            DisplayName = null!;
        }

        public string UserId { get; private set; }
        public string DisplayName { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return UserId;
            yield return DisplayName;
        }
    }
}