using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Api;

namespace Intent.Modules.Application.Contracts
{

    public static class OperationExtensions
    {
        public static bool IsAsync(this OperationModel operation)
        {
            return !operation.HasSynchronous();
        }
    }
}
