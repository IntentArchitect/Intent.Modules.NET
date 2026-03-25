using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ValueObjects.ValueObject", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Domain.CollaborativeEditing
{
    public class DocumentContent : ValueObject
    {
        public DocumentContent(string format, string text, string json)
        {
            Format = format;
            Text = text;
            Json = json;
        }

        [IntentMerge]
        protected DocumentContent()
        {
            Format = null!;
            Text = null!;
            Json = null!;
        }

        public string Format { get; private set; }
        public string Text { get; private set; }
        public string Json { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            // Using a yield return statement to return each element one at a time
            yield return Format;
            yield return Text;
            yield return Json;
        }
    }
}