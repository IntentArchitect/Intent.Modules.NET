using Intent.RoslynWeaver.Attributes;
using Morcatko.AspNetCore.JsonMergePatch;
using Scalar.NET11.Bearer.Application.Common.Patching;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchExecutor", Version = "1.0")]

namespace Scalar.NET11.Bearer.Api.Patching
{
    public class JsonMergePatchExecutor<T> : IPatchExecutor<T>
        where T : class
    {
        private readonly JsonMergePatchDocument<T> _document;

        public JsonMergePatchExecutor(JsonMergePatchDocument<T> document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public async Task ApplyToAsync(T target, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(target);

            _document.ApplyTo(target);
        }
    }
}