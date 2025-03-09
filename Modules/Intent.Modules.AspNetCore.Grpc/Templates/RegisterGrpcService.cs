using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.AspNetCore.Grpc.Templates
{
    internal class RegisterGrpcService(ICSharpTemplate template)
    {
        public ICSharpTemplate Template { get; } = template;
    }
}
