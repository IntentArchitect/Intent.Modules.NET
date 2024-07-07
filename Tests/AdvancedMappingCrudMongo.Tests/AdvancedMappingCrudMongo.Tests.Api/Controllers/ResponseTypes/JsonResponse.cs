using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.JsonResponse", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Api.Controllers.ResponseTypes
{
    /// <summary>
    /// Implicit wrapping of types that serialize to non-complex values.
    /// </summary>
    /// <typeparam name="T">Types such as string, Guid, int, long, etc.</typeparam>
    public class JsonResponse<T>
    {
        public JsonResponse(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}