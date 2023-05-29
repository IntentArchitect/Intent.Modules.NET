using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Entities
{
    public static class OperationExtensions
    {
        public static bool IsAsync(this OperationModel operation)
        {
            return operation.HasStereotype("Asynchronous") || operation.Name.EndsWith("Async");
        }
    }
}
