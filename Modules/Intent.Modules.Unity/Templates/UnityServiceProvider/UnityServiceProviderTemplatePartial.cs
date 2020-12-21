using System.Collections.Generic;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.UnityServiceProvider
{
    partial class UnityServiceProviderTemplate : CSharpTemplateBase<object>, ITemplateBeforeExecutionHook
    {
        public const string Identifier = "Intent.Unity.ServiceProvider";

        public UnityServiceProviderTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface("IServiceProvider"));
            //Project.Application.EventDispatcher.Publish(ContainerRegistrationEvent.EventId, new Dictionary<string, string>()
            //{
            //    { "InterfaceType", "IServiceProvider"},
            //    { "ConcreteType", $"{Namespace}.{ClassName}" },
            //    { "InterfaceTypeTemplateId", null },
            //    { "ConcreteTypeTemplateId", Identifier }
            //});
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"UnityServiceProvider",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
