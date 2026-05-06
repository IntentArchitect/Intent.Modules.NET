using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.JsonPatch.Templates.PatchExecutorInterface", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Common.Patching
{
    /// <summary>
    /// Defines abstraction for applying and (if applicable) validating JSON Merge Patch operations.
    /// </summary>
    /// <typeparam name="T">The type of object to apply the patch to.</typeparam>
    public interface IPatchExecutor<T>
    {
        /// <summary>
        /// Applies the patch to the target object and (if applicable) validates the result asynchronously.
        /// </summary>
        /// <param name="target">The object to apply the patch to.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task ApplyToAsync(T target, CancellationToken cancellationToken = default);
    }
}