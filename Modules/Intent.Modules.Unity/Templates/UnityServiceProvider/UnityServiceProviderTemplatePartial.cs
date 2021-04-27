using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Unity.Templates.UnityServiceProvider
{
    partial class UnityServiceProviderTemplate : CSharpTemplateBase<object>, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.Unity.ServiceProvider";

        public UnityServiceProviderTemplate(IOutputTarget outputTarget)
            : base(Identifier, outputTarget, null)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface("IServiceProvider"));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"UnityServiceProvider",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
